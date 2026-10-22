using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Implementations
{
    public class BuoiHocService : IBuoiHocService
    {
        private readonly IUnitOfWork _uow;

        public BuoiHocService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<BuoiHocSearchResultVM> GetFilteredSessionsAsync(
            int? maLop = null,
            string? phong = null,
            string? trangThai = null,
            string? search = null,
            int page = 1,
            int pageSize = 8)
        {
            var query = _uow.BuoiHocs.Query()
                .Include(b => b.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .AsQueryable();

            var allActive = await _uow.BuoiHocs.Query().Where(b => b.TrangThai != "Đã hủy").ToListAsync();
            var conflictIds = new HashSet<int>();
            for (int i = 0; i < allActive.Count; i++)
            {
                for (int j = i + 1; j < allActive.Count; j++)
                {
                    var b1 = allActive[i];
                    var b2 = allActive[j];
                    if ((b1.PhongHoc == b2.PhongHoc || b1.GiangVien == b2.GiangVien || b1.MaLop == b2.MaLop) &&
                        (b1.ThoiGianBatDau < b2.ThoiGianKetThuc && b1.ThoiGianKetThuc > b2.ThoiGianBatDau))
                    {
                        conflictIds.Add(b1.MaBuoiHoc);
                        conflictIds.Add(b2.MaBuoiHoc);
                    }
                }
            }

            int totalSessions = await _uow.BuoiHocs.CountAsync();
            int completedSessions = await _uow.BuoiHocs.CountAsync(b => b.TrangThai == "Đã hoàn thành");
            int scheduledSessions = await _uow.BuoiHocs.CountAsync(b => b.TrangThai == "Đã lên lịch");
            int canceledSessions = await _uow.BuoiHocs.CountAsync(b => b.TrangThai == "Đã hủy");

            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(b => b.MaLop == maLop.Value);
            }

            if (!string.IsNullOrWhiteSpace(phong))
            {
                query = query.Where(b => b.PhongHoc == phong);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(b => b.TrangThai == trangThai);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(b => (b.GiangVien != null && b.GiangVien.ToLower().Contains(s)) ||
                                         (b.PhongHoc != null && b.PhongHoc.ToLower().Contains(s)) ||
                                         (b.NoiDung != null && b.NoiDung.ToLower().Contains(s)) ||
                                         (b.LopHoc != null && b.LopHoc.TenLop.ToLower().Contains(s)));
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var items = await query
                .OrderByDescending(b => b.ThoiGianBatDau)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allSessions = await _uow.BuoiHocs.Query().Include(b => b.LopHoc).ToListAsync();
            var activeSessions = allSessions.Where(b => b.TrangThai != "Đã hủy").ToList();
            int soThi = activeSessions.Count(b => b.NoiDung != null && (b.NoiDung.ToLower().Contains("thi") || b.NoiDung.ToLower().Contains("kiểm tra")));
            int soChinhKhoa = activeSessions.Count(b => b.NoiDung == null || (!b.NoiDung.ToLower().Contains("thi") && !b.NoiDung.ToLower().Contains("kiểm tra")));
            var danhSachPhong = allSessions.Select(b => b.PhongHoc).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            var danhSachGiangVien = allSessions.Select(b => b.GiangVien).Where(g => !string.IsNullOrEmpty(g)).Distinct().ToList();

            return new BuoiHocSearchResultVM
            {
                Items = items,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = page,
                ConflictIds = conflictIds,
                TotalSessions = totalSessions,
                CompletedSessions = completedSessions,
                ScheduledSessions = scheduledSessions,
                CanceledSessions = canceledSessions,
                SoThi = soThi,
                SoChinhKhoa = soChinhKhoa,
                DanhSachPhong = danhSachPhong,
                DanhSachGiangVien = danhSachGiangVien,
                AllBuoiHocs = allSessions
            };
        }

        public async Task<BuoiHoc?> GetByIdAsync(int id)
        {
            return await _uow.BuoiHocs.Query()
                .Include(b => b.LopHoc)
                .FirstOrDefaultAsync(b => b.MaBuoiHoc == id);
        }

        public async Task<List<BuoiHoc>> GetByLopHocAsync(int maLop)
        {
            var list = await _uow.BuoiHocs.GetByLopHocAsync(maLop);
            return list.ToList();
        }

        public async Task<List<BuoiHoc>> GetScheduleForClassesAsync(IEnumerable<int> classIds, DateTime fromDate, DateTime toDate)
        {
            var list = await _uow.BuoiHocs.GetScheduleForClassesAsync(classIds, fromDate, toDate);
            return list.ToList();
        }

        public async Task<(bool Success, string Message, int? NewId)> CreateSessionAsync(BuoiHoc buoiHoc)
        {
            bool coHocVienDongHocPhi = await _uow.DangKyHocs.ExistsAsync(dk =>
                dk.MaLop == buoiHoc.MaLop && (dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học"));

            if (!coHocVienDongHocPhi)
            {
                return (false, "Lớp này chưa có học viên nào đóng học phí, không được xếp lịch.", null);
            }

            if (buoiHoc.ThoiGianKetThuc <= buoiHoc.ThoiGianBatDau)
            {
                return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.", null);
            }

            if (buoiHoc.ThoiGianBatDau < DateTime.Now.AddDays(-1))
            {
                return (false, "Thời gian học không được nằm trong quá khứ.", null);
            }

            var lopHoc = await _uow.LopHocs.GetByIdAsync(buoiHoc.MaLop);
            if (lopHoc != null)
            {
                int soBuoiChuaHuy = await _uow.BuoiHocs.CountAsync(b => b.MaLop == buoiHoc.MaLop && b.TrangThai != "Đã hủy");
                if (soBuoiChuaHuy >= lopHoc.SoBuoiHoc)
                {
                    return (false, $"Lớp này đã xếp đủ {lopHoc.SoBuoiHoc} buổi học theo quy định.", null);
                }
            }

            bool isTrungLich = await _uow.BuoiHocs.ExistsAsync(b =>
                b.TrangThai != "Đã hủy" &&
                (b.MaLop == buoiHoc.MaLop || b.PhongHoc == buoiHoc.PhongHoc || b.GiangVien == buoiHoc.GiangVien) &&
                (buoiHoc.ThoiGianBatDau < b.ThoiGianKetThuc && buoiHoc.ThoiGianKetThuc > b.ThoiGianBatDau)
            );

            if (isTrungLich)
            {
                return (false, "Lịch bị trùng! Kiểm tra lại Giảng viên, Phòng học hoặc Lớp đã có lịch trong khung giờ này.", null);
            }

            if (string.IsNullOrEmpty(buoiHoc.TrangThai)) buoiHoc.TrangThai = "Đã lên lịch";

            await _uow.BuoiHocs.AddAsync(buoiHoc);
            await _uow.CompleteAsync();

            return (true, "Tạo buổi học thành công!", buoiHoc.MaBuoiHoc);
        }

        public async Task<(bool Success, string Message)> UpdateSessionAsync(BuoiHoc buoiHoc)
        {
            var existing = await _uow.BuoiHocs.GetByIdAsync(buoiHoc.MaBuoiHoc);
            if (existing == null) return (false, "Buổi học không tồn tại.");

            if (buoiHoc.ThoiGianKetThuc <= buoiHoc.ThoiGianBatDau)
            {
                return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }

            bool isTrungLich = await _uow.BuoiHocs.ExistsAsync(b =>
                b.MaBuoiHoc != buoiHoc.MaBuoiHoc &&
                b.TrangThai != "Đã hủy" &&
                (b.MaLop == buoiHoc.MaLop || b.PhongHoc == buoiHoc.PhongHoc || b.GiangVien == buoiHoc.GiangVien) &&
                (buoiHoc.ThoiGianBatDau < b.ThoiGianKetThuc && buoiHoc.ThoiGianKetThuc > b.ThoiGianBatDau)
            );

            if (isTrungLich)
            {
                return (false, "Lịch bị trùng! Vui lòng chọn thời gian, phòng hoặc giảng viên khác.");
            }

            existing.MaLop = buoiHoc.MaLop;
            existing.ThoiGianBatDau = buoiHoc.ThoiGianBatDau;
            existing.ThoiGianKetThuc = buoiHoc.ThoiGianKetThuc;
            existing.PhongHoc = buoiHoc.PhongHoc;
            existing.GiangVien = buoiHoc.GiangVien;
            existing.NoiDung = buoiHoc.NoiDung;
            existing.TrangThai = buoiHoc.TrangThai;

            _uow.BuoiHocs.Update(existing);
            await _uow.CompleteAsync();

            return (true, "Cập nhật buổi học thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteSessionAsync(int id)
        {
            var buoiHoc = await _uow.BuoiHocs.GetByIdAsync(id);
            if (buoiHoc == null) return (false, "Không tìm thấy buổi học.");

            _uow.BuoiHocs.Delete(buoiHoc);
            await _uow.CompleteAsync();
            return (true, "Đã xóa buổi học thành công.");
        }

        public async Task<(bool Success, string Message)> CompleteSessionAsync(int id)
        {
            var buoiHoc = await _uow.BuoiHocs.GetByIdAsync(id);
            if (buoiHoc == null) return (false, "Không tìm thấy buổi học.");

            buoiHoc.TrangThai = "Đã hoàn thành";
            _uow.BuoiHocs.Update(buoiHoc);
            await _uow.CompleteAsync();
            return (true, "Đã hoàn thành buổi học!");
        }

        public async Task<(bool Success, string Message)> CancelSessionAsync(int id)
        {
            var buoiHoc = await _uow.BuoiHocs.GetByIdAsync(id);
            if (buoiHoc == null) return (false, "Không tìm thấy buổi học.");

            buoiHoc.TrangThai = "Đã hủy";
            _uow.BuoiHocs.Update(buoiHoc);
            await _uow.CompleteAsync();
            return (true, "Đã hủy buổi học!");
        }
    }
}
