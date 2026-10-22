// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Nội dung thực hiện: Module 2 - Quản lý Lớp học, Tìm kiếm, Lọc đa tiêu chí, Sắp xếp, Phân trang LINQ (Service Layer)

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
    public class LopHocService : ILopHocService
    {
        private readonly IUnitOfWork _uow;

        public LopHocService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LopHocSearchResultVM> GetFilteredClassesAsync(
            int page = 1,
            int pageSize = 6,
            string search = "",
            int? maChuongTrinh = null,
            string trinhDo = "",
            string trangThai = "",
            double? hocPhiMin = null,
            double? hocPhiMax = null,
            string hanDangKyStatus = "",
            string choTrongStatus = "",
            string sortBy = "")
        {
            var query = _uow.LopHocs.Query()
                .Include(l => l.ChuongTrinhDaoTao)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(l => l.TenLop.ToLower().Contains(s) ||
                                         (l.ChuongTrinhDaoTao != null && l.ChuongTrinhDaoTao.TenChuongTrinh.ToLower().Contains(s)) ||
                                         (l.GiangVien != null && l.GiangVien.ToLower().Contains(s)));
            }

            if (maChuongTrinh.HasValue && maChuongTrinh.Value > 0)
            {
                query = query.Where(l => l.MaChuongTrinh == maChuongTrinh.Value);
            }

            if (!string.IsNullOrWhiteSpace(trinhDo))
            {
                query = query.Where(l => l.TrinhDoDauVao == trinhDo);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            if (hocPhiMin.HasValue && hocPhiMin.Value >= 0)
            {
                query = query.Where(l => l.HocPhi >= hocPhiMin.Value);
            }
            if (hocPhiMax.HasValue && hocPhiMax.Value > 0)
            {
                query = query.Where(l => l.HocPhi <= hocPhiMax.Value);
            }

            DateTime today = DateTime.Now;
            if (hanDangKyStatus == "con_han")
            {
                query = query.Where(l => l.HanDangKy >= today.Date && l.NgayBatDauDangKy <= today);
            }
            else if (hanDangKyStatus == "het_han")
            {
                query = query.Where(l => l.HanDangKy < today.Date);
            }

            switch (sortBy)
            {
                case "ten_asc":
                    query = query.OrderBy(l => l.TenLop);
                    break;
                case "ten_desc":
                    query = query.OrderByDescending(l => l.TenLop);
                    break;
                case "han_asc":
                    query = query.OrderBy(l => l.HanDangKy);
                    break;
                case "han_desc":
                    query = query.OrderByDescending(l => l.HanDangKy);
                    break;
                case "hocphi_asc":
                    query = query.OrderBy(l => l.HocPhi);
                    break;
                case "hocphi_desc":
                    query = query.OrderByDescending(l => l.HocPhi);
                    break;
                case "khaigiang_asc":
                    query = query.OrderBy(l => l.NgayKhaiGiang);
                    break;
                case "khaigiang_desc":
                    query = query.OrderByDescending(l => l.NgayKhaiGiang);
                    break;
                default:
                    query = query.OrderByDescending(l => l.MaLop);
                    break;
            }

            var allFiltered = await query.ToListAsync();
            var allLopIds = allFiltered.Select(l => l.MaLop).ToList();

            var siSoMap = await _uow.DangKyHocs.Query()
                .Where(d => allLopIds.Contains(d.MaLop) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"))
                .GroupBy(d => d.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            if (choTrongStatus == "con_cho")
            {
                allFiltered = allFiltered.Where(l => (siSoMap.ContainsKey(l.MaLop) ? siSoMap[l.MaLop] : 0) < l.SiSoToiDa).ToList();
            }
            else if (choTrongStatus == "het_cho")
            {
                allFiltered = allFiltered.Where(l => (siSoMap.ContainsKey(l.MaLop) ? siSoMap[l.MaLop] : 0) >= l.SiSoToiDa).ToList();
            }

            int totalItems = allFiltered.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = allFiltered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var buoiHocMap = await _uow.BuoiHocs.Query()
                .Where(b => allLopIds.Contains(b.MaLop) && b.TrangThai != "Đã hủy")
                .GroupBy(b => b.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            return new LopHocSearchResultVM
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                SiSoMap = siSoMap,
                BuoiHocMap = buoiHocMap
            };
        }

        public async Task<LopHocKpiVM> GetKpiAsync()
        {
            int tongSoLop = await _uow.LopHocs.CountAsync();
            int lopDangMo = await _uow.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
            int tongHocVienDangHoc = await _uow.DangKyHocs.CountAsync(d => d.TrangThai == "Đang học" || d.TrangThai == "Đã đóng học phí");

            return new LopHocKpiVM
            {
                TongSoLop = tongSoLop,
                LopDangMo = lopDangMo,
                TongHocVienDangHoc = tongHocVienDangHoc
            };
        }

        public async Task<LopHoc?> GetByIdAsync(int id)
        {
            return await _uow.LopHocs.GetByIdAsync(id);
        }

        public async Task<LopHoc?> GetDetailsAsync(int id)
        {
            return await _uow.LopHocs.GetDetailsWithProgramAsync(id);
        }

        public async Task<LopHocDetailsVM?> GetClassDetailsAsync(int id)
        {
            var lop = await _uow.LopHocs.GetDetailsWithProgramAsync(id);
            if (lop == null) return null;

            var buoiHocs = await _uow.BuoiHocs.Query()
                .Where(b => b.MaLop == id)
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();

            var dangKys = await _uow.DangKyHocs.Query()
                .Include(d => d.HocVien)
                .Where(d => d.MaLop == id)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            int soHocVien = dangKys.Count(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");

            return new LopHocDetailsVM
            {
                LopHoc = lop,
                BuoiHocs = buoiHocs,
                DangKys = dangKys,
                SoHocVien = soHocVien
            };
        }

        public async Task<List<LopHoc>> GetAllAsync()
        {
            var list = await _uow.LopHocs.Query()
                .Include(l => l.ChuongTrinhDaoTao)
                .OrderBy(l => l.TenLop)
                .ToListAsync();
            return list;
        }

        public async Task<List<LopHoc>> GetOpenClassesAsync()
        {
            var list = await _uow.LopHocs.GetOpenClassesWithProgramAsync();
            return list.ToList();
        }

        public async Task<(bool Success, string Message, int? NewId)> CreateClassAsync(LopHoc lop)
        {
            if (string.IsNullOrWhiteSpace(lop.TenLop))
            {
                return (false, "Tên lớp học bắt buộc nhập.", null);
            }

            if (lop.SiSoToiDa <= 0)
            {
                return (false, "Sĩ số tối đa phải lớn hơn 0.", null);
            }

            if (lop.SoBuoiHoc <= 0)
            {
                return (false, "Số buổi học phải lớn hơn 0.", null);
            }

            if (lop.HocPhi < 0)
            {
                return (false, "Học phí phải lớn hơn hoặc bằng 0.", null);
            }

            if (lop.HanDangKy < lop.NgayBatDauDangKy)
            {
                return (false, "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu đăng ký.", null);
            }

            if (lop.NgayKhaiGiang < lop.HanDangKy)
            {
                return (false, "Ngày khai giảng phải sau hoặc bằng hạn đăng ký.", null);
            }

            if (string.IsNullOrEmpty(lop.TrangThai)) lop.TrangThai = "Chưa mở";
            if (string.IsNullOrEmpty(lop.TrinhDoDauVao)) lop.TrinhDoDauVao = "Cơ bản";

            await _uow.LopHocs.AddAsync(lop);
            await _uow.CompleteAsync();
            return (true, "Tạo lớp học mới thành công!", lop.MaLop);
        }

        public async Task<(bool Success, string Message)> UpdateClassAsync(LopHoc lop)
        {
            var existing = await _uow.LopHocs.GetByIdAsync(lop.MaLop);
            if (existing == null) return (false, "Không tìm thấy lớp học.");

            existing.TenLop = lop.TenLop;
            existing.MaChuongTrinh = lop.MaChuongTrinh;
            existing.GiangVien = lop.GiangVien;
            existing.TrinhDoDauVao = lop.TrinhDoDauVao;
            existing.NgayBatDauDangKy = lop.NgayBatDauDangKy;
            existing.HanDangKy = lop.HanDangKy;
            existing.NgayKhaiGiang = lop.NgayKhaiGiang;
            existing.SiSoToiDa = lop.SiSoToiDa;
            existing.SoBuoiHoc = lop.SoBuoiHoc;
            existing.HocPhi = lop.HocPhi;
            existing.TrangThai = lop.TrangThai;
            existing.MoTa = lop.MoTa;
            existing.YeuCauHocVien = lop.YeuCauHocVien;

            _uow.LopHocs.Update(existing);
            await _uow.CompleteAsync();
            return (true, "Cập nhật lớp học thành công!");
        }

        public async Task<(bool Success, string Message)> EditClassAsync(
            int maLop,
            string tenLop,
            int maChuongTrinh,
            string? giangVien,
            string? trinhDoDauVao,
            int siSoToiDa,
            int soBuoiHoc,
            double hocPhi,
            DateTime ngayBatDauDangKy,
            DateTime hanDangKy,
            DateTime ngayKhaiGiang,
            string? moTa,
            string? yeuCauHocVien,
            string trangThai)
        {
            var lop = await _uow.LopHocs.GetByIdAsync(maLop);
            if (lop == null) return (false, "Không tìm thấy lớp học cần cập nhật.");

            int soHocVienDaDuyet = await _uow.DangKyHocs.CountAsync(d =>
                d.MaLop == maLop && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            if (siSoToiDa < soHocVienDaDuyet)
            {
                return (false, $"Không thể giảm sĩ số tối đa xuống {siSoToiDa} vì lớp đang có {soHocVienDaDuyet} học viên đã được duyệt/đóng học phí.");
            }

            if (hanDangKy < ngayBatDauDangKy)
            {
                return (false, "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu đăng ký.");
            }

            if (ngayKhaiGiang < hanDangKy)
            {
                return (false, "Ngày khai giảng phải sau hoặc bằng hạn đăng ký.");
            }

            lop.TenLop = tenLop.Trim();
            lop.MaChuongTrinh = maChuongTrinh;
            lop.GiangVien = giangVien ?? "Chưa xếp";
            lop.TrinhDoDauVao = trinhDoDauVao ?? "Cơ bản";
            lop.SiSoToiDa = siSoToiDa;
            lop.SoBuoiHoc = soBuoiHoc;
            lop.HocPhi = hocPhi;
            lop.NgayBatDauDangKy = ngayBatDauDangKy;
            lop.HanDangKy = hanDangKy;
            lop.NgayKhaiGiang = ngayKhaiGiang;
            lop.MoTa = moTa ?? "";
            lop.YeuCauHocVien = yeuCauHocVien ?? "";
            lop.TrangThai = trangThai;

            _uow.LopHocs.Update(lop);
            await _uow.CompleteAsync();

            return (true, "Cập nhật thông tin lớp học thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteClassAsync(int id)
        {
            var lop = await _uow.LopHocs.GetByIdAsync(id);
            if (lop == null) return (false, "Không tìm thấy lớp học.");

            int soDangKy = await _uow.DangKyHocs.CountAsync(d => d.MaLop == id);
            if (soDangKy > 0)
            {
                return (false, $"Không thể xóa lớp '{lop.TenLop}' vì đã có {soDangKy} hồ sơ đăng ký của học viên liên quan. Bạn có thể chuyển trạng thái lớp sang 'Đã đóng đăng ký'.");
            }

            _uow.LopHocs.Delete(lop);
            await _uow.CompleteAsync();

            return (true, "Xóa lớp học thành công!");
        }
    }
}
