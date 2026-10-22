// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Nội dung thực hiện: Module 1 - Quản lý Chương trình đào tạo (CTĐT)

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class ChuongTrinhDaoTaoController : Controller
    {
        private readonly IChuongTrinhDaoTaoService _chuongTrinhService;

        public ChuongTrinhDaoTaoController(IChuongTrinhDaoTaoService chuongTrinhService)
        {
            _chuongTrinhService = chuongTrinhService;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "", string trangThai = "")
        {
            int pageSize = 8;
            var result = await _chuongTrinhService.GetFilteredProgramsAsync(page, pageSize, search, trangThai);

            ViewBag.ClassCounts = result.ClassCounts;
            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.Search = search;
            ViewBag.SelectedTrangThai = trangThai;

            // 3 thẻ KPI
            ViewBag.TongCTDT = result.TongCTDT;
            ViewBag.DangHoatDong = result.DangHoatDong;
            ViewBag.TongLopLienKet = result.TongLopLienKet;

            return View(result.Items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(ChuongTrinhDaoTao ctdt)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _chuongTrinhService.CreateProgramAsync(ctdt);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int maChuongTrinh, string tenChuongTrinh, string? moTa, string? emailLienHe, string trangThai)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _chuongTrinhService.UpdateProgramAsync(maChuongTrinh, tenChuongTrinh, moTa, emailLienHe, trangThai);

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

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _chuongTrinhService.DeleteProgramAsync(id);

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
