// Họ và tên: Nguyễn Hoàng Long
// Mã sinh viên: 23103100046
// Nội dung thực hiện: Tiếp nhận, Xét duyệt đăng ký & Thu học phí (Module 4) - Service Layer

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Implementations
{
    public class DangKyHocService : IDangKyHocService
    {
        private readonly IUnitOfWork _uow;

        public DangKyHocService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DangKyHocSearchResultVM> GetFilteredRegistrationsAsync(
            string search = "",
            int? maChuongTrinh = null,
            int? maLop = null,
            string trangThai = "",
            DateTime? tuNgay = null,
            DateTime? denNgay = null,
            string sortBy = "",
            int page = 1,
            int pageSize = 10)
        {
            var query = _uow.DangKyHocs.Query()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(d =>
                    (d.HocVien != null && d.HocVien.HoTen.ToLower().Contains(kw)) ||
                    (d.HocVien != null && d.HocVien.Email.ToLower().Contains(kw)) ||
                    (d.HocVien != null && d.HocVien.SoDienThoai.Contains(kw)) ||
                    (d.LopHoc != null && d.LopHoc.TenLop.ToLower().Contains(kw)) ||
                    ("dk" + d.MaDangKy.ToString()).Contains(kw));
            }

            if (maChuongTrinh.HasValue)
            {
                query = query.Where(d => d.LopHoc != null && d.LopHoc.MaChuongTrinh == maChuongTrinh.Value);
            }

            if (maLop.HasValue)
            {
                query = query.Where(d => d.MaLop == maLop.Value);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(d => d.TrangThai == trangThai);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(d => d.NgayDangKy >= tuNgay.Value.Date);
            }
            if (denNgay.HasValue)
            {
                DateTime end = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayDangKy <= end);
            }

            query = sortBy switch
            {
                "ngay_asc" => query.OrderBy(d => d.NgayDangKy),
                "ten_asc" => query.OrderBy(d => d.HocVien != null ? d.HocVien.HoTen : ""),
                "ten_desc" => query.OrderByDescending(d => d.HocVien != null ? d.HocVien.HoTen : ""),
                _ => query.OrderByDescending(d => d.NgayDangKy)
            };

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new DangKyHocSearchResultVM
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<DangKyHocKpiVM> GetKpiAsync()
        {
            int choDuyet = await _uow.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ duyệt");
            int daDuyet = await _uow.DangKyHocs.CountAsync(d => d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học");
            double tongDoanhThu = await _uow.DangKyHocs.Query().SumAsync(d => (double?)d.SoTienDaDong) ?? 0;

            return new DangKyHocKpiVM
            {
                ChoDuyetCount = choDuyet,
                DaDuyetCount = daDuyet,
                TongDoanhThu = tongDoanhThu
            };
        }

        public async Task<DangKyHoc?> GetDetailsAsync(int id)
        {
            return await _uow.DangKyHocs.GetDetailsAsync(id);
        }

        public async Task<List<DangKyHoc>> GetByHocVienAsync(int maHocVien)
        {
            var list = await _uow.DangKyHocs.GetByHocVienAsync(maHocVien);
            return list.ToList();
        }

        public async Task<List<DangKyHoc>> GetEligibleForGradingAsync()
        {
            return await _uow.DangKyHocs.Query()
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                .Where(d => d.TrangThai == "Đang học" || d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí")
                .ToListAsync();
        }

        public async Task<(bool Success, string Message, int? DangKyId)> RegisterClassAsync(int maHocVien, int maLop, string? ghiChu = null)
        {
            var existing = await _uow.DangKyHocs.FindRegistrationAsync(maHocVien, maLop);
            if (existing != null)
            {
                return (false, "Bạn đã đăng ký khóa học này trước đó rồi (Trạng thái: " + existing.TrangThai + ").", existing.MaDangKy);
            }

            var lop = await _uow.LopHocs.GetByIdAsync(maLop);
            if (lop == null)
            {
                return (false, "Khóa học không tồn tại trong hệ thống.", null);
            }

            if (lop.TrangThai != "Mở đăng ký" && lop.TrangThai != "Đang mở đăng ký")
            {
                return (false, $"Khóa học hiện đang ở trạng thái '{lop.TrangThai}', không nhận thêm đăng ký mới.", null);
            }

            if (lop.HanDangKy < DateTime.Today)
            {
                return (false, "Khóa học này đã hết hạn nhận đăng ký từ ngày " + lop.HanDangKy.ToString("dd/MM/yyyy") + ".", null);
            }

            int currentCount = await _uow.DangKyHocs.CountAsync(d => d.MaLop == maLop && (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));
            if (currentCount >= lop.SiSoToiDa)
            {
                return (false, $"Rất tiếc! Khóa học đã đạt sĩ số tối đa ({currentCount}/{lop.SiSoToiDa} học viên).", null);
            }

            var dk = new DangKyHoc
            {
                MaHocVien = maHocVien,
                MaLop = maLop,
                NgayDangKy = DateTime.Now,
                TrangThai = "Chờ duyệt",
                SoTienDaDong = 0,
                NhanXetGiaoVu = string.IsNullOrWhiteSpace(ghiChu) ? "Học viên đăng ký qua Cổng học viên." : ghiChu
            };

            await _uow.DangKyHocs.AddAsync(dk);
            await _uow.CompleteAsync();

            return (true, "Đăng ký khóa học thành công! Vui lòng chờ Giáo vụ xét duyệt hồ sơ.", dk.MaDangKy);
        }

        public async Task<(bool Success, string Message)> ApproveRegistrationAsync(int id, string? nhanXet = null)
        {
            var dk = await _uow.DangKyHocs.Query()
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dk == null) return (false, "Không tìm thấy đơn đăng ký.");
            var lop = dk.LopHoc;
            if (lop == null) return (false, "Lớp học liên kết không tồn tại.");

            int enrolledCount = await _uow.DangKyHocs.CountAsync(d =>
                d.MaLop == lop.MaLop &&
                d.MaDangKy != id &&
                (d.TrangThai == "Đã duyệt" || d.TrangThai == "Đã đóng học phí" || d.TrangThai == "Đang học"));

            if (enrolledCount >= lop.SiSoToiDa)
            {
                return (false, $"Không thể duyệt đơn này! Lớp '{lop.TenLop}' đã đủ chỉ tiêu sĩ số ({enrolledCount}/{lop.SiSoToiDa} học viên).");
            }

            dk.TrangThai = "Đã duyệt";
            dk.NgayXuLy = DateTime.Now;
            dk.NhanXetGiaoVu = string.IsNullOrWhiteSpace(nhanXet) ? "Đã duyệt hồ sơ đủ điều kiện nhập học." : nhanXet;

            if (enrolledCount + 1 >= lop.SiSoToiDa)
            {
                lop.TrangThai = "Đã đóng đăng ký";
                _uow.LopHocs.Update(lop);
            }

            _uow.DangKyHocs.Update(dk);
            await _uow.CompleteAsync();

            return (true, $"Đã duyệt hồ sơ thành công cho học viên! (Sĩ số hiện tại: {enrolledCount + 1}/{lop.SiSoToiDa})");
        }

        public async Task<(bool Success, string Message)> RejectRegistrationAsync(int id, string lyDo)
        {
            var dk = await _uow.DangKyHocs.GetByIdAsync(id);
            if (dk == null) return (false, "Không tìm thấy hồ sơ đăng ký.");

            dk.TrangThai = "Từ chối";
            dk.NgayXuLy = DateTime.Now;
            dk.NhanXetGiaoVu = string.IsNullOrWhiteSpace(lyDo) ? "Hồ sơ không đáp ứng yêu cầu tuyển sinh." : lyDo;

            _uow.DangKyHocs.Update(dk);
            await _uow.CompleteAsync();

            return (true, "Đã từ chối đơn đăng ký.");
        }

        public async Task<(bool Success, string Message)> ProcessPaymentAsync(int id, double soTien, string? ghiChu = null)
        {
            var dk = await _uow.DangKyHocs.Query()
                .Include(d => d.LopHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dk == null) return (false, "Không tìm thấy hồ sơ.");

            if (soTien <= 0)
            {
                return (false, "Số tiền đóng học phí phải lớn hơn 0 VNĐ.");
            }

            double hocPhiGoc = dk.LopHoc?.HocPhi ?? 0;
            if (hocPhiGoc > 0 && soTien > hocPhiGoc)
            {
                return (false, $"Số tiền nộp ({soTien:N0} đ) vượt quá mức học phí của lớp ({hocPhiGoc:N0} đ).");
            }

            dk.SoTienDaDong = soTien;
            dk.NgayDongHocPhi = DateTime.Now;

            if (soTien >= hocPhiGoc)
            {
                dk.TrangThai = "Đã đóng học phí";
            }

            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                dk.NhanXetGiaoVu = (string.IsNullOrWhiteSpace(dk.NhanXetGiaoVu) ? "" : dk.NhanXetGiaoVu + " | ") + "Thu phí: " + ghiChu;
            }

            _uow.DangKyHocs.Update(dk);
            await _uow.CompleteAsync();

            return (true, $"Ghi nhận thu {soTien:N0} đ thành công! Trạng thái: {dk.TrangThai}");
        }

        public async Task<(bool Success, string Message)> StartLearningAsync(int id)
        {
            var dk = await _uow.DangKyHocs.GetByIdAsync(id);
            if (dk == null) return (false, "Không tìm thấy hồ sơ.");

            dk.TrangThai = "Đang học";
            _uow.DangKyHocs.Update(dk);
            await _uow.CompleteAsync();

            return (true, "Đã cập nhật trạng thái học viên sang 'Đang học'!");
        }

        public async Task<(bool Success, string Message)> DeleteRegistrationAsync(int id)
        {
            var dk = await _uow.DangKyHocs.GetByIdAsync(id);
            if (dk == null) return (false, "Không tìm thấy đơn đăng ký.");

            if (dk.SoTienDaDong > 0)
            {
                return (false, $"Không thể xóa đơn đăng ký #{dk.MaDangKy} vì học viên đã đóng {dk.SoTienDaDong:N0} đ. Vui lòng hoàn phí hoặc chuyển sang trạng thái 'Đã hủy'.");
            }

            _uow.DangKyHocs.Delete(dk);
            await _uow.CompleteAsync();

            return (true, "Đã xóa đơn đăng ký thành công.");
        }
    }
}
