// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Nội dung thực hiện: Module 1 - Tài khoản, Đăng nhập, Phân quyền Session

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;

        public TaiKhoanController(ITaiKhoanService taiKhoanService)
        {
            _taiKhoanService = taiKhoanService;
        }

        // --- ĐĂNG KÝ TÀI KHOẢN HỌC VIÊN (PUBLIC) ---
        [HttpGet]
        public IActionResult Register()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.IsNullOrEmpty(vaiTro))
            {
                if (vaiTro == "Học viên") return RedirectToAction("DangKy", "CongHocVien");
                return RedirectToAction("Dashboard", "ThongKe");
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _taiKhoanService.RegisterStudentAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            var tk = result.User!;
            var hv = result.HocVien!;

            // TỰ ĐỘNG ĐĂNG NHẬP VÀO SESSION
            HttpContext.Session.SetInt32("MaTaiKhoan", tk.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", tk.TenDangNhap);
            HttpContext.Session.SetString("HoTen", tk.HoTen);
            HttpContext.Session.SetString("VaiTro", tk.VaiTro);
            HttpContext.Session.SetString("Email", tk.Email);
            HttpContext.Session.SetInt32("MaHocVien", hv.MaHocVien);

            TempData["Success"] = $"Chúc mừng {tk.HoTen}! Bạn đã đăng ký thành công tài khoản Học viên UNETI EDUSPACE.";
            return RedirectToAction("DangKy", "CongHocVien");
        }

        // --- ĐĂNG NHẬP / ĐĂNG XUẤT ---

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (!string.IsNullOrEmpty(vaiTro))
            {
                if (vaiTro == "Học viên") return RedirectToAction("DangKy", "CongHocVien");
                return RedirectToAction("Dashboard", "ThongKe");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau, string? returnUrl = null)
        {
            var result = await _taiKhoanService.AuthenticateAsync(tenDangNhap, matKhau);
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var user = result.User!;
            HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);
            HttpContext.Session.SetString("Email", user.Email);

            if (user.VaiTro == "Học viên" && result.HocVien != null)
            {
                HttpContext.Session.SetInt32("MaHocVien", result.HocVien.MaHocVien);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user.VaiTro == "Học viên")
            {
                return RedirectToAction("DangKy", "CongHocVien");
            }
            return RedirectToAction("Dashboard", "ThongKe");
        }

        // Đăng nhập nhanh hỗ trợ kiểm thử tiện lợi
        [HttpGet]
        public async Task<IActionResult> QuickLogin(string role = "Admin")
        {
            var user = await _taiKhoanService.GetQuickLoginUserAsync(role);
            if (user != null)
            {
                var auth = await _taiKhoanService.AuthenticateAsync(user.TenDangNhap, user.MatKhau);
                if (auth.Success)
                {
                    HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
                    HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
                    HttpContext.Session.SetString("HoTen", user.HoTen);
                    HttpContext.Session.SetString("VaiTro", user.VaiTro);
                    HttpContext.Session.SetString("Email", user.Email);

                    if (user.VaiTro == "Học viên" && auth.HocVien != null)
                    {
                        HttpContext.Session.SetInt32("MaHocVien", auth.HocVien.MaHocVien);
                        return RedirectToAction("DangKy", "CongHocVien");
                    }
                    return RedirectToAction("Dashboard", "ThongKe");
                }
            }

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // --- QUẢN LÝ DANH SÁCH TÀI KHOẢN (ADMIN / GIÁO VỤ) ---

        public async Task<IActionResult> Index(int page = 1, string search = "", string vaiTro = "", string trangThai = "")
        {
            int pageSize = 10;
            var (items, totalItems, totalPages, currentPage, tongTaiKhoan, dangHoatDong, biKhoa) = 
                await _taiKhoanService.GetFilteredAccountsAsync(page, pageSize, search, vaiTro, trangThai);

            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.SelectedVaiTro = vaiTro;
            ViewBag.SelectedTrangThai = trangThai;

            ViewBag.TongTaiKhoan = tongTaiKhoan;
            ViewBag.DangHoatDong = dangHoatDong;
            ViewBag.BiKhoa = biKhoa;

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(TaiKhoan tk)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _taiKhoanService.CreateAccountAsync(tk);

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
        public async Task<IActionResult> EditPost(int maTaiKhoan, string hoTen, string email, string vaiTro, string trangThai, string? matKhauMoi)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var result = await _taiKhoanService.EditAccountAsync(maTaiKhoan, hoTen, email, vaiTro, trangThai, matKhauMoi);

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
