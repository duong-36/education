// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Module 5 - Quản lý Kết quả học tập

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class KetQuaHocTapController : Controller
    {
        private readonly IKetQuaHocTapService _ketQuaService;
        private readonly ILopHocService _lopHocService;
        private readonly IDangKyHocService _dangKyHocService;

        public KetQuaHocTapController(
            IKetQuaHocTapService ketQuaService,
            ILopHocService lopHocService,
            IDangKyHocService dangKyHocService)
        {
            _ketQuaService = ketQuaService;
            _lopHocService = lopHocService;
            _dangKyHocService = dangKyHocService;
        }

        public async Task<IActionResult> Index(int? maLop, string? ketQua, string? search, int page = 1)
        {
            int pageSize = 8;
            var result = await _ketQuaService.GetFilteredResultsAsync(maLop, ketQua, search, page, pageSize);

            ViewBag.TotalGraded = result.TotalGraded;
            ViewBag.PassCount = result.PassCount;
            ViewBag.FailCount = result.FailCount;
            ViewBag.AverageScore = result.AverageScore;
            ViewBag.PassRate = result.PassRate;

            // Phổ điểm
            ViewBag.CountA = result.CountA;
            ViewBag.CountB = result.CountB;
            ViewBag.CountC = result.CountC;
            ViewBag.CountD = result.CountD;
            ViewBag.CountF = result.CountF;

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalRecords = result.TotalRecords;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedKetQua = ketQua;
            ViewBag.Search = search;

            ViewBag.LopHocList = await _lopHocService.GetAllAsync();
            ViewBag.EligibleDangKys = await _dangKyHocService.GetEligibleForGradingAsync();

            return View(result.Items);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.LopHocList = await _lopHocService.GetAllAsync();
            ViewBag.EligibleDangKys = await _dangKyHocService.GetEligibleForGradingAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KetQuaHocTap kq)
        {
            ModelState.Remove("DangKyHoc");
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var result = await _ketQuaService.CreateResultAsync(kq);
            if (!result.Success)
            {
                if (isAjax) return Json(new { success = false, message = result.Message });
                TempData["Error"] = result.Message;
                ViewBag.LopHocList = await _lopHocService.GetAllAsync();
                ViewBag.EligibleDangKys = await _dangKyHocService.GetEligibleForGradingAsync();
                return View(kq);
            }

            if (isAjax) return Json(new { success = true, message = result.Message });
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int maKetQua, double diemChuyenCan, double diemCuoiKhoa, string? nhanXet, string? ketQua)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _ketQuaService.UpdateResultAsync(maKetQua, diemChuyenCan, diemCuoiKhoa, nhanXet, ketQua);

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
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _ketQuaService.DeleteResultAsync(id);

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
