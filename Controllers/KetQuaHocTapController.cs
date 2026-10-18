// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Xử lý nghiệp vụ điểm số, CRUD kết quả học tập (Module 5)

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class KetQuaHocTapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KetQuaHocTapController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? maLop, string? ketQua, string? search, int page = 1)
        {
            int pageSize = 8;
            var query = _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                .ThenInclude(d => d.HocVien)
                .Include(k => k.DangKyHoc)
                .ThenInclude(d => d.LopHoc)
                .AsQueryable();

            // 1. Áp dụng bộ lọc
            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(k => k.DangKyHoc.MaLop == maLop.Value);
            }

            if (!string.IsNullOrWhiteSpace(ketQua))
            {
                query = query.Where(k => k.KetQua == ketQua);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(k => k.DangKyHoc.HocVien.HoTen.ToLower().Contains(s) || 
                                         k.DangKyHoc.HocVien.MaHocVien.ToString().Contains(s) ||
                                         k.DangKyHoc.LopHoc.TenLop.ToLower().Contains(s));
            }

            // 2. Tính toán 3 KPI Stats cho Header
            var allScores = await _context.KetQuaHocTaps.Select(k => new { k.DiemTongKet, k.KetQua }).ToListAsync();
            int totalGraded = allScores.Count;
            int passCount = allScores.Count(k => k.KetQua == "Hoàn thành");
            int failCount = allScores.Count(k => k.KetQua == "Không hoàn thành");
            double avgScore = allScores.Any() ? Math.Round(allScores.Average(k => k.DiemTongKet), 2) : 0;
            double passRate = totalGraded > 0 ? Math.Round((double)passCount / totalGraded * 100, 1) : 0;

            ViewBag.TotalGraded = totalGraded;
            ViewBag.PassCount = passCount;
            ViewBag.FailCount = failCount;
            ViewBag.AverageScore = avgScore;
            ViewBag.PassRate = passRate;

            // Phổ điểm (Grade distribution)
            ViewBag.CountA = allScores.Count(d => d.DiemTongKet >= 8.5);
            ViewBag.CountB = allScores.Count(d => d.DiemTongKet >= 7.0 && d.DiemTongKet < 8.5);
            ViewBag.CountC = allScores.Count(d => d.DiemTongKet >= 5.5 && d.DiemTongKet < 7.0);
            ViewBag.CountD = allScores.Count(d => d.DiemTongKet >= 4.0 && d.DiemTongKet < 5.5);
            ViewBag.CountF = allScores.Count(d => d.DiemTongKet < 4.0);

            // 3. Phân trang
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var ketQuas = await query
                .OrderByDescending(k => k.NgayCapNhat)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedKetQua = ketQua;
            ViewBag.Search = search;

            // 4. Danh sách chọn lớp học và danh sách học viên đủ điều kiện để nhập điểm
            ViewBag.LopHocList = await _context.LopHocs.ToListAsync();
            ViewBag.EligibleDangKys = await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .Where(d => d.TrangThai == "Đang học" || d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí")
                .ToListAsync();

            return View(ketQuas);
        }

        public IActionResult Create()
        {
            ViewBag.EligibleDangKys = _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KetQuaHocTap kq)
        {
            ModelState.Remove("DangKyHoc");
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var dangKy = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == kq.MaDangKy);

            if (dangKy == null)
            {
                ModelState.AddModelError("", "Không tìm thấy hồ sơ đăng ký hợp lệ.");
            }
            else
            {
                // Kiểm tra xem đã có kết quả trước đó chưa
                bool daCoDiem = await _context.KetQuaHocTaps.AnyAsync(k => k.MaDangKy == kq.MaDangKy);
                if (daCoDiem)
                {
                    ModelState.AddModelError("", "Học viên này đã được ghi nhận kết quả học tập.");
                }
            }

            if (kq.DiemChuyenCan < 0 || kq.DiemChuyenCan > 10 || kq.DiemCuoiKhoa < 0 || kq.DiemCuoiKhoa > 10)
            {
                ModelState.AddModelError("", "Điểm chuyên cần và cuối khóa phải nằm trong khoảng từ 0 đến 10.");
            }

            if (ModelState.IsValid)
            {
                // Tự động tính điểm tổng kết: 30% CC + 70% CK
                kq.DiemTongKet = Math.Round((kq.DiemChuyenCan * 0.3) + (kq.DiemCuoiKhoa * 0.7), 2);
                if (string.IsNullOrEmpty(kq.KetQua))
                {
                    kq.KetQua = kq.DiemTongKet >= 5.0 ? "Hoàn thành" : "Không hoàn thành";
                }
                kq.NgayCapNhat = DateTime.Now;

                _context.Add(kq);

                // Cập nhật trạng thái đăng ký thành Hoàn thành nếu đạt
                if (dangKy != null)
                {
                    dangKy.TrangThai = kq.KetQua == "Hoàn thành" ? "Hoàn thành" : "Không hoàn thành";
                }

                await _context.SaveChangesAsync();
                if (isAjax)
                {
                    return Json(new { success = true, message = "Ghi nhận kết quả học tập thành công!" });
                }
                TempData["Success"] = "Ghi nhận kết quả học tập thành công!";
                return RedirectToAction(nameof(Index));
            }

            string errorMsg = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            if (isAjax)
            {
                return Json(new { success = false, message = errorMsg });
            }
            TempData["Error"] = errorMsg;
            return RedirectToAction(nameof(Index));
        }

        // --- CẬP NHẬT ĐIỂM SỐ TỪ RIGHT SLIDE-OVER DRAWER ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int maKetQua, double diemChuyenCan, double diemCuoiKhoa, string? nhanXet, string? ketQua)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var kq = await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                .FirstOrDefaultAsync(k => k.MaKetQua == maKetQua);

            if (kq == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy bản ghi kết quả học tập." });
                TempData["Error"] = "Không tìm thấy bản ghi kết quả học tập.";
                return RedirectToAction(nameof(Index));
            }

            if (diemChuyenCan < 0 || diemChuyenCan > 10 || diemCuoiKhoa < 0 || diemCuoiKhoa > 10)
            {
                if (isAjax) return Json(new { success = false, message = "Điểm phải nằm trong khoảng từ 0 đến 10." });
                TempData["Error"] = "Điểm phải nằm trong khoảng từ 0 đến 10.";
                return RedirectToAction(nameof(Index));
            }

            kq.DiemChuyenCan = diemChuyenCan;
            kq.DiemCuoiKhoa = diemCuoiKhoa;
            kq.DiemTongKet = Math.Round((diemChuyenCan * 0.3) + (diemCuoiKhoa * 0.7), 2);
            kq.NhanXet = nhanXet ?? "";
            
            if (!string.IsNullOrEmpty(ketQua))
            {
                kq.KetQua = ketQua == "pass" || ketQua == "Hoàn thành" ? "Hoàn thành" : "Không hoàn thành";
            }
            else
            {
                kq.KetQua = kq.DiemTongKet >= 5.0 ? "Hoàn thành" : "Không hoàn thành";
            }
            kq.NgayCapNhat = DateTime.Now;

            if (kq.DangKyHoc != null)
            {
                kq.DangKyHoc.TrangThai = kq.KetQua;
            }

            await _context.SaveChangesAsync();
            if (isAjax) return Json(new { success = true, message = "Đã cập nhật thành công kết quả học tập!" });
            TempData["Success"] = "Đã cập nhật thành công kết quả học tập!";
            return RedirectToAction(nameof(Index));
        }

        // --- XÓA KẾT QUẢ TỪ DELETE MODAL ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var kq = await _context.KetQuaHocTaps.FindAsync(id);
            if (kq == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy kết quả học tập để xóa." });
                TempData["Error"] = "Không tìm thấy kết quả học tập để xóa.";
                return RedirectToAction(nameof(Index));
            }

            _context.KetQuaHocTaps.Remove(kq);
            await _context.SaveChangesAsync();
            if (isAjax) return Json(new { success = true, message = "Đã xóa bản ghi kết quả học tập khỏi hệ thống." });
            TempData["Success"] = "Đã xóa bản ghi kết quả học tập khỏi hệ thống.";
            return RedirectToAction(nameof(Index));
        }
    }
}
