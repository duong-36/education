// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Module 5 - Quản lý Buổi học & Lịch học

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class BuoiHocController : Controller
    {
        private readonly IBuoiHocService _buoiHocService;
        private readonly ILopHocService _lopHocService;

        public BuoiHocController(IBuoiHocService buoiHocService, ILopHocService lopHocService)
        {
            _buoiHocService = buoiHocService;
            _lopHocService = lopHocService;
        }

        public async Task<IActionResult> Index(int? maLop, string? phong, string? trangThai, string? search, int page = 1)
        {
            int pageSize = 8;
            var result = await _buoiHocService.GetFilteredSessionsAsync(maLop, phong, trangThai, search, page, pageSize);

            ViewBag.ConflictIds = result.ConflictIds;
            ViewBag.SoLoiTrung = result.ConflictIds.Count;

            ViewBag.TotalSessions = result.TotalSessions;
            ViewBag.CompletedSessions = result.CompletedSessions;
            ViewBag.ScheduledSessions = result.ScheduledSessions;
            ViewBag.CanceledSessions = result.CanceledSessions;

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalRecords = result.TotalRecords;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedPhong = phong;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.Search = search;

            ViewBag.LopHocList = await _lopHocService.GetAllAsync();
            ViewBag.AllBuoiHocs = result.AllBuoiHocs;
            ViewBag.SoThi = result.SoThi;
            ViewBag.SoChinhKhoa = result.SoChinhKhoa;
            ViewBag.DanhSachPhong = result.DanhSachPhong;
            ViewBag.DanhSachGiangVien = result.DanhSachGiangVien;

            return View(result.Items);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.LopHocList = await _lopHocService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuoiHoc buoiHoc)
        {
            ModelState.Remove("LopHoc");
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var result = await _buoiHocService.CreateSessionAsync(buoiHoc);
            if (!result.Success)
            {
                if (isAjax) return Json(new { success = false, message = result.Message });
                TempData["Error"] = result.Message;
                ViewBag.LopHocList = await _lopHocService.GetAllAsync();
                return View(buoiHoc);
            }

            if (isAjax) return Json(new { success = true, message = result.Message });
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(BuoiHoc buoiHoc)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _buoiHocService.UpdateSessionAsync(buoiHoc);

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
            var result = await _buoiHocService.DeleteSessionAsync(id);

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
        public async Task<IActionResult> HoanThanh(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _buoiHocService.CompleteSessionAsync(id);

            if (isAjax) return Json(new { success = result.Success, message = result.Message });
            if (result.Success) TempData["Success"] = result.Message;
            else TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _buoiHocService.CancelSessionAsync(id);

            if (isAjax) return Json(new { success = result.Success, message = result.Message });
            if (result.Success) TempData["Success"] = result.Message;
            else TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
