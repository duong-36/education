// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Nội dung thực hiện: Module 2 - Quản lý Lớp học, Tìm kiếm, Lọc đa tiêu chí, Sắp xếp, Phân trang LINQ

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
    public class LopHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LopHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- DANH SÁCH LỚP HỌC (SEARCH + FILTER + SORT + PAGINATION) ---
        public async Task<IActionResult> Index(
            int page = 1,
            string search = "",
            int? maChuongTrinh = null,
            string trinhDo = "",
            string trangThai = "",
            double? hocPhiMin = null,
            double? hocPhiMax = null,
            string hanDangKyStatus = "",
            string choTrongStatus = "",
            string sortBy = "")
        {
            int pageSize = 6;
            var query = _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .AsQueryable();

            // 1. Tìm kiếm LINQ (theo Tên lớp, Chương trình đào tạo, Giảng viên)
            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(l => l.TenLop.ToLower().Contains(s) ||
                                         (l.ChuongTrinhDaoTao != null && l.ChuongTrinhDaoTao.TenChuongTrinh.ToLower().Contains(s)) ||
                                         (l.GiangVien != null && l.GiangVien.ToLower().Contains(s)));
            }

            // 2. Lọc theo Chương trình đào tạo
            if (maChuongTrinh.HasValue && maChuongTrinh.Value > 0)
            {
                query = query.Where(l => l.MaChuongTrinh == maChuongTrinh.Value);
            }

            // 3. Lọc theo Trình độ đầu vào
            if (!string.IsNullOrWhiteSpace(trinhDo))
            {
                query = query.Where(l => l.TrinhDoDauVao == trinhDo);
            }

            // 4. Lọc theo Trạng thái lớp
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(l => l.TrangThai == trangThai);
            }

            // 5. Lọc theo Khoảng học phí
            if (hocPhiMin.HasValue && hocPhiMin.Value >= 0)
            {
                query = query.Where(l => l.HocPhi >= hocPhiMin.Value);
            }
            if (hocPhiMax.HasValue && hocPhiMax.Value > 0)
            {
                query = query.Where(l => l.HocPhi <= hocPhiMax.Value);
            }

            // 6. Lọc theo Hạn đăng ký (Còn hạn / Hết hạn)
            DateTime today = DateTime.Now;
            if (hanDangKyStatus == "con_han")
            {
                query = query.Where(l => l.HanDangKy >= today.Date && l.NgayBatDauDangKy <= today);
            }
            else if (hanDangKyStatus == "het_han")
            {
                query = query.Where(l => l.HanDangKy < today.Date);
            }

            // 7. Sắp xếp LINQ
            switch (sortBy)
            {
                case "ten_asc":
                    query = query.OrderBy(l => l.TenLop);
                    break;
                case "ten_desc":
                    query = query.OrderByDescending(l => l.TenLop);
                    break;
                case "han_asc":
                    query = query.OrderBy(l => l.HanDangKy);
                    break;
                case "han_desc":
                    query = query.OrderByDescending(l => l.HanDangKy);
                    break;
                case "hocphi_asc":
                    query = query.OrderBy(l => l.HocPhi);
                    break;
                case "hocphi_desc":
                    query = query.OrderByDescending(l => l.HocPhi);
                    break;
                case "khaigiang_asc":
                    query = query.OrderBy(l => l.NgayKhaiGiang);
                    break;
                case "khaigiang_desc":
                    query = query.OrderByDescending(l => l.NgayKhaiGiang);
                    break;
                default:
                    query = query.OrderByDescending(l => l.MaLop);
                    break;
            }

            // Lấy danh sách tạm để tính sĩ số thực tế (Hỗ trợ lọc Còn chỗ / Hết chỗ)
            var allFiltered = await query.ToListAsync();

            // Tính sĩ số đã duyệt/đóng học phí/đang học cho từng lớp
            var allLopIds = allFiltered.Select(l => l.MaLop).ToList();
            var siSoMap = await _context.DangKyHocs
                .Where(d => allLopIds.Contains(d.MaLop) && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"))
                .GroupBy(d => d.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            // 8. Lọc Còn chỗ / Hết chỗ
            if (choTrongStatus == "con_cho")
            {
                allFiltered = allFiltered.Where(l => (siSoMap.ContainsKey(l.MaLop) ? siSoMap[l.MaLop] : 0) < l.SiSoToiDa).ToList();
            }
            else if (choTrongStatus == "het_cho")
            {
                allFiltered = allFiltered.Where(l => (siSoMap.ContainsKey(l.MaLop) ? siSoMap[l.MaLop] : 0) >= l.SiSoToiDa).ToList();
            }

            // 9. Phân trang LINQ
            int totalItems = allFiltered.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = allFiltered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Đếm số buổi học đã xếp
            var buoiHocMap = await _context.BuoiHocs
                .Where(b => allLopIds.Contains(b.MaLop) && b.TrangThai != "Đã hủy")
                .GroupBy(b => b.MaLop)
                .Select(g => new { MaLop = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaLop, x => x.Count);

            // Gán ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.Search = search;
            ViewBag.SelectedMaChuongTrinh = maChuongTrinh;
            ViewBag.SelectedTrinhDo = trinhDo;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.HocPhiMin = hocPhiMin;
            ViewBag.HocPhiMax = hocPhiMax;
            ViewBag.HanDangKyStatus = hanDangKyStatus;
            ViewBag.ChoTrongStatus = choTrongStatus;
            ViewBag.SortBy = sortBy;

            ViewBag.SiSoMap = siSoMap;
            ViewBag.BuoiHocMap = buoiHocMap;
            ViewBag.ChuongTrinhList = await _context.ChuongTrinhDaoTaos.OrderBy(c => c.TenChuongTrinh).ToListAsync();

            // 3 thẻ KPI
            ViewBag.TongSoLop = await _context.LopHocs.CountAsync();
            ViewBag.LopDangMo = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang mở đăng ký");
            ViewBag.TongHocVienDangHoc = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Đang học" || d.TrangThai == "Đã đóng học phí");

            return View(items);
        }

        // --- CHI TIẾT LỚP HỌC ---
        public async Task<IActionResult> Details(int id)
        {
            var lop = await _context.LopHocs
                .Include(l => l.ChuongTrinhDaoTao)
                .FirstOrDefaultAsync(l => l.MaLop == id);

            if (lop == null) return NotFound();

            var buoiHocs = await _context.BuoiHocs
                .Where(b => b.MaLop == id)
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();

            var dangKys = await _context.DangKyHocs
                .Include(d => d.HocVien)
                .Where(d => d.MaLop == id)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            ViewBag.BuoiHocs = buoiHocs;
            ViewBag.DangKys = dangKys;
            ViewBag.SoHocVien = dangKys.Count(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");

            return View(lop);
        }

        // --- CREATE LỚP HỌC (DRAWER AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(LopHoc lop)
        {
            ModelState.Remove("ChuongTrinhDaoTao");
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // Kiểm tra ràng buộc nghiệp vụ (Mục 6.2)
            if (string.IsNullOrWhiteSpace(lop.TenLop))
            {
                if (isAjax) return Json(new { success = false, message = "Tên lớp học bắt buộc nhập." });
                TempData["Error"] = "Tên lớp học bắt buộc nhập.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.SiSoToiDa <= 0)
            {
                if (isAjax) return Json(new { success = false, message = "Sĩ số tối đa phải lớn hơn 0." });
                TempData["Error"] = "Sĩ số tối đa phải lớn hơn 0.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.SoBuoiHoc <= 0)
            {
                if (isAjax) return Json(new { success = false, message = "Số buổi học phải lớn hơn 0." });
                TempData["Error"] = "Số buổi học phải lớn hơn 0.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.HocPhi < 0)
            {
                if (isAjax) return Json(new { success = false, message = "Học phí phải lớn hơn hoặc bằng 0." });
                TempData["Error"] = "Học phí phải lớn hơn hoặc bằng 0.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.HanDangKy < lop.NgayBatDauDangKy)
            {
                if (isAjax) return Json(new { success = false, message = "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu đăng ký." });
                TempData["Error"] = "Hạn đăng ký không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (lop.NgayKhaiGiang < lop.HanDangKy)
            {
                if (isAjax) return Json(new { success = false, message = "Ngày khai giảng phải sau hoặc bằng hạn đăng ký." });
                TempData["Error"] = "Ngày khai giảng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(lop.TrangThai)) lop.TrangThai = "Chưa mở";
            if (string.IsNullOrEmpty(lop.TrinhDoDauVao)) lop.TrinhDoDauVao = "Cơ bản";

            _context.Add(lop);
            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Tạo lớp học mới thành công!" });
            TempData["Success"] = "Tạo lớp học mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- EDIT LỚP HỌC (DRAWER AJAX) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(
            int maLop,
            string tenLop,
            int maChuongTrinh,
            string? giangVien,
            string? trinhDoDauVao,
            int siSoToiDa,
            int soBuoiHoc,
            double hocPhi,
            DateTime ngayBatDauDangKy,
            DateTime hanDangKy,
            DateTime ngayKhaiGiang,
            string? moTa,
            string? yeuCauHocVien,
            string trangThai)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var lop = await _context.LopHocs.FindAsync(maLop);
            if (lop == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy lớp học cần cập nhật." });
                TempData["Error"] = "Không tìm thấy lớp học.";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra ràng buộc nghiệp vụ Mục 6.2: Không cho giảm sĩ số tối đa xuống thấp hơn số học viên đã duyệt
            int soHocVienDaDuyet = await _context.DangKyHocs
                .CountAsync(d => d.MaLop == maLop && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            if (siSoToiDa < soHocVienDaDuyet)
            {
                string msg = $"Không thể giảm sĩ số tối đa xuống {siSoToiDa} vì lớp đang có {soHocVienDaDuyet} học viên đã được duyệt/đóng học phí.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            if (hanDangKy < ngayBatDauDangKy)
            {
                if (isAjax) return Json(new { success = false, message = "Hạn đăng ký phải sau hoặc bằng ngày bắt đầu đăng ký." });
                TempData["Error"] = "Hạn đăng ký không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (ngayKhaiGiang < hanDangKy)
            {
                if (isAjax) return Json(new { success = false, message = "Ngày khai giảng phải sau hoặc bằng hạn đăng ký." });
                TempData["Error"] = "Ngày khai giảng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            lop.TenLop = tenLop.Trim();
            lop.MaChuongTrinh = maChuongTrinh;
            lop.GiangVien = giangVien ?? "Chưa xếp";
            lop.TrinhDoDauVao = trinhDoDauVao ?? "Cơ bản";
            lop.SiSoToiDa = siSoToiDa;
            lop.SoBuoiHoc = soBuoiHoc;
            lop.HocPhi = hocPhi;
            lop.NgayBatDauDangKy = ngayBatDauDangKy;
            lop.HanDangKy = hanDangKy;
            lop.NgayKhaiGiang = ngayKhaiGiang;
            lop.MoTa = moTa ?? "";
            lop.YeuCauHocVien = yeuCauHocVien ?? "";
            lop.TrangThai = trangThai;

            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = "Cập nhật thông tin lớp học thành công!" });
            TempData["Success"] = "Cập nhật thông tin lớp học thành công!";
            return RedirectToAction(nameof(Index));
        }

        // --- XÓA LỚP HỌC (AN TOÀN KIỂM SOÁT DỮ LIỆU) ---
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var lop = await _context.LopHocs.FindAsync(id);
            if (lop == null)
            {
                if (isAjax) return Json(new { success = false, message = "Không tìm thấy lớp học." });
                return RedirectToAction(nameof(Index));
            }

            // Quy định nghiệp vụ Mục 6.2: Khi lớp đã có đăng ký, việc xóa phải được kiểm soát
            int soDangKy = await _context.DangKyHocs.CountAsync(d => d.MaLop == id);
            if (soDangKy > 0)
            {
                string msg = $"Không thể xóa lớp '{lop.TenLop}' vì đã có {soDangKy} hồ sơ đăng ký của học viên liên quan. Bạn có thể chuyển trạng thái lớp sang 'Đã đóng đăng ký'.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra các buổi học đã xếp
            int soBuoi = await _context.BuoiHocs.CountAsync(b => b.MaLop == id);
            if (soBuoi > 0)
            {
                string msg = $"Không thể xóa lớp '{lop.TenLop}' vì đang có {soBuoi} buổi học trong lịch học. Vui lòng hủy các buổi học trước.";
                if (isAjax) return Json(new { success = false, message = msg });
                TempData["Error"] = msg;
                return RedirectToAction(nameof(Index));
            }

            _context.LopHocs.Remove(lop);
            await _context.SaveChangesAsync();

            if (isAjax) return Json(new { success = true, message = $"Đã xóa lớp học '{lop.TenLop}' thành công." });
            TempData["Success"] = "Đã xóa lớp học thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
