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
    public class KetQuaHocTapService : IKetQuaHocTapService
    {
        private readonly IUnitOfWork _uow;

        public KetQuaHocTapService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public (double DiemTongKet, string KetQua, string XepLoai) CalculateGrade(double diemChuyenCan, double diemCuoiKhoa)
        {
            double dtb = Math.Round((diemChuyenCan * 0.3) + (diemCuoiKhoa * 0.7), 2);
            string ketQua = dtb >= 5.0 ? "Hoàn thành" : "Không hoàn thành";
            string xepLoai = dtb switch
            {
                >= 8.5 => "Xuất sắc",
                >= 7.0 => "Giỏi",
                >= 5.5 => "Khá",
                >= 4.0 => "Trung bình",
                _ => "Yếu"
            };
            return (dtb, ketQua, xepLoai);
        }

        public async Task<KetQuaHocTapSearchResultVM> GetFilteredResultsAsync(
            int? maLop = null,
            string? ketQua = null,
            string? search = null,
            int page = 1,
            int pageSize = 8)
        {
            var query = _uow.KetQuaHocTaps.Query()
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.HocVien)
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                .AsQueryable();

            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(k => k.DangKyHoc.MaLop == maLop.Value);
            }

            if (!string.IsNullOrWhiteSpace(ketQua))
            {
                query = query.Where(k => k.KetQua == ketQua);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(k => k.DangKyHoc.HocVien.HoTen.ToLower().Contains(s) || 
                                         k.DangKyHoc.HocVien.MaHocVien.ToString().Contains(s) ||
                                         k.DangKyHoc.LopHoc.TenLop.ToLower().Contains(s));
            }

            var allScores = await _uow.KetQuaHocTaps.Query().Select(k => new { k.DiemTongKet, k.KetQua }).ToListAsync();
            int totalGraded = allScores.Count;
            int passCount = allScores.Count(k => k.KetQua == "Hoàn thành");
            int failCount = allScores.Count(k => k.KetQua == "Không hoàn thành");
            double avgScore = allScores.Any() ? Math.Round(allScores.Average(k => k.DiemTongKet), 2) : 0;
            double passRate = totalGraded > 0 ? Math.Round((double)passCount / totalGraded * 100, 1) : 0;

            int countA = allScores.Count(d => d.DiemTongKet >= 8.5);
            int countB = allScores.Count(d => d.DiemTongKet >= 7.0 && d.DiemTongKet < 8.5);
            int countC = allScores.Count(d => d.DiemTongKet >= 5.5 && d.DiemTongKet < 7.0);
            int countD = allScores.Count(d => d.DiemTongKet >= 4.0 && d.DiemTongKet < 5.5);
            int countF = allScores.Count(d => d.DiemTongKet < 4.0);

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var items = await query
                .OrderByDescending(k => k.NgayCapNhat)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new KetQuaHocTapSearchResultVM
            {
                Items = items,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = page,
                TotalGraded = totalGraded,
                PassCount = passCount,
                FailCount = failCount,
                AverageScore = avgScore,
                PassRate = passRate,
                CountA = countA,
                CountB = countB,
                CountC = countC,
                CountD = countD,
                CountF = countF
            };
        }

        public async Task<KetQuaHocTap?> GetByIdAsync(int id)
        {
            return await _uow.KetQuaHocTaps.Query()
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.HocVien)
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                .FirstOrDefaultAsync(k => k.MaKetQua == id);
        }

        public async Task<List<KetQuaHocTap>> GetByHocVienAsync(int maHocVien)
        {
            var list = await _uow.KetQuaHocTaps.Query()
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                        .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == maHocVien)
                .ToListAsync();

            return list;
        }

        public async Task<(bool Success, string Message, int? NewId)> CreateResultAsync(KetQuaHocTap kq)
        {
            var dangKy = await _uow.DangKyHocs.GetByIdAsync(kq.MaDangKy);
            if (dangKy == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký hợp lệ.", null);
            }

            bool daCoDiem = await _uow.KetQuaHocTaps.ExistsAsync(k => k.MaDangKy == kq.MaDangKy);
            if (daCoDiem)
            {
                return (false, "Học viên này đã được ghi nhận kết quả học tập.", null);
            }

            if (kq.DiemChuyenCan < 0 || kq.DiemChuyenCan > 10 || kq.DiemCuoiKhoa < 0 || kq.DiemCuoiKhoa > 10)
            {
                return (false, "Điểm chuyên cần và cuối khóa phải nằm trong khoảng từ 0 đến 10.", null);
            }

            var (dtb, ketQua, _) = CalculateGrade(kq.DiemChuyenCan, kq.DiemCuoiKhoa);
            kq.DiemTongKet = dtb;
            if (string.IsNullOrEmpty(kq.KetQua))
            {
                kq.KetQua = ketQua;
            }
            kq.NgayCapNhat = DateTime.Now;

            await _uow.KetQuaHocTaps.AddAsync(kq);

            dangKy.TrangThai = kq.KetQua == "Hoàn thành" ? "Hoàn thành" : "Không hoàn thành";
            _uow.DangKyHocs.Update(dangKy);

            await _uow.CompleteAsync();

            return (true, "Ghi nhận kết quả học tập thành công!", kq.MaKetQua);
        }

        public async Task<(bool Success, string Message)> UpdateResultAsync(int maKetQua, double diemChuyenCan, double diemCuoiKhoa, string? nhanXet, string? ketQua)
        {
            var kq = await _uow.KetQuaHocTaps.Query()
                .Include(k => k.DangKyHoc)
                .FirstOrDefaultAsync(k => k.MaKetQua == maKetQua);

            if (kq == null) return (false, "Không tìm thấy kết quả.");

            if (diemChuyenCan < 0 || diemChuyenCan > 10 || diemCuoiKhoa < 0 || diemCuoiKhoa > 10)
            {
                return (false, "Điểm phải nằm trong thang điểm 0 - 10.");
            }

            kq.DiemChuyenCan = diemChuyenCan;
            kq.DiemCuoiKhoa = diemCuoiKhoa;
            var (dtb, defaultKetQua, _) = CalculateGrade(diemChuyenCan, diemCuoiKhoa);
            kq.DiemTongKet = dtb;
            kq.KetQua = string.IsNullOrEmpty(ketQua) ? defaultKetQua : ketQua;
            kq.NhanXet = nhanXet;
            kq.NgayCapNhat = DateTime.Now;

            if (kq.DangKyHoc != null)
            {
                kq.DangKyHoc.TrangThai = kq.KetQua == "Hoàn thành" ? "Hoàn thành" : "Không hoàn thành";
                _uow.DangKyHocs.Update(kq.DangKyHoc);
            }

            _uow.KetQuaHocTaps.Update(kq);
            await _uow.CompleteAsync();

            return (true, "Cập nhật kết quả thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteResultAsync(int id)
        {
            var kq = await _uow.KetQuaHocTaps.GetByIdAsync(id);
            if (kq == null) return (false, "Không tìm thấy kết quả.");

            _uow.KetQuaHocTaps.Delete(kq);
            await _uow.CompleteAsync();

            return (true, "Đã xóa kết quả học tập.");
        }
    }
}
