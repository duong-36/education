// Họ và tên: Nguyễn Đức Linh
// Mã sinh viên: 23103100042
// Nội dung thực hiện: Cổng Học viên (Module 3) - Nền tảng Thương Mại Giáo Dục (Edu-Commerce / EdTech Marketplace)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers
{
    public class CongHocVienController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IDangKyHocService _dangKyHocService;
        private readonly IHocVienService _hocVienService;
        private readonly ILopHocService _lopHocService;
        private readonly IBuoiHocService _buoiHocService;
        private readonly IKetQuaHocTapService _ketQuaService;
        private readonly IChuongTrinhDaoTaoService _chuongTrinhService;
        private readonly ITaiKhoanService _taiKhoanService;

        public CongHocVienController(
            IUnitOfWork uow,
            IDangKyHocService dangKyHocService,
            IHocVienService hocVienService,
            ILopHocService lopHocService,
            IBuoiHocService buoiHocService,
            IKetQuaHocTapService ketQuaService,
            IChuongTrinhDaoTaoService chuongTrinhService,
            ITaiKhoanService taiKhoanService)
        {
            _uow = uow;
            _dangKyHocService = dangKyHocService;
            _hocVienService = hocVienService;
            _lopHocService = lopHocService;
            _buoiHocService = buoiHocService;
            _ketQuaService = ketQuaService;
            _chuongTrinhService = chuongTrinhService;
            _taiKhoanService = taiKhoanService;
        }

        // Helper lấy học viên hiện tại theo Session
        private async Task<HocVien?> GetCurrentHocVienAsync()
        {
            int? maHocVien = HttpContext.Session.GetInt32("MaHocVien");
            if (maHocVien.HasValue)
            {
                var hv = await _hocVienService.GetByIdAsync(maHocVien.Value);
                if (hv != null) return hv;
            }

            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan.HasValue)
            {
                var hv = await _hocVienService.GetByTaiKhoanIdAsync(maTaiKhoan.Value);
                if (hv != null) return hv;
            }

            return null;
        }

        // --- ĐIỀU HƯỚNG TRANG CHỦ CỔNG HỌC VIÊN ---
        public IActionResult Index() => RedirectToAction(nameof(DangKy));

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
            if (student == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập hoặc đăng ký tài khoản học viên để truy cập danh mục và đăng ký khóa học.";
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Request.Path + Request.QueryString });
            }

            ViewBag.CurrentStudent = student;

            var query = _uow.LopHocs.Query()
                .Include(l => l.ChuongTrinhDaoTao)
                .AsNoTracking()
                .Where(l => l.TrangThai == "Đang mở đăng ký");

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(l =>
                    l.TenLop.ToLower().Contains(kw) ||
                    l.GiangVien.ToLower().Contains(kw) ||
                    (l.ChuongTrinhDaoTao != null && l.ChuongTrinhDaoTao.TenChuongTrinh.ToLower().Contains(kw)));
            }

            if (maChuongTrinh.HasValue)
            {
                query = query.Where(l => l.MaChuongTrinh == maChuongTrinh.Value);
            }

            if (!string.IsNullOrWhiteSpace(trinhDo))
            {
                query = query.Where(l => l.TrinhDoDauVao == trinhDo);
            }

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

            var classIds = classes.Select(c => c.MaLop).ToList();
            var siSoMap = await _uow.DangKyHocs.Query()
                .Where(d => classIds.Contains(d.MaLop) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"))
                .GroupBy(d => d.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            var myRegistrations = new Dictionary<int, DangKyHoc>();
            var allMyDks = await _uow.DangKyHocs.Query()
                .Where(d => d.MaHocVien == student.MaHocVien && d.TrangThai != "Đã hủy")
                .ToListAsync();

            myRegistrations = allMyDks
                .GroupBy(d => d.MaLop)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(d => d.MaDangKy).First());

            var chuongTrinhs = await _chuongTrinhService.GetAllAsync();
            var openCounts = await _uow.LopHocs.Query()
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .GroupBy(l => l.MaChuongTrinh)
                .Select(g => new { MaChuongTrinh = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaChuongTrinh, x => x.Count);

            int totalOpenClasses = await _uow.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
            int totalEnrolledStudents = await _uow.DangKyHocs.CountAsync(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");
            int totalPrograms = await _uow.ChuongTrinhDaoTaos.CountAsync(c => c.TrangThai == "Đang hoạt động");

            ViewBag.ChuongTrinhs = chuongTrinhs;
            ViewBag.OpenCounts = openCounts;
            ViewBag.SiSoMap = siSoMap;
            ViewBag.MyRegistrations = myRegistrations;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;

            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedTrinhDo = trinhDo;
            ViewBag.SelectedMucGia = mucGia;
            ViewBag.SelectedSapXep = sapXep;

            ViewBag.TotalOpenClasses = totalOpenClasses;
            ViewBag.TotalEnrolledStudents = totalEnrolledStudents;
            ViewBag.TotalPrograms = totalPrograms;

            return View(classes);
        }

        // --- CHI TIẾT KHÓA HỌC (EDU-COMMERCE PDP) ---
        public async Task<IActionResult> ChiTiet(int id)
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập hoặc đăng ký tài khoản học viên để xem chi tiết khóa học.";
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Request.Path + Request.QueryString });
            }
            ViewBag.CurrentStudent = student;

            var lop = await _lopHocService.GetDetailsAsync(id);
            if (lop == null) return NotFound();

            var buoiHocs = await _buoiHocService.GetByLopHocAsync(id);

            int enrolledCount = await _uow.DangKyHocs.CountAsync(d =>
                d.MaLop == id &&
                (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            DangKyHoc? myReg = await _uow.DangKyHocs.Query()
                .Where(d => d.MaHocVien == student.MaHocVien && d.MaLop == id && d.TrangThai != "Đã hủy")
                .OrderByDescending(d => d.MaDangKy)
                .FirstOrDefaultAsync();

            var similarCourses = await _uow.LopHocs.Query()
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
                return Json(new { success = false, requireLogin = true, message = "Bạn cần đăng nhập hoặc đăng ký tài khoản học viên để thực hiện đăng ký khóa học." });
            }

            var lop = await _lopHocService.GetByIdAsync(maLop);
            if (lop == null)
            {
                return Json(new { success = false, message = "Lớp học không tồn tại." });
            }

            if (lop.TrangThai != "Đang mở đăng ký" && lop.TrangThai != "Mở đăng ký")
            {
                return Json(new { success = false, message = "Lớp học này hiện không mở tuyển sinh." });
            }

            if (lop.HanDangKy < DateTime.Today)
            {
                return Json(new { success = false, message = "Lớp học đã hết thời hạn đăng ký." });
            }

            var existing = await _uow.DangKyHocs.Query()
                .FirstOrDefaultAsync(d => d.MaHocVien == student.MaHocVien && d.MaLop == maLop && d.TrangThai != "Đã hủy");

            if (existing != null)
            {
                return Json(new { success = false, message = $"Bạn đã đăng ký khóa học này rồi (Trạng thái: {existing.TrangThai})." });
            }

            int enrolledCount = await _uow.DangKyHocs.CountAsync(d =>
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

            await _uow.DangKyHocs.AddAsync(dangKy);
            await _uow.CompleteAsync();

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

            var dk = await _dangKyHocService.GetDetailsAsync(maDangKy);
            if (dk == null) return Json(new { success = false, message = "Không tìm thấy hồ sơ đăng ký." });

            if (dk.MaHocVien != student.MaHocVien)
            {
                return Json(new { success = false, message = "Bạn không có quyền thao tác trên hồ sơ của người khác." });
            }

            if (dk.TrangThai == "Đã đóng học phí" || dk.TrangThai == "Đang học" || dk.TrangThai == "Hoàn thành")
            {
                return Json(new { success = false, message = $"Không thể hủy lớp khi đã ở trạng thái '{dk.TrangThai}'. Vui lòng liên hệ ban giáo vụ để được hỗ trợ bảo lưu." });
            }

            dk.TrangThai = "Đã hủy";
            _uow.DangKyHocs.Update(dk);
            await _uow.CompleteAsync();

            return Json(new { success = true, message = $"Đã hủy đăng ký lớp '{dk.LopHoc?.TenLop}' thành công." });
        }

        // --- THỜI KHÓA BIỂU THEO TUẦN (WEEKLY TIMETABLE MATRIX CHUẨN UNETI) ---
        public async Task<IActionResult> ThoiKhoaBieu(int? maLop, DateTime? ngay, string loaiLich = "tat_ca")
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập tài khoản học viên để xem thời khóa biểu.";
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Request.Path + Request.QueryString });
            }

            ViewBag.CurrentStudent = student;

            var myEnrolled = await _uow.DangKyHocs.Query()
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(d => d.MaHocVien == student.MaHocVien && 
                           (d.TrangThai == "Đang học" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đã duyệt"))
                .ToListAsync();

            ViewBag.MyEnrolledClasses = myEnrolled;
            var activeClassIds = myEnrolled.Select(d => d.MaLop).ToList();

            DateTime targetDate = ngay ?? new DateTime(2026, 10, 22);

            if (maLop.HasValue && !ngay.HasValue)
            {
                var nearSession = await _uow.BuoiHocs.Query()
                    .Where(b => b.MaLop == maLop.Value && b.TrangThai != "Đã hủy")
                    .OrderBy(b => Math.Abs(EF.Functions.DateDiffDay(b.ThoiGianBatDau, DateTime.Today)))
                    .FirstOrDefaultAsync();

                if (nearSession != null)
                {
                    targetDate = nearSession.ThoiGianBatDau.Date;
                }
            }

            int diff = (7 + (targetDate.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime monday = targetDate.Date.AddDays(-diff);
            DateTime sunday = monday.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            var weekDays = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                weekDays.Add(monday.AddDays(i));
            }

            ViewBag.WeekDays = weekDays;
            ViewBag.Monday = monday;
            ViewBag.Sunday = sunday;
            ViewBag.SelectedDate = targetDate;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedLoaiLich = loaiLich;

            LopHoc? selectedLop = null;
            if (maLop.HasValue)
            {
                selectedLop = await _lopHocService.GetByIdAsync(maLop.Value);
            }
            ViewBag.SelectedLop = selectedLop;

            var query = _uow.BuoiHocs.Query()
                .Include(b => b.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(b => b.ThoiGianBatDau >= monday && b.ThoiGianBatDau <= sunday && b.TrangThai != "Đã hủy");

            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(b => b.MaLop == maLop.Value);
            }
            else
            {
                query = query.Where(b => activeClassIds.Contains(b.MaLop));
            }

            if (loaiLich == "lich_hoc")
            {
                query = query.Where(b => !b.NoiDung.ToLower().Contains("kiểm tra") && !b.NoiDung.ToLower().Contains("thi "));
            }
            else if (loaiLich == "lich_thi")
            {
                query = query.Where(b => b.NoiDung.ToLower().Contains("kiểm tra") || b.NoiDung.ToLower().Contains("thi "));
            }

            var sessions = await query
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();

            return View(sessions);
        }

        // --- TIẾN TRÌNH HỌC TẬP & BẢNG ĐIỂM (PERSONALIZED DASHBOARD) ---
        public async Task<IActionResult> TienTrinh()
        {
            var student = await GetCurrentHocVienAsync();
            if (student == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập hoặc đăng ký tài khoản học viên để truy cập không gian học tập và theo dõi tiến trình.";
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = "/CongHocVien/TienTrinh" });
            }

            ViewBag.CurrentStudent = student;

            var dangKys = await _uow.DangKyHocs.Query()
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(d => d.MaHocVien == student.MaHocVien)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            var activeClassIds = dangKys
                .Where(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học")
                .Select(d => d.MaLop)
                .ToList();

            var buoiHocs = await _uow.BuoiHocs.Query()
                .Include(b => b.LopHoc)
                .Where(b => activeClassIds.Contains(b.MaLop) && b.TrangThai != "Đã hủy")
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();

            var completedSessionsCount = await _uow.BuoiHocs.Query()
                .Where(b => activeClassIds.Contains(b.MaLop) && b.TrangThai == "Đã hoàn thành")
                .GroupBy(b => b.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            var ketQuas = await _ketQuaService.GetByHocVienAsync(student.MaHocVien);

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
                TempData["Warning"] = "Vui lòng đăng nhập hoặc đăng ký tài khoản học viên để xem thông tin hồ sơ và lịch sử học phí.";
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = "/CongHocVien/HoSo" });
            }

            var tk = await _taiKhoanService.GetByIdAsync(student.MaTaiKhoan);
            ViewBag.TaiKhoan = tk;

            var paymentHistory = await _uow.DangKyHocs.Query()
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

            var result = await _hocVienService.UpdateStudentAsync(student);

            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }

            HttpContext.Session.SetString("HoTen", student.HoTen);

            return Json(new { success = true, message = "Cập nhật thông tin hồ sơ cá nhân thành công!" });
        }
    }
}
