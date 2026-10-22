// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Tiếp nhận, Xét duyệt đăng ký & Thu học phí (Module 4) - Phía Giáo vụ & Admin

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class DangKyHocController : Controller
    {
        private readonly IDangKyHocService _dangKyHocService;
        private readonly IChuongTrinhDaoTaoService _chuongTrinhService;
        private readonly ILopHocService _lopHocService;

        public DangKyHocController(
            IDangKyHocService dangKyHocService,
            IChuongTrinhDaoTaoService chuongTrinhService,
            ILopHocService lopHocService)
        {
            _dangKyHocService = dangKyHocService;
            _chuongTrinhService = chuongTrinhService;
            _lopHocService = lopHocService;
        }

        // --- DANH SÁCH HỒ SƠ ĐĂNG KÝ (LINQ MULTI-FILTER, SEARCH, PAGINATION, AJAX) ---
        public async Task<IActionResult> Index(
            string search,
            int? maChuongTrinh,
            int? maLop,
            string trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string sortBy,
            int page = 1,
            int pageSize = 10)
        {
            var result = await _dangKyHocService.GetFilteredRegistrationsAsync(
                search, maChuongTrinh, maLop, trangThai, tuNgay, denNgay, sortBy, page, pageSize);

            var kpi = await _dangKyHocService.GetKpiAsync();

            ViewBag.ChoDuyetCount = kpi.ChoDuyetCount;
            ViewBag.DaDuyetCount = kpi.DaDuyetCount;
            ViewBag.TongDoanhThu = kpi.TongDoanhThu;

            ViewBag.ChuongTrinhList = await _chuongTrinhService.GetAllAsync();
            var allClasses = await _lopHocService.GetAllAsync();
            ViewBag.LopHocList = allClasses
                .Select(l => new { l.MaLop, l.TenLop, l.MaChuongTrinh })
                .ToList();

            ViewBag.CurrentPage = result.CurrentPage;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.TotalItems = result.TotalItems;
            ViewBag.PageSize = result.PageSize;
            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
            ViewBag.SortBy = sortBy;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_DangKyHocTablePartial", result.Items);
            }

            return View(result.Items);
        }

        // --- LẤY THÔNG TIN ĐƠN ĐĂNG KÝ CHO MODAL THU HỌC PHÍ (AJAX) ---
        [HttpGet]
        public async Task<IActionResult> GetDangKy(int id)
        {
            var dk = await _dangKyHocService.GetDetailsAsync(id);
            if (dk == null)
            {
                return Json(new { success = false, message = "Không tìm thấy hồ sơ." });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    maDangKy = dk.MaDangKy,
                    hoTen = dk.HocVien?.HoTen,
                    email = dk.HocVien?.Email,
                    soDienThoai = dk.HocVien?.SoDienThoai,
                    tenLop = dk.LopHoc?.TenLop,
                    hocPhi = dk.LopHoc?.HocPhi ?? 0,
                    soTienDaDong = dk.SoTienDaDong,
                    trangThai = dk.TrangThai
                }
            });
        }

        // --- PHÊ DUYỆT ĐĂNG KÝ (RÀNG BUỘC SĨ SỐ LINQ) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DuyetDangKyPost(int id, string nhanXet)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _dangKyHocService.ApproveRegistrationAsync(id, nhanXet);

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

        // --- TỪ CHỐI ĐĂNG KÝ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoiDangKyPost(int id, string lyDo)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _dangKyHocService.RejectRegistrationAsync(id, lyDo);

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

        // --- GHI NHẬN ĐÓNG HỌC PHÍ ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThuHocPhiPost(int id, double soTien, string ghiChu)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _dangKyHocService.ProcessPaymentAsync(id, soTien, ghiChu);

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

        // --- CHUYỂN TRẠNG THÁI SANG 'ĐANG HỌC' ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatDauHocPost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _dangKyHocService.StartLearningAsync(id);

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

        // --- XÓA ĐƠN ĐĂNG KÝ (RÀNG BUỘC HỌC PHÍ) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _dangKyHocService.DeleteRegistrationAsync(id);

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
