// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Dashboard và các truy vấn Thống kê bằng LINQ (Module 5)

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly IThongKeService _thongKeService;

        public ThongKeController(IThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
        }

        public async Task<IActionResult> Dashboard(int page = 1, string? status = "all", string? search = "")
        {
            var vm = await _thongKeService.GetDashboardDataAsync(page, status, search);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DuyetNhanh(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _thongKeService.QuickApproveRegistrationAsync(id);

            if (isAjax)
            {
                return Json(new { success = result.Success, message = result.Message });
            }

            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Index()
        {
            var report = await _thongKeService.GetStatisticsReportAsync();

            ViewBag.TongDoanhThu = report.TongDoanhThu;
            ViewBag.AvgLapDay = report.AvgLapDay;
            ViewBag.TongChoHoc = report.TongChoHoc;
            ViewBag.ChoDaDangKy = report.ChoDaDangKy;
            ViewBag.TongBuoiHoc = report.TongBuoiHoc;
            ViewBag.DangKyChoDuyet = report.DangKyChoDuyet;
            ViewBag.TyLeHoanThanh = report.TyLeHoanThanh;

            ViewBag.DoanhThuTheoThang = report.DoanhThuTheoThang;
            ViewBag.LopTheoCTDT = report.LopTheoCTDT;
            ViewBag.DKTheoLop = report.DKTheoLop;
            ViewBag.DKTheoTrangThai = report.DKTheoTrangThai;
            ViewBag.LopNhieuDKNhat = report.LopNhieuDKNhat;
            ViewBag.DoanhThuTheoCTDT = report.DoanhThuTheoCTDT;
            ViewBag.DKTheoThang = report.DKTheoThang;
            ViewBag.BuoiHocTheoThang = report.BuoiHocTheoThang;
            ViewBag.DiemTBTheoLop = report.DiemTBTheoLop;
            ViewBag.TyLeLapDay = report.TyLeLapDay;
            ViewBag.TopGiangVien = report.TopGiangVien;

            return View();
        }
    }
}
