// Họ và tên: Nguyễn Đức Linh
// Mã sinh viên: 23103100042
// Nội dung thực hiện: Module 3 - Quản lý Học viên

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class HocVienController : Controller
    {
        private readonly IHocVienService _hocVienService;

        public HocVienController(IHocVienService hocVienService)
        {
            _hocVienService = hocVienService;
        }

        public async Task<IActionResult> Index(
            string search,
            string trangThai,
            string trinhDo,
            int page = 1,
            int pageSize = 10)
        {
            var result = await _hocVienService.GetFilteredStudentsAsync(search, trangThai, trinhDo, page, pageSize);
            var kpi = await _hocVienService.GetKpiAsync();

            ViewBag.TongSoHocVien = kpi.TongSoHocVien;
            ViewBag.DangHoc = kpi.DangHoc;
            ViewBag.TotNghiep = kpi.TotNghiep;

            ViewBag.TrinhDoList = result.TrinhDoList;
            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.PageSize = result.PageSize;
            ViewBag.Search = search;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.SelectedTrinhDo = trinhDo;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_HocVienTablePartial", result.Items);
            }

            return View(result.Items);
        }

        // --- CHI TIẾT HỌC VIÊN ---
        public async Task<IActionResult> Details(int id)
        {
            var details = await _hocVienService.GetDetailsWithRegistrationsAsync(id);
            if (details == null) return NotFound();

            ViewBag.DangKys = details.DangKys;
            ViewBag.KetQuas = details.KetQuas;

            return View(details.HocVien);
        }

        // --- GET HỌC VIÊN CHO DRAWER EDIT (AJAX) ---
        [HttpGet]
        public async Task<IActionResult> GetHocVien(int id)
        {
            var hv = await _hocVienService.GetByIdAsync(id);
            if (hv == null) return Json(new { success = false, message = "Không tìm thấy học viên." });

            return Json(new
            {
                success = true,
                data = new
                {
                    maHocVien = hv.MaHocVien,
                    hoTen = hv.HoTen,
                    ngaySinh = hv.NgaySinh.ToString("yyyy-MM-dd"),
                    gioiTinh = hv.GioiTinh,
                    soDienThoai = hv.SoDienThoai,
                    email = hv.Email,
                    diaChi = hv.DiaChi,
                    trinhDoHocVan = hv.TrinhDoHocVan,
                    ngheNghiep = hv.NgheNghiep,
                    trangThai = hv.TrangThai
                }
            });
        }

        // --- CREATE HỌC VIÊN (AJAX DRAWER) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(HocVien model, string tenDangNhap, string matKhau)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _hocVienService.CreateStudentAsync(model, tenDangNhap, matKhau);

            if (!result.Success)
            {
                if (isAjax) return Json(new { success = false, message = result.Message });
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            if (isAjax) return Json(new { success = true, message = result.Message });
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // --- EDIT HỌC VIÊN (AJAX DRAWER) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(HocVien model)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _hocVienService.UpdateStudentAsync(model);

            if (!result.Success)
            {
                if (isAjax) return Json(new { success = false, message = result.Message });
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            if (isAjax) return Json(new { success = true, message = result.Message });
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // --- DELETE HỌC VIÊN (RÀNG BUỘC TOÀN VẸN NGHIỆP VỤ) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _hocVienService.DeleteStudentAsync(id);

            if (!result.Success)
            {
                if (isAjax) return Json(new { success = false, message = result.Message });
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            if (isAjax) return Json(new { success = true, message = result.Message });
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
