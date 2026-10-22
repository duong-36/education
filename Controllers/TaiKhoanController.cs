// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Nội dung thực hiện: Module 1 - Tài khoản, Đăng nhập, Phân quyền Session

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ĐĂNG NHẬP / ĐĂNG XUẤT ---

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            // Nếu đã đăng nhập thì chuyển hướng phù hợp
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
            if (string.IsNullOrWhiteSpace(tenDangNhap) || string.IsNullOrWhiteSpace(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            tenDangNhap = tenDangNhap.Trim();
            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap && t.MatKhau == matKhau);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return View();
            }

            if (user.TrangThai == "Bị khóa")
            {
                ViewBag.Error = "Tài khoản này hiện đang bị khóa. Vui lòng liên hệ Admin để được hỗ trợ.";
                return View();
            }

            // Ghi nhận thông tin vào Session
            HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("HoTen", user.HoTen);
            HttpContext.Session.SetString("VaiTro", user.VaiTro);
            HttpContext.Session.SetString("Email", user.Email);

            // Nếu là Học viên, lưu thêm MaHocVien
            if (user.VaiTro == "Học viên")
            {
                var hv = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == user.MaTaiKhoan);
                if (hv != null)
                {
                    HttpContext.Session.SetInt32("MaHocVien", hv.MaHocVien);
                }
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
            TaiKhoan? user = null;
            if (role == "Admin")
            {
                user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.VaiTro == "Admin" && t.TrangThai == "Đang hoạt động");
            }
            else if (role == "GiaoVu")
            {
                user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.VaiTro == "Giáo vụ" && t.TrangThai == "Đang hoạt động");
            }
            else
            {
                user = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.VaiTro == "Học viên" && t.TrangThai == "Đang hoạt động");
            }

            if (user != null)
            {
                HttpContext.Session.SetInt32("MaTaiKhoan", user.MaTaiKhoan);
                HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
                HttpContext.Session.SetString("HoTen", user.HoTen);
                HttpContext.Session.SetString("VaiTro", user.VaiTro);
                HttpContext.Session.SetString("Email", user.Email);

                if (user.VaiTro == "Học viên")
                {
                    var hv = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == user.MaTaiKhoan);
                    if (hv != null) HttpContext.Session.SetInt32("MaHocVien", hv.MaHocVien);
                    return RedirectToAction("DangKy", "CongHocVien");
                }
                return RedirectToAction("Dashboard", "ThongKe");
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
            var query = _context.TaiKhoans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(s) || 
                                         t.HoTen.ToLower().Contains(s) || 
                                         t.Email.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(t => t.TrangThai == trangThai);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(t => t.MaTaiKhoan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.SelectedVaiTro = vaiTro;
            ViewBag.SelectedTrangThai = trangThai;

            // 3 KPI cards
            ViewBag.TongTaiKhoan = await _context.TaiKhoans.CountAsync();
            ViewBag.DangHoatDong = await _context.TaiKhoans.CountAsync(t => t.TrangThai == "Đang hoạt động");
            ViewBag.BiKhoa = await _context.TaiKhoans.CountAsync(t => t.TrangThai == "Bị khóa");

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(TaiKhoan tk)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (string.IsNullOrWhiteSpace(tk.TenDangNhap) || string.IsNullOrWhiteSpace(tk.MatKhau))
            {
                if (isAjax) return Json(new { success = false, message = "Tên đăng nhập và mật khẩu không được để trống." });
                TempData["Error"] = "Tên đăng nhập và mật khẩu không được để trống.";
                return RedirectToAction(nameof(Index));
            }

            tk.TenDangNhap = tk.TenDangNhap.Trim();
            bool exists = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == tk.TenDangNhap.ToLower());
            if (exists)
            {
                if (isAjax) return Json(new { success = false, message = "Tên đăng nhập này đã tồn tại trong hệ thống." });
                TempData["Error"] = "Tên đăng nhập này đã tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(tk.TrangThai)) tk.TrangThai = "Đang hoạt động";
            if (string.IsNullOrEmpty(tk.VaiTro)) tk.VaiTro = "Học viên";

            _context.Add(tk);
            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Thêm tài khoản mới thành công!" });
            TempData["Success"] = "Thêm tài khoản mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int maTaiKhoan, string hoTen, string email, string vaiTro, string trangThai, string? matKhauMoi)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var user = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            if (user == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy tài khoản cần chỉnh sửa." });
                TempData["Error"] = "Không tìm thấy tài khoản.";
                return RedirectToAction(nameof(Index));
            }

            user.HoTen = hoTen.Trim();
            user.Email = email.Trim();
            user.VaiTro = vaiTro;
            user.TrangThai = trangThai;
            if (!string.IsNullOrWhiteSpace(matKhauMoi))
            {
                user.MatKhau = matKhauMoi.Trim();
            }

            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Cập nhật tài khoản thành công!" });
            TempData["Success"] = "Cập nhật tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLock(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var user = await _context.TaiKhoans.FindAsync(id);
            if (user == null)
            {
                if (isAjax) return Json(new { success = false, message = "Tài khoản không tồn tại." });
                return RedirectToAction(nameof(Index));
            }

            user.TrangThai = (user.TrangThai == "Bị khóa") ? "Đang hoạt động" : "Bị khóa";
            await _context.SaveChangesAsync();

            string msg = user.TrangThai == "Bị khóa" ? $"Đã khóa tài khoản {user.TenDangNhap}." : $"Đã mở khóa tài khoản {user.TenDangNhap}.";
            if (isAjax) return Json(new { success = true, message = msg, trangThai = user.TrangThai });

            TempData["Success"] = msg;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var user = await _context.TaiKhoans.FindAsync(id);
            if (user == null)
            {
                if (isAjax) return Json(new { success = false, message = "Tài khoản không tồn tại." });
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra ràng buộc học viên
            bool coHocVien = await _context.HocViens.AnyAsync(h => h.MaTaiKhoan == id);
            if (coHocVien)
            {
                if (isAjax) return Json(new { success = false, message = "Không thể xóa tài khoản này vì đã có dữ liệu hồ sơ học viên liên kết. Bạn có thể chọn khóa tài khoản." });
                TempData["Error"] = "Không thể xóa tài khoản đã có hồ sơ học viên liên kết.";
                return RedirectToAction(nameof(Index));
            }

            _context.TaiKhoans.Remove(user);
            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Đã xóa tài khoản vĩnh viễn." });
            TempData["Success"] = "Đã xóa tài khoản vĩnh viễn.";
            return RedirectToAction(nameof(Index));
        }
    }
}
