// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Dashboard và các truy vấn Thống kê bằng LINQ (Module 5)

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard(int page = 1)
        {
            int pageSize = 5;
            int totalActivities = await _context.DangKyHocs.CountAsync();
            int totalPages = (int)Math.Ceiling(totalActivities / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var recentActivities = await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .OrderByDescending(d => d.NgayDangKy)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var danhSachBuoiHoc = await _context.BuoiHocs
                .Include(b => b.LopHoc)
                .OrderBy(b => b.ThoiGianBatDau)
                .Take(5)
                .ToListAsync();

            int totalCoDiem = await _context.KetQuaHocTaps.CountAsync();
            int soHoanThanh = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Hoàn thành");
            double tyLeHT = totalCoDiem > 0 ? Math.Round((double)soHoanThanh / totalCoDiem * 100, 1) : 0;

            var vm = new DashboardVM
            {
                TongSoCTDT = await _context.ChuongTrinhDaoTaos.CountAsync(),
                TongSoLop = await _context.LopHocs.CountAsync(),
                LopDangMo = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký"),
                TongSoHocVien = await _context.HocViens.CountAsync(),
                TongSoDangKy = await _context.DangKyHocs.CountAsync(),
                DangKyChoDuyet = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt"),
                DangKyDaDuyet = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Đã duyệt"),
                BuoiHocSapToi = await _context.BuoiHocs.CountAsync(b => b.ThoiGianBatDau > DateTime.Now && b.TrangThai == "Đã lên lịch"),
                HocVienHoanThanh = soHoanThanh,
                TyLeHoanThanh = tyLeHT,
                DanhSachBuoiHoc = danhSachBuoiHoc,
                HoatDongGanDay = recentActivities,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DuyetNhanh(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var dangKy = await _context.DangKyHocs.FindAsync(id);
            if (dangKy != null)
            {
                dangKy.TrangThai = "Đã duyệt";
                dangKy.NgayXuLy = DateTime.Now;
                await _context.SaveChangesAsync();
                if (isAjax)
                {
                    return Json(new { success = true, message = "Đã phê duyệt đăng ký thành công!" });
                }
            }
            else
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });
                }
            }
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Index()
        {
            // 1. Số lớp học theo chương trình đào tạo
            ViewBag.LopTheoCTDT = await _context.LopHocs
                .GroupBy(l => l.ChuongTrinhDaoTao.TenChuongTrinh)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // 2. Số đăng ký theo từng lớp
            ViewBag.DKTheoLop = await _context.DangKyHocs
                .GroupBy(d => d.LopHoc.TenLop)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // 3. Số đăng ký theo trạng thái
            ViewBag.DKTheoTrangThai = await _context.DangKyHocs
                .GroupBy(d => d.TrangThai)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();

            // 4. Lớp có nhiều đăng ký nhất
            ViewBag.LopNhieuDKNhat = await _context.DangKyHocs
                .GroupBy(d => d.LopHoc.TenLop)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() })
                .FirstOrDefaultAsync();

            // 5. Doanh thu theo tháng
            var doanhThu = await _context.DangKyHocs
                .Where(d => d.NgayDongHocPhi != null)
                .GroupBy(d => new { d.NgayDongHocPhi.Value.Year, d.NgayDongHocPhi.Value.Month })
                .Select(g => new ThongKeDoanhThuVM
                {
                    Thang = g.Key.Month + "/" + g.Key.Year,
                    TongDoanhThu = g.Sum(d => d.SoTienDaDong)
                })
                .OrderBy(r => r.Thang)
                .ToListAsync();
            ViewBag.DoanhThuTheoThang = doanhThu;

            // 6. Doanh thu theo chương trình đào tạo
            ViewBag.DoanhThuTheoCTDT = await _context.DangKyHocs
                .Where(d => d.NgayDongHocPhi != null)
                .GroupBy(d => d.LopHoc.ChuongTrinhDaoTao.TenChuongTrinh)
                .Select(g => new { Ten = g.Key, DoanhThu = g.Sum(d => d.SoTienDaDong) })
                .ToListAsync();

            // 7. Số đăng ký theo tháng
            ViewBag.DKTheoThang = await _context.DangKyHocs
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => new { Thang = g.Key.Month + "/" + g.Key.Year, SoLuong = g.Count() })
                .ToListAsync();

            // 8. Số buổi học theo tháng
            ViewBag.BuoiHocTheoThang = await _context.BuoiHocs
                .GroupBy(b => new { b.ThoiGianBatDau.Year, b.ThoiGianBatDau.Month })
                .Select(g => new { Thang = g.Key.Month + "/" + g.Key.Year, SoLuong = g.Count() })
                .ToListAsync();

            // 9. Điểm tổng kết trung bình theo lớp
            ViewBag.DiemTBTheoLop = await _context.KetQuaHocTaps
                .GroupBy(k => k.DangKyHoc.LopHoc.TenLop)
                .Select(g => new { Ten = g.Key, DiemTB = g.Average(k => k.DiemTongKet) })
                .ToListAsync();

            // 10. Tỷ lệ lấp đầy sĩ số
            var tyLe = await _context.LopHocs
                .Select(l => new TyLeLapDayVM
                {
                    TenLop = l.TenLop,
                    SiSoToiDa = l.SiSoToiDa,
                    SiSoHienTai = _context.DangKyHocs.Count(dk => dk.MaLop == l.MaLop && 
                                  (dk.TrangThai == "Đã duyệt" || dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học")),
                    TyLe = l.SiSoToiDa > 0 ? 
                           (double)_context.DangKyHocs.Count(dk => dk.MaLop == l.MaLop && 
                           (dk.TrangThai == "Đã duyệt" || dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học")) / l.SiSoToiDa * 100 
                           : 0
                })
                .ToListAsync();
            ViewBag.TyLeLapDay = tyLe;

            // 11. Tỷ lệ Hoàn thành trên số học viên đã có kết quả
            var totalCoDiem = await _context.KetQuaHocTaps.CountAsync();
            ViewBag.TyLeHoanThanh = totalCoDiem > 0 ? (double)(await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Hoàn thành")) / totalCoDiem * 100 : 0;

            // 12. Thống kê cấp cao phục vụ Dashboard & KPI
            ViewBag.TongDoanhThu = await _context.DangKyHocs.Where(d => d.NgayDongHocPhi != null).SumAsync(d => (double?)d.SoTienDaDong) ?? 0;
            ViewBag.TongBuoiHoc = await _context.BuoiHocs.CountAsync();
            ViewBag.DangKyChoDuyet = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt");
            ViewBag.AvgLapDay = tyLe.Any() ? Math.Round(tyLe.Average(t => t.TyLe), 1) : 0;
            ViewBag.TongChoHoc = await _context.LopHocs.SumAsync(l => (int?)l.SiSoToiDa) ?? 0;
            ViewBag.ChoDaDangKy = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");

            // 13. Bảng xếp hạng giảng viên phụ trách
            var lopList = await _context.LopHocs.ToListAsync();
            var dkList = await _context.DangKyHocs.ToListAsync();
            var kqList = await _context.KetQuaHocTaps.Include(k => k.DangKyHoc).ToListAsync();

            ViewBag.TopGiangVien = lopList
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
                        DiemTB = scores.Any() ? Math.Round(scores.Average(), 1) : 0,
                        DanhGia = scores.Any() ? Math.Round(Math.Min(5.0, 3.5 + (scores.Average() / 10.0) * 1.5), 1) : 4.5
                    };
                })
                .OrderByDescending(x => x.SoSV)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
