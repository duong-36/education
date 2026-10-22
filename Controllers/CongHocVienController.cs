// Họ và tên: Nguyễn Đức Linh
// Mã sinh viên: 23103100042
// Nội dung thực hiện: Cổng Học viên (Module 3) - Nền tảng Thương Mại Giáo Dục (Edu-Commerce / EdTech Marketplace)

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

        // --- TRANG CHỦ MARKETPLACE KHÓA HỌC (STOREFRONT EDU-COMMERCE) ---
        public async Task<IActionResult> DangKy(
            string search,
            int? maChuongTrinh,
            string trinhDo,
            string mucGia,
            string sapXep,
            int page = 1,
            int pageSize = 12)
        {
            var student = await GetCurrentHocVienAsync();
            ViewBag.CurrentStudent = student;

            var query = _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .AsNoTracking()
                .Where(l => l.TrangThai == "Đang mở đăng ký");

            // 1. Tìm kiếm theo tên lớp học, giảng viên, tên CTĐT
            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(l =>
                    l.TenLop.ToLower().Contains(kw) ||
                    l.GiangVien.ToLower().Contains(kw) ||
                    (l.ChuongTrinhDaoTao != null && l.ChuongTrinhDaoTao.TenChuongTrinh.ToLower().Contains(kw)));
            }

            // 2. Lọc theo chuyên ngành / Chương trình đào tạo
            if (maChuongTrinh.HasValue)
            {
                query = query.Where(l => l.MaChuongTrinh == maChuongTrinh.Value);
            }

            // 3. Lọc theo trình độ đầu vào
            if (!string.IsNullOrWhiteSpace(trinhDo))
            {
                query = query.Where(l => l.TrinhDoDauVao == trinhDo);
            }

            // 4. Lọc theo khoảng học phí
            if (mucGia == "duoi_3tr")
            {
                query = query.Where(l => l.HocPhi < 3000000);
            }
            else if (mucGia == "3tr_6tr")
            {
                query = query.Where(l => l.HocPhi >= 3000000 && l.HocPhi <= 6000000);
            }
            else if (mucGia == "tren_6tr")
            {
                query = query.Where(l => l.HocPhi > 6000000);
            }

            // 5. Sắp xếp E-commerce
            query = sapXep switch
            {
                "gia_tang" => query.OrderBy(l => l.HocPhi),
                "gia_giam" => query.OrderByDescending(l => l.HocPhi),
                "khai_giang_som" => query.OrderBy(l => l.NgayKhaiGiang),
                "han_dang_ky" => query.OrderBy(l => l.HanDangKy),
                _ => query.OrderByDescending(l => l.MaLop)
            };

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var classes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tính sĩ số đã duyệt cho từng lớp hiển thị
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
                myRegistrations = dks
                    .GroupBy(d => d.MaLop)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(d => d.MaDangKy).First());
            }

            // Marketplace Stats
            ViewBag.TongKhoaHoc = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
            ViewBag.TongHocVien = await _context.HocViens.CountAsync();
            ViewBag.TongChuongTrinh = await _context.ChuongTrinhDaoTaos.CountAsync();

            ViewBag.SiSoMap = siSoMap;
            ViewBag.MyRegistrations = myRegistrations;
            ViewBag.ChuongTrinhList = await _context.ChuongTrinhDaoTaos.Where(c => c.TrangThai == "Đang hoạt động").ToListAsync();
            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedTrinhDo = trinhDo;
            ViewBag.SelectedMucGia = mucGia;
            ViewBag.SelectedSapXep = sapXep;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(classes);
        }

        // --- TRANG LANDING PAGE CHI TIẾT KHÓA HỌC (COURSE DETAILS & STICKY CTA) ---
        public async Task<IActionResult> ChiTiet(int id)
        {
            var student = await GetCurrentHocVienAsync();
            ViewBag.CurrentStudent = student;

            var lop = await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .FirstOrDefaultAsync(l => l.MaLop == id);

            if (lop == null) return NotFound();

            // Danh sách buổi học chi tiết
            var buoiHocs = await _context.BuoiHocs
                .Where(b => b.MaLop == id && b.TrangThai != "Đã hủy")
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();

            // Sĩ số đã tiếp nhận
            int enrolledCount = await _context.DangKyHocs.CountAsync(d =>
                d.MaLop == id &&
                (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            // Đơn đăng ký của học viên hiện tại nếu có
            DangKyHoc myReg = null;
            if (student != null)
            {
                myReg = await _context.DangKyHocs
                    .Where(d => d.MaHocVien == student.MaHocVien && d.MaLop == id && d.TrangThai != "Đã hủy")
                    .OrderByDescending(d => d.MaDangKy)
                    .FirstOrDefaultAsync();
            }

            // Các khóa học tương tự cùng chương trình đào tạo
            var similarCourses = await _context.LopHocs
                .Where(l => l.MaChuongTrinh == lop.MaChuongTrinh && l.MaLop != id && l.TrangThai == "Đang mở đăng ký")
                .Take(3)
                .ToListAsync();

            ViewBag.BuoiHocs = buoiHocs;
            ViewBag.EnrolledCount = enrolledCount;
            ViewBag.RemainingSeats = Math.Max(0, lop.SiSoToiDa - enrolledCount);
            ViewBag.PercentFilled = lop.SiSoToiDa > 0 ? Math.Min(100.0, Math.Round((double)enrolledCount / lop.SiSoToiDa * 100, 1)) : 0;
            ViewBag.MyRegistration = myReg;
            ViewBag.SimilarCourses = similarCourses;

            return View(lop);
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
                return Json(new { success = false, message = "Lớp học này hiện không mở tuyển sinh." });
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
                return Json(new { success = false, message = $"Bạn đã đăng ký khóa học này rồi (Trạng thái: {existing.TrangThai})." });
            }

            // Kiểm tra sĩ số còn chỗ
            int enrolledCount = await _context.DangKyHocs.CountAsync(d =>
                d.MaLop == maLop &&
                (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            if (enrolledCount >= lop.SiSoToiDa)
            {
                return Json(new { success = false, message = "Khóa học đã đủ chỉ tiêu sĩ số. Vui lòng chọn lớp học khác." });
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

            return Json(new
            {
                success = true,
                message = $"Đăng ký thành công khóa học '{lop.TenLop}'! Đơn của bạn đang ở trạng thái 'Chờ duyệt'."
            });
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
                return Json(new { success = false, message = $"Không thể hủy lớp khi đã ở trạng thái '{dk.TrangThai}'. Vui lòng liên hệ ban giáo vụ để được hỗ trợ bảo lưu." });
            }

            dk.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã hủy đăng ký lớp '{dk.LopHoc?.TenLop}' thành công." });
        }

        // --- KHÔNG GIAN HỌC TẬP CỦA TÔI (MY LEARNING DASHBOARD) ---
        public async Task<IActionResult> TienTrinh()
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem không gian học tập.";
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
                .Where(b => activeClassIds.Contains(b.MaLop) && b.TrangThai != "Đã hủy")
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();

            // Đếm số buổi đã hoàn thành cho từng lớp để tính tiến độ %
            var completedSessionsCount = await _context.BuoiHocs
                .Where(b => activeClassIds.Contains(b.MaLop) && b.TrangThai == "Đã hoàn thành")
                .GroupBy(b => b.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            // Bảng điểm cá nhân
            var ketQuas = await _context.KetQuaHocTaps
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == student.MaHocVien)
                .ToListAsync();

            ViewBag.BuoiHocs = buoiHocs;
            ViewBag.CompletedSessionsCount = completedSessionsCount;
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

            // Lịch sử thanh toán học phí
            var paymentHistory = await _context.DangKyHocs
                .Include(d => d.LopHoc)
                .Where(d => d.MaHocVien == student.MaHocVien && d.SoTienDaDong > 0)
                .OrderByDescending(d => d.NgayDongHocPhi)
                .ToListAsync();

            ViewBag.PaymentHistory = paymentHistory;

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

            return Json(new { success = true, message = "Cập nhật thông tin cá nhân thành công!" });
        }
    }
}
