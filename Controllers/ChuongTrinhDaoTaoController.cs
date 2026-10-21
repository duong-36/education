// Họ và tên: [Tên SV1]
// Mã sinh viên: [Mã SV1]
// Nội dung thực hiện: Module 1 - Quản lý Chương trình đào tạo (CTĐT)

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class ChuongTrinhDaoTaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuongTrinhDaoTaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, string search = "", string trangThai = "")
        {
            int pageSize = 8;
            var query = _context.ChuongTrinhDaoTaos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(c => c.TenChuongTrinh.ToLower().Contains(s) || 
                                         (c.MoTa != null && c.MoTa.ToLower().Contains(s)) ||
                                         (c.EmailLienHe != null && c.EmailLienHe.ToLower().Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(c => c.TrangThai == trangThai);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(c => c.MaChuongTrinh)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tính số lớp học tương ứng cho từng CTĐT để hiển thị trên bảng
            var ctdtIds = items.Select(c => c.MaChuongTrinh).ToList();
            var classCounts = await _context.LopHocs
                .Where(l => ctdtIds.Contains(l.MaChuongTrinh))
                .GroupBy(l => l.MaChuongTrinh)
                .Select(g => new { MaChuongTrinh = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaChuongTrinh, x => x.Count);

            ViewBag.ClassCounts = classCounts;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.SelectedTrangThai = trangThai;

            // 3 thẻ KPI
            ViewBag.TongCTDT = await _context.ChuongTrinhDaoTaos.CountAsync();
            ViewBag.DangHoatDong = await _context.ChuongTrinhDaoTaos.CountAsync(c => c.TrangThai == "Đang hoạt động");
            ViewBag.TongLopLienKet = await _context.LopHocs.CountAsync();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(ChuongTrinhDaoTao ctdt)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (string.IsNullOrWhiteSpace(ctdt.TenChuongTrinh))
            {
                if (isAjax) return Json(new { success = false, message = "Tên chương trình đào tạo bắt buộc nhập." });
                TempData["Error"] = "Tên chương trình đào tạo bắt buộc nhập.";
                return RedirectToAction(nameof(Index));
            }

            ctdt.TenChuongTrinh = ctdt.TenChuongTrinh.Trim();
            bool exists = await _context.ChuongTrinhDaoTaos.AnyAsync(c => c.TenChuongTrinh.ToLower() == ctdt.TenChuongTrinh.ToLower());
            if (exists)
            {
                if (isAjax) return Json(new { success = false, message = "Tên chương trình đào tạo này đã tồn tại." });
                TempData["Error"] = "Tên chương trình đào tạo này đã tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(ctdt.TrangThai)) ctdt.TrangThai = "Đang hoạt động";
            if (string.IsNullOrEmpty(ctdt.EmailLienHe)) ctdt.EmailLienHe = "contact@uneti.edu.vn";

            _context.Add(ctdt);
            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Tạo chương trình đào tạo mới thành công!" });
            TempData["Success"] = "Tạo chương trình đào tạo mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int maChuongTrinh, string tenChuongTrinh, string? moTa, string? emailLienHe, string trangThai)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var ctdt = await _context.ChuongTrinhDaoTaos.FindAsync(maChuongTrinh);
            if (ctdt == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy chương trình đào tạo cần sửa." });
                TempData["Error"] = "Không tìm thấy chương trình đào tạo.";
                return RedirectToAction(nameof(Index));
            }

            tenChuongTrinh = tenChuongTrinh.Trim();
            bool exists = await _context.ChuongTrinhDaoTaos
                .AnyAsync(c => c.MaChuongTrinh != maChuongTrinh && c.TenChuongTrinh.ToLower() == tenChuongTrinh.ToLower());
            if (exists)
            {
                if (isAjax) return Json(new { success = false, message = "Tên chương trình đào tạo này đã trùng với chương trình khác." });
                TempData["Error"] = "Tên chương trình đào tạo này đã trùng.";
                return RedirectToAction(nameof(Index));
            }

            ctdt.TenChuongTrinh = tenChuongTrinh;
            ctdt.MoTa = moTa ?? "";
            ctdt.EmailLienHe = emailLienHe ?? "contact@uneti.edu.vn";
            ctdt.TrangThai = trangThai;

            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Cập nhật chương trình đào tạo thành công!" });
            TempData["Success"] = "Cập nhật chương trình đào tạo thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var ctdt = await _context.ChuongTrinhDaoTaos.FindAsync(id);
            if (ctdt == null)
            {
                if (isAjax) return Json(new { success = false, message = "Chương trình đào tạo không tồn tại." });
                return RedirectToAction(nameof(Index));
            }

            // Quy định nghiệp vụ Mục 5.5: Không được xóa chương trình nếu có lớp học liên quan
            int countLop = await _context.LopHocs.CountAsync(l => l.MaChuongTrinh == id);
            if (countLop > 0)
            {
                string msg = $"Không thể xóa chương trình '{ctdt.TenChuongTrinh}' vì đang có {countLop} lớp học trực thuộc. Vui lòng chuyển hoặc xóa các lớp học trước.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            _context.ChuongTrinhDaoTaos.Remove(ctdt);
            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = $"Đã xóa chương trình đào tạo '{ctdt.TenChuongTrinh}' thành công." });
            TempData["Success"] = "Đã xóa chương trình đào tạo thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
