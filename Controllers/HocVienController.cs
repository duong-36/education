// Họ và tên: [Tên SV3]
// Mã sinh viên: [Mã SV3]
// Nội dung thực hiện: Controller Quản lý Học viên (Module 3) - Phía Giáo vụ & Admin

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class HocVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HocVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- DANH SÁCH HỌC VIÊN (LINQ SEARCH, FILTER, PAGINATION, AJAX) ---
        public async Task<IActionResult> Index(
            string search,
            string trangThai,
            string trinhDo,
            int page = 1,
            int pageSize = 10)
        {
            // Kiểm tra quyền cơ bản (nếu chưa đăng nhập cho test)
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .AsNoTracking()
                .AsQueryable();

            // 1. Tìm kiếm LINQ
            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(h =>
                    h.HoTen.ToLower().Contains(kw) ||
                    h.Email.ToLower().Contains(kw) ||
                    h.SoDienThoai.Contains(kw) ||
                    ("hv" + h.MaHocVien.ToString()).Contains(kw));
            }

            // 2. Lọc trạng thái
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(h => h.TrangThai == trangThai);
            }

            // 3. Lọc trình độ học vấn
            if (!string.IsNullOrWhiteSpace(trinhDo))
            {
                query = query.Where(h => h.TrinhDoHocVan == trinhDo);
            }

            // Thống kê KPIs (toàn bộ database)
            ViewBag.TongSoHocVien = await _context.HocViens.CountAsync();
            ViewBag.DangHoc = await _context.HocViens.CountAsync(h => h.TrangThai == "Đang học");
            ViewBag.TotNghiep = await _context.HocViens.CountAsync(h => h.TrangThai == "Đã tốt nghiệp" || h.TrangThai == "Hoàn thành");

            // Danh sách trình độ học vấn phục vụ dropdown
            ViewBag.TrinhDoList = await _context.HocViens
                .Where(h => !string.IsNullOrEmpty(h.TrinhDoHocVan))
                .Select(h => h.TrinhDoHocVan)
                .Distinct()
                .ToListAsync();

            // Phân trang LINQ
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(h => h.MaHocVien)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.SelectedTrinhDo = trinhDo;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_HocVienTablePartial", items);
            }

            return View(items);
        }

        // --- CHI TIẾT HỌC VIÊN ---
        public async Task<IActionResult> Details(int id)
        {
            var hv = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hv == null) return NotFound();

            var dangKys = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(d => d.MaHocVien == id)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            var ketQuas = await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == id)
                .ToListAsync();

            ViewBag.DangKys = dangKys;
            ViewBag.KetQuas = ketQuas;

            return View(hv);
        }

        // --- GET HỌC VIÊN CHO DRAWER EDIT (AJAX) ---
        [HttpGet]
        public async Task<IActionResult> GetHocVien(int id)
        {
            var hv = await _context.HocViens.FindAsync(id);
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

            if (string.IsNullOrWhiteSpace(model.HoTen))
            {
                if (isAjax) return Json(new { success = false, message = "Họ tên học viên bắt buộc nhập." });
                TempData["Error"] = "Họ tên học viên bắt buộc nhập.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                if (isAjax) return Json(new { success = false, message = "Email học viên bắt buộc nhập." });
                TempData["Error"] = "Email bắt buộc nhập.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra trùng email
            if (await _context.HocViens.AnyAsync(h => h.Email.ToLower() == model.Email.ToLower()))
            {
                if (isAjax) return Json(new { success = false, message = "Email này đã được sử dụng bởi học viên khác." });
                TempData["Error"] = "Email đã tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            // Tự sinh hoặc kiểm tra tên đăng nhập
            string username = string.IsNullOrWhiteSpace(tenDangNhap) ? model.Email.Split('@')[0] : tenDangNhap.Trim();
            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == username))
            {
                username = username + "_" + new Random().Next(100, 999);
            }

            string password = string.IsNullOrWhiteSpace(matKhau) ? "123456" : matKhau;

            // Tạo tài khoản liên kết
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = username,
                MatKhau = password,
                HoTen = model.HoTen,
                Email = model.Email,
                VaiTro = "Học viên",
                TrangThai = "Đang hoạt động"
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // Lưu hồ sơ học viên
            model.MaTaiKhoan = taiKhoan.MaTaiKhoan;
            if (model.NgaySinh == default) model.NgaySinh = new DateTime(2000, 1, 1);
            if (string.IsNullOrWhiteSpace(model.TrangThai)) model.TrangThai = "Đang học";

            _context.HocViens.Add(model);
            await _context.SaveChangesAsync();

            if (isAjax)
            {
                return Json(new { success = true, message = $"Thêm học viên thành công! Tài khoản: {username}" });
            }

            TempData["Success"] = "Thêm học viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- EDIT HỌC VIÊN (AJAX DRAWER) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(HocVien model)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var existing = await _context.HocViens.FindAsync(model.MaHocVien);
            if (existing == null)
            {
                if (isAjax) return Json(new { success = false, message = "Học viên không tồn tại." });
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(model.HoTen))
            {
                if (isAjax) return Json(new { success = false, message = "Họ tên không được để trống." });
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra trùng email với học viên khác
            if (await _context.HocViens.AnyAsync(h => h.Email.ToLower() == model.Email.ToLower() && h.MaHocVien != model.MaHocVien))
            {
                if (isAjax) return Json(new { success = false, message = "Email này đã được dùng bởi học viên khác." });
                return RedirectToAction(nameof(Index));
            }

            existing.HoTen = model.HoTen;
            existing.NgaySinh = model.NgaySinh;
            existing.GioiTinh = model.GioiTinh;
            existing.SoDienThoai = model.SoDienThoai;
            existing.Email = model.Email;
            existing.DiaChi = model.DiaChi;
            existing.TrinhDoHocVan = model.TrinhDoHocVan;
            existing.NgheNghiep = model.NgheNghiep;
            existing.TrangThai = model.TrangThai;

            // Đồng bộ sang TaiKhoan nếu có
            if (existing.MaTaiKhoan > 0)
            {
                var tk = await _context.TaiKhoans.FindAsync(existing.MaTaiKhoan);
                if (tk != null)
                {
                    tk.HoTen = model.HoTen;
                    tk.Email = model.Email;
                }
            }

            await _context.SaveChangesAsync();

            if (isAjax)
            {
                return Json(new { success = true, message = "Cập nhật học viên thành công!" });
            }

            TempData["Success"] = "Cập nhật học viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- DELETE HỌC VIÊN (RÀNG BUỘC TOÀN VẸN NGHIỆP VỤ) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var hv = await _context.HocViens.FindAsync(id);
            if (hv == null)
            {
                if (isAjax) return Json(new { success = false, message = "Học viên không tồn tại." });
                TempData["Error"] = "Học viên không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            // Ràng buộc nghiệp vụ: Không xóa nếu học viên đã có lịch sử đăng ký hoặc bảng điểm
            bool hasDangKy = await _context.DangKyHocs.AnyAsync(d => d.MaHocVien == id);
            bool hasKetQua = await _context.KetQuaHocTaps.AnyAsync(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == id);

            if (hasDangKy || hasKetQua)
            {
                string msg = $"Không thể xóa học viên #{hv.MaHocVien} ({hv.HoTen}) vì đã có {(hasDangKy ? "đơn đăng ký học" : "kết quả học tập")} trong hệ thống. Vui lòng chuyển trạng thái sang 'Đã nghỉ' hoặc 'Bảo lưu'.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            int tkId = hv.MaTaiKhoan;
            _context.HocViens.Remove(hv);

            // Xóa luôn tài khoản nếu có
            if (tkId > 0)
            {
                var tk = await _context.TaiKhoans.FindAsync(tkId);
                if (tk != null) _context.TaiKhoans.Remove(tk);
            }

            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Đã xóa học viên thành công!" });
            TempData["Success"] = "Đã xóa học viên thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
