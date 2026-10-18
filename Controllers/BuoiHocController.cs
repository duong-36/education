// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Xử lý nghiệp vụ xếp lịch học, CRUD và thuật toán chống trùng lịch (Module 5)

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
    public class BuoiHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BuoiHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? maLop, string? phong, string? trangThai, string? search, int page = 1)
        {
            int pageSize = 8;
            var query = _context.BuoiHocs
                .Include(b => b.LopHoc)
                .ThenInclude(l => l.ChuongTrinhDaoTao)
                .AsQueryable();

            // 1. Kiểm tra xung đột trùng lịch bằng LINQ trên các ca học chưa hủy
            var allActive = await _context.BuoiHocs.Where(b => b.TrangThai != "Đã hủy").ToListAsync();
            var conflictIds = new HashSet<int>();
            for (int i = 0; i < allActive.Count; i++)
            {
                for (int j = i + 1; j < allActive.Count; j++)
                {
                    var b1 = allActive[i];
                    var b2 = allActive[j];
                    if ((b1.PhongHoc == b2.PhongHoc || b1.GiangVien == b2.GiangVien || b1.MaLop == b2.MaLop) &&
                        (b1.ThoiGianBatDau < b2.ThoiGianKetThuc && b1.ThoiGianKetThuc > b2.ThoiGianBatDau))
                    {
                        conflictIds.Add(b1.MaBuoiHoc);
                        conflictIds.Add(b2.MaBuoiHoc);
                    }
                }
            }
            ViewBag.ConflictIds = conflictIds;
            ViewBag.SoLoiTrung = conflictIds.Count;

            // 2. Thống kê KPI tổng quan (3 Card đầu trang)
            ViewBag.TotalSessions = await _context.BuoiHocs.CountAsync();
            ViewBag.CompletedSessions = await _context.BuoiHocs.CountAsync(b => b.TrangThai == "Đã hoàn thành");
            ViewBag.ScheduledSessions = await _context.BuoiHocs.CountAsync(b => b.TrangThai == "Đã lên lịch");
            ViewBag.CanceledSessions = await _context.BuoiHocs.CountAsync(b => b.TrangThai == "Đã hủy");

            // 3. Áp dụng bộ lọc
            if (maLop.HasValue && maLop.Value > 0)
            {
                query = query.Where(b => b.MaLop == maLop.Value);
            }

            if (!string.IsNullOrWhiteSpace(phong))
            {
                query = query.Where(b => b.PhongHoc == phong);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(b => b.TrangThai == trangThai);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(b => (b.GiangVien != null && b.GiangVien.ToLower().Contains(s)) ||
                                         (b.PhongHoc != null && b.PhongHoc.ToLower().Contains(s)) ||
                                         (b.NoiDung != null && b.NoiDung.ToLower().Contains(s)) ||
                                         (b.LopHoc.TenLop != null && b.LopHoc.TenLop.ToLower().Contains(s)));
            }

            // 4. Phân trang
            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var buoiHocs = await query
                .OrderByDescending(b => b.ThoiGianBatDau)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedPhong = phong;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.Search = search;

            // 5. Dữ liệu nạp Dropdowns & Thời khóa biểu tuần
            ViewBag.LopHocList = await _context.LopHocs.ToListAsync();
            var allSessions = await _context.BuoiHocs.Include(b => b.LopHoc).ToListAsync();
            ViewBag.AllBuoiHocs = allSessions;
            ViewBag.SoThi = allSessions.Count(b => b.NoiDung != null && (b.NoiDung.ToLower().Contains("thi") || b.NoiDung.ToLower().Contains("kiểm tra")));
            ViewBag.SoChinhKhoa = allSessions.Count(b => b.TrangThai != "Đã hủy");
            ViewBag.DanhSachPhong = allSessions.Select(b => b.PhongHoc).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            ViewBag.DanhSachGiangVien = allSessions.Select(b => b.GiangVien).Where(g => !string.IsNullOrEmpty(g)).Distinct().ToList();

            return View(buoiHocs);
        }

        public IActionResult Create()
        {
            ViewBag.LopHocList = _context.LopHocs.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BuoiHoc buoiHoc)
        {
            // 1. Kiểm tra lớp có ít nhất 1 đăng ký "Đã đóng học phí" không?
            bool coHocVienDongHocPhi = _context.DangKyHocs
                .Any(dk => dk.MaLop == buoiHoc.MaLop && dk.TrangThai == "Đã đóng học phí");
            
            if (!coHocVienDongHocPhi)
            {
                ModelState.AddModelError("", "Lớp này chưa có học viên nào đóng học phí, không được xếp lịch.");
            }

            // 2. Validate thời gian hợp lệ
            if (buoiHoc.ThoiGianKetThuc <= buoiHoc.ThoiGianBatDau)
            {
                ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }
            if (buoiHoc.ThoiGianBatDau < DateTime.Now.AddDays(-1))
            {
                ModelState.AddModelError("ThoiGianBatDau", "Thời gian học không được nằm trong quá khứ.");
            }

            // 3. Kiểm tra số buổi học vượt quy định
            var lopHoc = await _context.LopHocs.FindAsync(buoiHoc.MaLop);
            if (lopHoc != null)
            {
                int soBuoiChuaHuy = _context.BuoiHocs
                    .Count(b => b.MaLop == buoiHoc.MaLop && b.TrangThai != "Đã hủy");
                
                if (soBuoiChuaHuy >= lopHoc.SoBuoiHoc)
                {
                    ModelState.AddModelError("", $"Lớp này đã xếp đủ {lopHoc.SoBuoiHoc} buổi học theo quy định.");
                }
            }

            // 4. KIỂM TRA TRÙNG LỊCH BẰNG LINQ (Lớp, Phòng, Giảng viên)
            bool isTrungLich = _context.BuoiHocs.Any(b =>
                b.TrangThai != "Đã hủy" &&
                (b.MaLop == buoiHoc.MaLop || b.PhongHoc == buoiHoc.PhongHoc || b.GiangVien == buoiHoc.GiangVien) &&
                (buoiHoc.ThoiGianBatDau < b.ThoiGianKetThuc && buoiHoc.ThoiGianKetThuc > b.ThoiGianBatDau)
            );

            if (isTrungLich)
            {
                ModelState.AddModelError("", "Lịch bị trùng! Kiểm tra lại Giảng viên, Phòng học hoặc Lớp đã có lịch trong khung giờ này.");
            }

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(buoiHoc.TrangThai)) buoiHoc.TrangThai = "Đã lên lịch";
                _context.Add(buoiHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xếp lịch học thành công!";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["Error"] = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Index));
        }

        // --- CẬP NHẬT BUỔI HỌC TỪ SLIDE-OVER DRAWER ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(BuoiHoc buoiHoc)
        {
            var existing = await _context.BuoiHocs.FindAsync(buoiHoc.MaBuoiHoc);
            if (existing == null)
            {
                TempData["Error"] = "Không tìm thấy buổi học cần chỉnh sửa.";
                return RedirectToAction(nameof(Index));
            }

            if (buoiHoc.ThoiGianKetThuc <= buoiHoc.ThoiGianBatDau)
            {
                TempData["Error"] = "Thời gian kết thúc phải sau thời gian bắt đầu.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra trùng lịch ngoại trừ chính buổi học này
            bool isTrungLich = await _context.BuoiHocs.AnyAsync(b =>
                b.MaBuoiHoc != buoiHoc.MaBuoiHoc &&
                b.TrangThai != "Đã hủy" &&
                (b.MaLop == buoiHoc.MaLop || b.PhongHoc == buoiHoc.PhongHoc || b.GiangVien == buoiHoc.GiangVien) &&
                (buoiHoc.ThoiGianBatDau < b.ThoiGianKetThuc && buoiHoc.ThoiGianKetThuc > b.ThoiGianBatDau)
            );

            if (isTrungLich)
            {
                TempData["Error"] = "Cập nhật thất bại: Trùng lịch với ca học khác (trùng phòng, giảng viên hoặc lớp)!";
                return RedirectToAction(nameof(Index));
            }

            existing.MaLop = buoiHoc.MaLop;
            existing.PhongHoc = buoiHoc.PhongHoc;
            existing.GiangVien = buoiHoc.GiangVien;
            existing.ThoiGianBatDau = buoiHoc.ThoiGianBatDau;
            existing.ThoiGianKetThuc = buoiHoc.ThoiGianKetThuc;
            existing.NoiDung = buoiHoc.NoiDung;
            existing.GhiChu = buoiHoc.GhiChu;
            existing.TrangThai = buoiHoc.TrangThai ?? existing.TrangThai;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật buổi học thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- XÓA BUỔI HỌC TỪ DELETE MODAL ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var buoiHoc = await _context.BuoiHocs.FindAsync(id);
            if (buoiHoc == null)
            {
                TempData["Error"] = "Không tìm thấy buổi học để xóa.";
                return RedirectToAction(nameof(Index));
            }

            _context.BuoiHocs.Remove(buoiHoc);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa vĩnh viễn buổi học khỏi hệ thống.";
            return RedirectToAction(nameof(Index));
        }

        // --- CẬP NHẬT HOÀN THÀNH SAU BUỔI HỌC (Yêu cầu 9.3) ---
        [HttpPost]
        public async Task<IActionResult> HoanThanh(int id)
        {
            var buoiHoc = await _context.BuoiHocs.FindAsync(id);
            if (buoiHoc == null) return NotFound();

            if (buoiHoc.TrangThai == "Đã hủy")
            {
                TempData["Error"] = "Không thể hoàn thành buổi học đã hủy.";
                return RedirectToAction(nameof(Index));
            }
            
            if (buoiHoc.ThoiGianKetThuc > DateTime.Now)
            {
                TempData["Error"] = "Buổi học chưa kết thúc (hoặc chưa diễn ra), không thể đánh dấu hoàn thành.";
                return RedirectToAction(nameof(Index));
            }

            buoiHoc.TrangThai = "Đã hoàn thành";
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Đã cập nhật trạng thái hoàn thành thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Huy(int id)
        {
            var buoiHoc = await _context.BuoiHocs.FindAsync(id);
            if (buoiHoc == null) return NotFound();

            buoiHoc.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Đã hủy ca học thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
