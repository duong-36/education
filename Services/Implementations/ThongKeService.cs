// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Dashboard và các truy vấn Thống kê bằng LINQ (Module 5) - Service Layer

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
    public class ThongKeService : IThongKeService
    {
        private readonly IUnitOfWork _uow;

        public ThongKeService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DashboardVM> GetDashboardDataAsync(int page = 1, string? status = "all", string? search = "")
        {
            int pageSize = 5;
            status = string.IsNullOrWhiteSpace(status) ? "all" : status.Trim();
            search = string.IsNullOrWhiteSpace(search) ? "" : search.Trim();

            var query = _uow.DangKyHocs.Query()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                string s = search.ToLower();
                query = query.Where(d => (d.HocVien != null && (d.HocVien.HoTen.ToLower().Contains(s) || d.HocVien.MaHocVien.ToString().Contains(s)))
                                      || (d.LopHoc != null && d.LopHoc.TenLop.ToLower().Contains(s)));
            }

            if (status == "Chờ duyệt")
            {
                query = query.Where(d => d.TrangThai == "Chờ duyệt");
            }
            else if (status == "Đã duyệt")
            {
                query = query.Where(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");
            }

            int totalActivities = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalActivities / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var recentActivities = await query
                .OrderByDescending(d => d.NgayDangKy)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var danhSachBuoiHoc = await _uow.BuoiHocs.Query()
                .Include(b => b.LopHoc)
                .OrderBy(b => b.ThoiGianBatDau)
                .Take(5)
                .ToListAsync();

            int totalCoDiem = await _uow.KetQuaHocTaps.CountAsync();
            int soHoanThanh = await _uow.KetQuaHocTaps.CountAsync(k => k.KetQua == "Hoàn thành");
            double tyLeHT = totalCoDiem > 0 ? Math.Round((double)soHoanThanh / totalCoDiem * 100, 1) : 0;

            return new DashboardVM
            {
                TongSoCTDT = await _uow.ChuongTrinhDaoTaos.CountAsync(),
                TongSoLop = await _uow.LopHocs.CountAsync(),
                LopDangMo = await _uow.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký"),
                TongSoHocVien = await _uow.HocViens.CountAsync(),
                TongSoDangKy = await _uow.DangKyHocs.CountAsync(),
                DangKyChoDuyet = await _uow.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt"),
                DangKyDaDuyet = await _uow.DangKyHocs.CountAsync(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"),
                BuoiHocSapToi = await _uow.BuoiHocs.CountAsync(b => b.ThoiGianBatDau > DateTime.Now && b.TrangThai == "Đã lên lịch"),
                HocVienHoanThanh = soHoanThanh,
                TyLeHoanThanh = tyLeHT,
                DanhSachBuoiHoc = danhSachBuoiHoc,
                HoatDongGanDay = recentActivities,
                CurrentPage = page,
                TotalPages = totalPages,
                SelectedStatus = status,
                Search = search
            };
        }

        public async Task<(bool Success, string Message)> QuickApproveRegistrationAsync(int id)
        {
            var dangKy = await _uow.DangKyHocs.GetByIdAsync(id);
            if (dangKy == null)
            {
                return (false, "Không tìm thấy hồ sơ đăng ký.");
            }

            dangKy.TrangThai = "Đã duyệt";
            dangKy.NgayXuLy = DateTime.Now;
            _uow.DangKyHocs.Update(dangKy);
            await _uow.CompleteAsync();

            return (true, "Đã phê duyệt đăng ký thành công!");
        }

        public async Task<ThongKeReportVM> GetStatisticsReportAsync()
        {
            // 1. Số lớp học theo chương trình đào tạo
            var lopTheoCTDT = await _uow.LopHocs.Query()
                .GroupBy(l => l.ChuongTrinhDaoTao.TenChuongTrinh)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync<dynamic>();

            // 2. Số đăng ký theo từng lớp
            var dkTheoLop = await _uow.DangKyHocs.Query()
                .GroupBy(d => d.LopHoc.TenLop)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync<dynamic>();

            // 3. Số đăng ký theo trạng thái
            var dkTheoTrangThai = await _uow.DangKyHocs.Query()
                .GroupBy(d => d.TrangThai)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync<dynamic>();

            // 4. Lớp có nhiều đăng ký nhất
            dynamic? lopNhieuDKNhat = await _uow.DangKyHocs.Query()
                .GroupBy(d => d.LopHoc.TenLop)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .FirstOrDefaultAsync();

            // 5. Doanh thu theo tháng
            var doanhThuTheoThang = await _uow.DangKyHocs.Query()
                .Where(d => d.NgayDongHocPhi != null)
                .GroupBy(d => new { d.NgayDongHocPhi.Value.Year, d.NgayDongHocPhi.Value.Month })
                .Select(g => new ThongKeDoanhThuVM
                {
                    Thang = g.Key.Month + "/" + g.Key.Year,
                    TongDoanhThu = g.Sum(d => d.SoTienDaDong)
                })
                .OrderBy(r => r.Thang)
                .ToListAsync();

            // 6. Doanh thu theo chương trình đào tạo
            var doanhThuCTDT = await _uow.DangKyHocs.Query()
                .Where(d => d.NgayDongHocPhi != null)
                .GroupBy(d => d.LopHoc.ChuongTrinhDaoTao.TenChuongTrinh)
                .Select(g => new { Ten = g.Key, DoanhThu = g.Sum(d => d.SoTienDaDong) })
                .ToListAsync<dynamic>();

            // 7. Số đăng ký theo tháng
            var dkTheoThang = await _uow.DangKyHocs.Query()
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => new { Thang = g.Key.Month + "/" + g.Key.Year, SoLuong = g.Count() })
                .ToListAsync<dynamic>();

            // 8. Số buổi học theo tháng
            var buoiHocTheoThang = await _uow.BuoiHocs.Query()
                .GroupBy(b => new { b.ThoiGianBatDau.Year, b.ThoiGianBatDau.Month })
                .Select(g => new { Thang = g.Key.Month + "/" + g.Key.Year, SoLuong = g.Count() })
                .ToListAsync<dynamic>();

            // 9. Điểm tổng kết trung bình theo lớp
            var diemTBTheoLop = await _uow.KetQuaHocTaps.Query()
                .GroupBy(k => k.DangKyHoc.LopHoc.TenLop)
                .Select(g => new { Ten = g.Key, DiemTB = g.Average(k => k.DiemTongKet) })
                .ToListAsync<dynamic>();

            // 10. Tỷ lệ lấp đầy sĩ số
            var tyLe = await _uow.LopHocs.Query()
                .Select(l => new TyLeLapDayVM
                {
                    TenLop = l.TenLop,
                    SiSoToiDa = l.SiSoToiDa,
                    SiSoHienTai = _uow.DangKyHocs.Query().Count(dk => dk.MaLop == l.MaLop && 
                                  (dk.TrangThai == "Đã duyệt" || dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học")),
                    TyLe = l.SiSoToiDa > 0 ? 
                           (double)_uow.DangKyHocs.Query().Count(dk => dk.MaLop == l.MaLop && 
                           (dk.TrangThai == "Đã duyệt" || dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học")) / l.SiSoToiDa * 100 
                           : 0
                })
                .ToListAsync();

            // 11. Tỷ lệ hoàn thành
            var totalCoDiem = await _uow.KetQuaHocTaps.CountAsync();
            double tyLeHoanThanh = totalCoDiem > 0 ? (double)(await _uow.KetQuaHocTaps.CountAsync(k => k.KetQua == "Hoàn thành")) / totalCoDiem * 100 : 0;

            // 12. Tổng hợp chỉ số
            double tongDoanhThu = await _uow.DangKyHocs.Query().Where(d => d.NgayDongHocPhi != null).SumAsync(d => (double?)d.SoTienDaDong) ?? 0;
            int tongBuoiHoc = await _uow.BuoiHocs.CountAsync();
            int dangKyChoDuyet = await _uow.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt");
            double avgLapDay = tyLe.Any() ? Math.Round(tyLe.Average(t => t.TyLe), 1) : 0;
            int tongChoHoc = await _uow.LopHocs.Query().SumAsync(l => (int?)l.SiSoToiDa) ?? 0;
            int choDaDangKy = await _uow.DangKyHocs.CountAsync(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");

            // 13. Xếp hạng giảng viên
            var lopList = await _uow.LopHocs.GetAllAsync();
            var dkList = await _uow.DangKyHocs.GetAllAsync();
            var kqList = await _uow.KetQuaHocTaps.Query().Include(k => k.DangKyHoc).ToListAsync();

            var topGiangVien = lopList
                .Where(l => !string.IsNullOrEmpty(l.GiangVien))
                .GroupBy(l => l.GiangVien)
                .Select(g => {
                    var maLops = g.Select(x => x.MaLop).ToList();
                    var svCount = dkList.Count(d => maLops.Contains(d.MaLop) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));
                    var scores = kqList.Where(k => maLops.Contains(k.DangKyHoc.MaLop)).Select(k => k.DiemTongKet).ToList();
                    return new {
                        GiangVien = g.Key,
                        SoLop = g.Count(),
                        MonHoc = string.Join(", ", g.Select(x => x.TenLop).Take(2)),
                        SoSV = svCount,
                        DiemDanhGia = scores.Any() ? Math.Round(scores.Average(), 1) : 8.5
                    };
                })
                .OrderByDescending(x => x.SoSV)
                .Take(5)
                .ToList<dynamic>();

            return new ThongKeReportVM
            {
                TongDoanhThu = tongDoanhThu,
                AvgLapDay = avgLapDay,
                TongChoHoc = tongChoHoc,
                ChoDaDangKy = choDaDangKy,
                TongBuoiHoc = tongBuoiHoc,
                DangKyChoDuyet = dangKyChoDuyet,
                TyLeHoanThanh = tyLeHoanThanh,
                DoanhThuTheoThang = doanhThuTheoThang,
                LopTheoCTDT = lopTheoCTDT,
                DKTheoLop = dkTheoLop,
                DKTheoTrangThai = dkTheoTrangThai,
                LopNhieuDKNhat = lopNhieuDKNhat,
                DoanhThuTheoCTDT = doanhThuCTDT,
                DKTheoThang = dkTheoThang,
                BuoiHocTheoThang = buoiHocTheoThang,
                DiemTBTheoLop = diemTBTheoLop,
                TyLeLapDay = tyLe,
                TopGiangVien = topGiangVien
            };
        }
    }
}
