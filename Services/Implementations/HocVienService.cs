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
    public class HocVienService : IHocVienService
    {
        private readonly IUnitOfWork _uow;

        public HocVienService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<HocVienSearchResultVM> GetFilteredStudentsAsync(
            string search = "",
            string trangThai = "",
            string trinhDo = "",
            int page = 1,
            int pageSize = 10)
        {
            var query = _uow.HocViens.Query()
                .Include(h => h.TaiKhoan)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(h =>
                    h.HoTen.ToLower().Contains(kw) ||
                    h.Email.ToLower().Contains(kw) ||
                    h.SoDienThoai.Contains(kw) ||
                    ("hv" + h.MaHocVien.ToString()).Contains(kw));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(h => h.TrangThai == trangThai);
            }

            if (!string.IsNullOrWhiteSpace(trinhDo))
            {
                query = query.Where(h => h.TrinhDoHocVan == trinhDo);
            }

            var trinhDoList = await _uow.HocViens.Query()
                .Where(h => !string.IsNullOrEmpty(h.TrinhDoHocVan))
                .Select(h => h.TrinhDoHocVan)
                .Distinct()
                .ToListAsync();

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

            return new HocVienSearchResultVM
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                TrinhDoList = trinhDoList
            };
        }

        public async Task<HocVienKpiVM> GetKpiAsync()
        {
            int total = await _uow.HocViens.CountAsync();
            int dangHoc = await _uow.HocViens.CountAsync(h => h.TrangThai == "Đang học");
            int totNghiep = await _uow.HocViens.CountAsync(h => h.TrangThai == "Đã tốt nghiệp" || h.TrangThai == "Hoàn thành");

            return new HocVienKpiVM
            {
                TongSoHocVien = total,
                DangHoc = dangHoc,
                TotNghiep = totNghiep
            };
        }

        public async Task<HocVien?> GetByIdAsync(int id)
        {
            return await _uow.HocViens.GetByIdAsync(id);
        }

        public async Task<HocVien?> GetByTaiKhoanIdAsync(int maTaiKhoan)
        {
            return await _uow.HocViens.GetByTaiKhoanIdAsync(maTaiKhoan);
        }

        public async Task<HocVienDetailsVM?> GetDetailsWithRegistrationsAsync(int id)
        {
            var hv = await _uow.HocViens.GetWithTaiKhoanAsync(id);
            if (hv == null) return null;

            var dangKys = await _uow.DangKyHocs.Query()
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(d => d.MaHocVien == id)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            var ketQuas = await _uow.KetQuaHocTaps.Query()
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == id)
                .ToListAsync();

            return new HocVienDetailsVM
            {
                HocVien = hv,
                DangKys = dangKys,
                KetQuas = ketQuas
            };
        }

        public async Task<(bool Success, string Message, int? NewId, string? Username)> CreateStudentAsync(HocVien model, string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(model.HoTen))
            {
                return (false, "Họ tên học viên bắt buộc nhập.", null, null);
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return (false, "Email học viên bắt buộc nhập.", null, null);
            }

            bool emailExists = await _uow.HocViens.ExistsAsync(h => h.Email.ToLower() == model.Email.ToLower());
            if (emailExists)
            {
                return (false, "Email này đã được sử dụng bởi học viên khác.", null, null);
            }

            string username = string.IsNullOrWhiteSpace(tenDangNhap) ? model.Email.Split('@')[0] : tenDangNhap.Trim();
            if (await _uow.TaiKhoans.IsUsernameTakenAsync(username))
            {
                username = username + "_" + new Random().Next(100, 999);
            }

            string password = string.IsNullOrWhiteSpace(matKhau) ? "123456" : matKhau;

            await _uow.BeginTransactionAsync();
            try
            {
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = username,
                    MatKhau = password,
                    HoTen = model.HoTen,
                    Email = model.Email,
                    VaiTro = "Học viên",
                    TrangThai = "Đang hoạt động"
                };

                await _uow.TaiKhoans.AddAsync(taiKhoan);
                await _uow.CompleteAsync();

                model.MaTaiKhoan = taiKhoan.MaTaiKhoan;
                if (model.NgaySinh == default) model.NgaySinh = new DateTime(2000, 1, 1);
                if (string.IsNullOrWhiteSpace(model.TrangThai)) model.TrangThai = "Đang học";

                await _uow.HocViens.AddAsync(model);
                await _uow.CompleteAsync();

                await _uow.CommitTransactionAsync();
                return (true, $"Thêm học viên thành công! Tài khoản: {username}", model.MaHocVien, username);
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync();
                return (false, "Lỗi khi tạo học viên: " + ex.Message, null, null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateStudentAsync(HocVien model)
        {
            var existing = await _uow.HocViens.GetByIdAsync(model.MaHocVien);
            if (existing == null) return (false, "Học viên không tồn tại.");

            if (string.IsNullOrWhiteSpace(model.HoTen))
            {
                return (false, "Họ tên không được để trống.");
            }

            bool emailTaken = await _uow.HocViens.ExistsAsync(h => h.Email.ToLower() == model.Email.ToLower() && h.MaHocVien != model.MaHocVien);
            if (emailTaken)
            {
                return (false, "Email này đã được dùng bởi học viên khác.");
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

            if (existing.MaTaiKhoan > 0)
            {
                var tk = await _uow.TaiKhoans.GetByIdAsync(existing.MaTaiKhoan);
                if (tk != null)
                {
                    tk.HoTen = model.HoTen;
                    tk.Email = model.Email;
                    _uow.TaiKhoans.Update(tk);
                }
            }

            _uow.HocViens.Update(existing);
            await _uow.CompleteAsync();

            return (true, "Cập nhật học viên thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteStudentAsync(int id)
        {
            var hv = await _uow.HocViens.GetByIdAsync(id);
            if (hv == null) return (false, "Học viên không tồn tại.");

            bool hasDangKy = await _uow.DangKyHocs.ExistsAsync(d => d.MaHocVien == id);
            bool hasKetQua = await _uow.KetQuaHocTaps.ExistsAsync(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == id);

            if (hasDangKy || hasKetQua)
            {
                return (false, $"Không thể xóa học viên #{hv.MaHocVien} ({hv.HoTen}) vì đã có {(hasDangKy ? "đơn đăng ký học" : "kết quả học tập")} trong hệ thống. Vui lòng chuyển trạng thái sang 'Đã nghỉ' hoặc 'Bảo lưu'.");
            }

            int tkId = hv.MaTaiKhoan;
            _uow.HocViens.Delete(hv);

            if (tkId > 0)
            {
                var tk = await _uow.TaiKhoans.GetByIdAsync(tkId);
                if (tk != null)
                {
                    _uow.TaiKhoans.Delete(tk);
                }
            }

            await _uow.CompleteAsync();
            return (true, "Xóa học viên thành công!");
        }
    }
}
