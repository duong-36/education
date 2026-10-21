// Họ và tên: [Tên SV3]
// Mã sinh viên: [Mã SV3]
// Nội dung thực hiện: Cổng Học viên (Module 3) - Đăng ký môn, Theo dõi tiến trình, Hồ sơ cá nhân

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
    public class CongHocVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CongHocVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper lấy học viên hiện tại theo Session
        private async Task<HocVien> GetCurrentHocVienAsync()
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaHocVien");
            if (maHocVien.HasValue)
            {
                var hv = await _context.HocViens.FindAsync(maHocVien.Value);
                if (hv != null) return hv;
            }

            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan.HasValue)
            {
                var hv = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan.Value);
                if (hv != null) return hv;
            }

            // Hỗ trợ test nhanh nếu chưa đăng nhập hoặc là Admin test cổng học viên: lấy học viên đầu tiên
            return await _context.HocViens.FirstOrDefaultAsync();
        }

        // --- TRANG ĐĂNG KÝ HỌC PHẦN (DANH SÁCH LỚP ĐANG MỞ & CÒN CHỖ) ---
        public async Task<IActionResult> DangKy(string search, int? maChuongTrinh, string trinhDo)
        {
            var student = await GetCurrentHocVienAsync();
            ViewBag.CurrentStudent = student;

            var query = _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .AsNoTracking()
                .Where(l => l.TrangThai == "Đang mở đăng ký");

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(l => l.TenLop.ToLower().Contains(kw) || l.GiangVien.ToLower().Contains(kw));
            }

            if (maChuongTrinh.HasValue)
            {
                query = query.Where(l => l.MaChuongTrinh == maChuongTrinh.Value);
            }

            if (!string.IsNullOrWhiteSpace(trinhDo))
            {
                query = query.Where(l => l.TrinhDoDauVao == trinhDo);
            }

            var classes = await query
                .OrderBy(l => l.HanDangKy)
                .ToListAsync();

            // Tính sĩ số đã duyệt cho từng lớp
            var classIds = classes.Select(c => c.MaLop).ToList();
            var siSoMap = await _context.DangKyHocs
                .Where(d => classIds.Contains(d.MaLop) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"))
                .GroupBy(d => d.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            // Kiểm tra các lớp mà học viên hiện tại đã đăng ký
            var myRegistrations = new Dictionary<int, DangKyHoc>();
            if (student != null)
            {
                var dks = await _context.DangKyHocs
                    .Where(d => d.MaHocVien == student.MaHocVien && d.TrangThai != "Đã hủy")
                    .ToListAsync();
                myRegistrations = dks.ToDictionary(d => d.MaLop, d => d);
            }

            ViewBag.SiSoMap = siSoMap;
            ViewBag.MyRegistrations = myRegistrations;
            ViewBag.ChuongTrinhList = await _context.ChuongTrinhDaoTaos.Where(c => c.TrangThai == "Đang hoạt động").ToListAsync();
            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedTrinhDo = trinhDo;

            return View(classes);
        }

        // --- HÀNH ĐỘNG ĐĂNG KÝ LỚP HỌC (AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyLopPost(int maLop, string ghiChu)
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập tài khoản học viên để thực hiện đăng ký." });
            }

            var lop = await _context.LopHocs.FindAsync(maLop);
            if (lop == null)
            {
                return Json(new { success = false, message = "Lớp học không tồn tại." });
            }

            if (lop.TrangThai != "Đang mở đăng ký")
            {
                return Json(new { success = false, message = "Lớp học này hiện không mở đăng ký." });
            }

            if (lop.HanDangKy < DateTime.Today)
            {
                return Json(new { success = false, message = "Lớp học đã hết thời hạn đăng ký." });
            }

            // Kiểm tra đã đăng ký chưa
            var existing = await _context.DangKyHocs
                .FirstOrDefaultAsync(d => d.MaHocVien == student.MaHocVien && d.MaLop == maLop && d.TrangThai != "Đã hủy");

            if (existing != null)
            {
                return Json(new { success = false, message = $"Bạn đã đăng ký lớp học này rồi (Trạng thái: {existing.TrangThai})." });
            }

            // Kiểm tra sĩ số còn chỗ
            int enrolledCount = await _context.DangKyHocs.CountAsync(d =>
                d.MaLop == maLop &&
                (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            if (enrolledCount >= lop.SiSoToiDa)
            {
                return Json(new { success = false, message = "Lớp học này đã đủ sĩ số tối đa. Vui lòng chọn lớp khác." });
            }

            var dangKy = new DangKyHoc
            {
                MaHocVien = student.MaHocVien,
                MaLop = maLop,
                NgayDangKy = DateTime.Now,
                TrangThai = "Chờ duyệt",
                GhiChu = ghiChu ?? "",
                SoTienDaDong = 0
            };

            _context.DangKyHocs.Add(dangKy);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đăng ký lớp '{lop.TenLop}' thành công! Hồ sơ đang ở trạng thái 'Chờ duyệt'." });
        }

        // --- HÀNH ĐỘNG HỦY ĐĂNG KÝ LỚP HỌC (AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKyPost(int maDangKy)
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null) return Json(new { success = false, message = "Chưa xác thực học viên." });

            var dk = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            if (dk == null) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });

            if (dk.MaHocVien != student.MaHocVien)
            {
                return Json(new { success = false, message = "Bạn không có quyền thao tác trên hồ sơ của người khác." });
            }

            // Chỉ cho hủy khi còn Chờ duyệt hoặc Đã duyệt mà chưa đóng tiền
            if (dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học" || dk.TrangThai == "Hoàn thành")
            {
                return Json(new { success = false, message = $"Không thể hủy lớp khi đã ở trạng thái '{dk.TrangThai}'. Vui lòng liên hệ giáo vụ để được hỗ trợ." });
            }

            dk.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã hủy đăng ký lớp '{dk.LopHoc?.TenLop}' thành công." });
        }

        // --- TIẾN TRÌNH HỌC TẬP & KẾT QUẢ CÁ NHÂN ---
        public async Task<IActionResult> TienTrinh()
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem tiến trình học tập.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            ViewBag.CurrentStudent = student;

            // Danh sách các đăng ký của học viên
            var dangKys = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(d => d.MaHocVien == student.MaHocVien)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            // Danh sách các lớp học viên đang học hoặc đã đóng phí để lấy lịch học
            var activeClassIds = dangKys
                .Where(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học")
                .Select(d => d.MaLop)
                .ToList();

            var buoiHocs = await _context.BuoiHocs
                .Include(b => b.LopHoc)
                .Where(b => activeClassIds.Contains(b.MaLop))
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();

            // Bảng điểm cá nhân
            var ketQuas = await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == student.MaHocVien)
                .ToListAsync();

            ViewBag.BuoiHocs = buoiHocs;
            ViewBag.KetQuas = ketQuas;

            return View(dangKys);
        }

        // --- HỒ SƠ CÁ NHÂN HỌC VIÊN ---
        public async Task<IActionResult> HoSo()
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem hồ sơ.";
                return RedirectToAction("Login", "TaiKhoan");
            }

            var tk = await _context.TaiKhoans.FindAsync(student.MaTaiKhoan);
            ViewBag.TaiKhoan = tk;

            return View(student);
        }

        // --- CẬP NHẬT HỒ SƠ CÁ NHÂN ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatHoSoPost(HocVien model)
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null || student.MaHocVien != model.MaHocVien)
            {
                return Json(new { success = false, message = "Bạn không có quyền chỉnh sửa hồ sơ này." });
            }

            student.HoTen = model.HoTen;
            student.NgaySinh = model.NgaySinh;
            student.GioiTinh = model.GioiTinh;
            student.SoDienThoai = model.SoDienThoai;
            student.DiaChi = model.DiaChi;
            student.TrinhDoHocVan = model.TrinhDoHocVan;
            student.NgheNghiep = model.NgheNghiep;

            // Đồng bộ tên sang tài khoản
            var tk = await _context.TaiKhoans.FindAsync(student.MaTaiKhoan);
            if (tk != null)
            {
                tk.HoTen = model.HoTen;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật hồ sơ cá nhân thành công!" });
        }
    }
}
