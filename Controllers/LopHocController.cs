// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Nội dung thực hiện: Module 2 - Quản lý Lớp học, Tìm kiếm, Lọc đa tiêu chí, Sắp xếp, Phân trang LINQ

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class LopHocController : Controller
    {
        private readonly ILopHocService _lopHocService;
        private readonly IChuongTrinhDaoTaoService _chuongTrinhService;

        public LopHocController(ILopHocService lopHocService, IChuongTrinhDaoTaoService chuongTrinhService)
        {
            _lopHocService = lopHocService;
            _chuongTrinhService = chuongTrinhService;
        }

        // --- DANH SÁCH LỚP HỌC (SEARCH + FILTER + SORT + PAGINATION) ---
        public async Task<IActionResult> Index(
            int page = 1,
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
            int pageSize = 6;
            var result = await _lopHocService.GetFilteredClassesAsync(
                page, pageSize, search, maChuongTrinh, trinhDo, trangThai,
                hocPhiMin, hocPhiMax, hanDangKyStatus, choTrongStatus, sortBy);

            var kpi = await _lopHocService.GetKpiAsync();

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedTrinhDo = trinhDo;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.HocPhiMin = hocPhiMin;
            ViewBag.HocPhiMax = hocPhiMax;
            ViewBag.HanDangKyStatus = hanDangKyStatus;
            ViewBag.ChoTrongStatus = choTrongStatus;
            ViewBag.SortBy = sortBy;

            ViewBag.SiSoMap = result.SiSoMap;
            ViewBag.BuoiHocMap = result.BuoiHocMap;
            ViewBag.ChuongTrinhList = await _chuongTrinhService.GetAllAsync();

            // 3 thẻ KPI
            ViewBag.TongSoLop = kpi.TongSoLop;
            ViewBag.LopDangMo = kpi.LopDangMo;
            ViewBag.TongHocVienDangHoc = kpi.TongHocVienDangHoc;

            return View(result.Items);
        }

        // --- CHI TIẾT LỚP HỌC ---
        public async Task<IActionResult> Details(int id)
        {
            var details = await _lopHocService.GetClassDetailsAsync(id);
            if (details == null) return NotFound();

            ViewBag.BuoiHocs = details.BuoiHocs;
            ViewBag.DangKys = details.DangKys;
            ViewBag.SoHocVien = details.SoHocVien;

            return View(details.LopHoc);
        }

        // --- CREATE LỚP HỌC (DRAWER AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(LopHoc lop)
        {
            ModelState.Remove("ChuongTrinhDaoTao");
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var result = await _lopHocService.CreateClassAsync(lop);
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

        // --- EDIT LỚP HỌC (DRAWER AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(
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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var result = await _lopHocService.EditClassAsync(
                maLop, tenLop, maChuongTrinh, giangVien, trinhDoDauVao, siSoToiDa, soBuoiHoc,
                hocPhi, ngayBatDauDangKy, hanDangKy, ngayKhaiGiang, moTa, yeuCauHocVien, trangThai);

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

        // --- XÓA LỚP HỌC (AN TOÀN KIỂM SOÁT DỮ LIỆU) ---
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _lopHocService.DeleteClassAsync(id);

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
