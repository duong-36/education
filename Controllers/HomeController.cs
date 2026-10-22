// Họ và tên: Nhóm 5 (Vũ Đình Trường - Nguyễn Đình Kiên - Nguyễn Đức Linh - Nguyễn Hoàng Long - Nguyễn Tùng Dương)
// Lớp: DHTI17A1HN - Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI)
// Nội dung thực hiện: Trang chủ Nền tảng Thương mại Giáo dục UNETI EDUSPACE (EdTech Commercial Homepage)

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly IChuongTrinhDaoTaoService _chuongTrinhService;
        private readonly ILopHocService _lopHocService;
        private readonly IHocVienService _hocVienService;

        public HomeController(
            IUnitOfWork uow,
            IChuongTrinhDaoTaoService chuongTrinhService,
            ILopHocService lopHocService,
            IHocVienService hocVienService)
        {
            _uow = uow;
            _chuongTrinhService = chuongTrinhService;
            _lopHocService = lopHocService;
            _hocVienService = hocVienService;
        }

        // --- TRANG CHỦ NỀN TẢNG THƯƠNG MẠI GIÁO DỤC (EDTECH STOREFRONT HOME) ---
        public async Task<IActionResult> Index()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro == "Admin" || vaiTro == "Giáo vụ")
            {
                return RedirectToAction("Dashboard", "ThongKe");
            }

            // 1. Danh sách chương trình đào tạo & đếm số lớp đang mở
            var chuongTrinhs = await _uow.ChuongTrinhDaoTaos.Query()
                .AsNoTracking()
                .Where(c => c.TrangThai == "Đang hoạt động")
                .ToListAsync();

            var openClassCounts = await _uow.LopHocs.Query()
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .GroupBy(l => l.MaChuongTrinh)
                .Select(g => new { MaChuongTrinh = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaChuongTrinh, x => x.Count);

            ViewBag.ChuongTrinhs = chuongTrinhs;
            ViewBag.OpenClassCounts = openClassCounts;

            // 2. Các khóa học nổi bật / Bán chạy nhất (Top 6 lớp)
            var hotCourses = await _uow.LopHocs.Query()
                .Include(l => l.ChuongTrinhDaoTao)
                .AsNoTracking()
                .Where(l => l.TrangThai == "Đang mở đăng ký")
                .OrderByDescending(l => l.MaLop)
                .Take(6)
                .ToListAsync();

            // 3. Các khóa học sắp khai giảng (Top 4 lớp)
            var upcomingCourses = await _uow.LopHocs.Query()
                .Include(l => l.ChuongTrinhDaoTao)
                .AsNoTracking()
                .Where(l => l.TrangThai == "Đang mở đăng ký" && l.NgayKhaiGiang >= DateTime.Today)
                .OrderBy(l => l.NgayKhaiGiang)
                .Take(4)
                .ToListAsync();

            // Sĩ số đã duyệt cho từng lớp hiển thị
            var allCourseIds = hotCourses.Select(c => c.MaLop).Union(upcomingCourses.Select(c => c.MaLop)).Distinct().ToList();
            var siSoMap = await _uow.DangKyHocs.Query()
                .Where(d => allCourseIds.Contains(d.MaLop) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"))
                .GroupBy(d => d.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            ViewBag.HotCourses = hotCourses;
            ViewBag.UpcomingCourses = upcomingCourses;
            ViewBag.SiSoMap = siSoMap;

            // 4. Thống kê số liệu thực tế của trung tâm
            ViewBag.TotalCourses = await _uow.LopHocs.CountAsync();
            ViewBag.TotalStudents = await _uow.HocViens.CountAsync();
            ViewBag.TotalRegistrations = await _uow.DangKyHocs.CountAsync(d => d.TrangThai != "Đã hủy");

            // 5. Kiểm tra học viên đang đăng nhập (Personalized Learning Widget)
            int? maHocVien = HttpContext.Session.GetInt32("MaHocVien");
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            HocVien? student = null;
            if (maHocVien.HasValue)
            {
                student = await _hocVienService.GetByIdAsync(maHocVien.Value);
            }
            else if (maTaiKhoan.HasValue)
            {
                student = await _hocVienService.GetByTaiKhoanIdAsync(maTaiKhoan.Value);
            }

            ViewBag.CurrentStudent = student;
            var myRegistrations = new Dictionary<int, DangKyHoc>();
            BuoiHoc? nextSession = null;
            DangKyHoc? activeCourse = null;
            int totalSessions = 0;
            int completedSessions = 0;

            if (student != null)
            {
                var dks = await _uow.DangKyHocs.Query()
                    .Include(d => d.LopHoc)
                    .Where(d => d.MaHocVien == student.MaHocVien && d.TrangThai != "Đã hủy")
                    .ToListAsync();

                myRegistrations = dks.GroupBy(d => d.MaLop)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(d => d.MaDangKy).First());

                activeCourse = dks.FirstOrDefault(d => d.TrangThai == "Đang học" || d.TrangThai == "Đã đóng học phí")
                             ?? dks.FirstOrDefault();

                if (activeCourse != null)
                {
                    nextSession = await _uow.BuoiHocs.Query()
                        .Where(b => b.MaLop == activeCourse.MaLop && b.ThoiGianBatDau >= DateTime.Now && b.TrangThai != "Đã hủy")
                        .OrderBy(b => b.ThoiGianBatDau)
                        .FirstOrDefaultAsync();

                    totalSessions = activeCourse.LopHoc?.SoBuoiHoc ?? 1;
                    completedSessions = await _uow.BuoiHocs.CountAsync(b => b.MaLop == activeCourse.MaLop && b.TrangThai == "Đã hoàn thành");
                }
            }

            ViewBag.MyRegistrations = myRegistrations;
            ViewBag.ActiveCourse = activeCourse;
            ViewBag.NextSession = nextSession;
            ViewBag.TotalSessions = totalSessions;
            ViewBag.CompletedSessions = completedSessions;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
