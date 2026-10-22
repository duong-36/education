// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Nội dung thực hiện: Module 1 - Tài khoản, Đăng nhập, Phân quyền Session (Service Layer)

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
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly IUnitOfWork _uow;

        public TaiKhoanService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<(bool Success, string Message, TaiKhoan? User, HocVien? HocVien)> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return (false, "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", null, null);
            }

            var user = await _uow.TaiKhoans.Query()
                .FirstOrDefaultAsync(t => t.TenDangNhap == username.Trim() && t.MatKhau == password);

            if (user == null)
            {
                return (false, "Tên đăng nhập hoặc mật khẩu không chính xác.", null, null);
            }

            if (user.TrangThai == "Bị khóa")
            {
                return (false, "Tài khoản này hiện đang bị khóa. Vui lòng liên hệ Admin để được hỗ trợ.", null, null);
            }

            HocVien? hv = null;
            if (user.VaiTro == "Học viên")
            {
                hv = await _uow.HocViens.GetByTaiKhoanIdAsync(user.MaTaiKhoan);
            }

            return (true, "Đăng nhập thành công.", user, hv);
        }

        public async Task<TaiKhoan?> GetQuickLoginUserAsync(string role)
        {
            if (role == "Admin")
            {
                return await _uow.TaiKhoans.Query().FirstOrDefaultAsync(t => t.VaiTro == "Admin" && t.TrangThai == "Đang hoạt động");
            }
            if (role == "GiaoVu")
            {
                return await _uow.TaiKhoans.Query().FirstOrDefaultAsync(t => t.VaiTro == "Giáo vụ" && t.TrangThai == "Đang hoạt động");
            }
            return await _uow.TaiKhoans.Query().FirstOrDefaultAsync(t => t.VaiTro == "Học viên" && t.TrangThai == "Đang hoạt động");
        }

        public async Task<(bool Success, string Message, TaiKhoan? User, HocVien? HocVien)> RegisterStudentAsync(RegisterViewModel model)
        {
            string username = model.TenDangNhap.Trim();
            string email = model.Email.Trim().ToLower();

            if (await _uow.TaiKhoans.IsUsernameTakenAsync(username))
            {
                return (false, "Tên đăng nhập này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.", null, null);
            }

            if (await _uow.TaiKhoans.IsEmailTakenAsync(email))
            {
                return (false, "Địa chỉ email này đã được sử dụng cho một tài khoản khác.", null, null);
            }

            await _uow.BeginTransactionAsync();
            try
            {
                var tk = new TaiKhoan
                {
                    TenDangNhap = username,
                    MatKhau = model.MatKhau,
                    HoTen = model.HoTen.Trim(),
                    Email = email,
                    VaiTro = "Học viên",
                    TrangThai = "Đang hoạt động"
                };

                await _uow.TaiKhoans.AddAsync(tk);
                await _uow.CompleteAsync();

                var hv = new HocVien
                {
                    MaTaiKhoan = tk.MaTaiKhoan,
                    HoTen = model.HoTen.Trim(),
                    Email = email,
                    SoDienThoai = model.SoDienThoai?.Trim() ?? "",
                    NgaySinh = model.NgaySinh ?? DateTime.Today.AddYears(-20),
                    GioiTinh = string.IsNullOrWhiteSpace(model.GioiTinh) ? "Nam" : model.GioiTinh,
                    DiaChi = string.IsNullOrWhiteSpace(model.DiaChi) ? "Hà Nội" : model.DiaChi.Trim(),
                    TrinhDoHocVan = "Đại học",
                    NgheNghiep = "Học viên",
                    TrangThai = "Đang học"
                };

                await _uow.HocViens.AddAsync(hv);
                await _uow.CompleteAsync();

                await _uow.CommitTransactionAsync();
                return (true, "Đăng ký thành công!", tk, hv);
            }
            catch (Exception ex)
            {
                await _uow.RollbackTransactionAsync();
                return (false, "Lỗi trong quá trình tạo tài khoản: " + ex.Message, null, null);
            }
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(int maTaiKhoan, string oldPass, string newPass)
        {
            var user = await _uow.TaiKhoans.GetByIdAsync(maTaiKhoan);
            if (user == null) return (false, "Không tìm thấy tài khoản.");

            if (user.MatKhau != oldPass)
            {
                return (false, "Mật khẩu hiện tại không chính xác.");
            }

            user.MatKhau = newPass;
            _uow.TaiKhoans.Update(user);
            await _uow.CompleteAsync();

            return (true, "Đổi mật khẩu thành công!");
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(int maTaiKhoan, string hoTen, string email)
        {
            var user = await _uow.TaiKhoans.GetByIdAsync(maTaiKhoan);
            if (user == null) return (false, "Không tìm thấy tài khoản.");

            string newEmail = email.Trim().ToLower();
            if (await _uow.TaiKhoans.IsEmailTakenAsync(newEmail, maTaiKhoan))
            {
                return (false, "Email này đã được sử dụng bởi một tài khoản khác.");
            }

            user.HoTen = hoTen.Trim();
            user.Email = newEmail;
            _uow.TaiKhoans.Update(user);

            var hv = await _uow.HocViens.GetByTaiKhoanIdAsync(maTaiKhoan);
            if (hv != null)
            {
                hv.HoTen = user.HoTen;
                hv.Email = user.Email;
                _uow.HocViens.Update(hv);
            }

            await _uow.CompleteAsync();
            return (true, "Cập nhật thông tin cá nhân thành công!");
        }

        public async Task<TaiKhoan?> GetByIdAsync(int id)
        {
            return await _uow.TaiKhoans.GetByIdAsync(id);
        }

        public async Task<(List<TaiKhoan> Items, int TotalItems, int TotalPages, int CurrentPage, int TongTaiKhoan, int DangHoatDong, int BiKhoa)> GetFilteredAccountsAsync(
            int page = 1,
            int pageSize = 10,
            string search = "",
            string vaiTro = "",
            string trangThai = "")
        {
            var query = _uow.TaiKhoans.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(s) || 
                                         t.HoTen.ToLower().Contains(s) || 
                                         t.Email.ToLower().Contains(s));
            }

            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(t => t.TrangThai == trangThai);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(t => t.MaTaiKhoan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int tongTaiKhoan = await _uow.TaiKhoans.CountAsync();
            int dangHoatDong = await _uow.TaiKhoans.CountAsync(t => t.TrangThai == "Đang hoạt động");
            int biKhoa = await _uow.TaiKhoans.CountAsync(t => t.TrangThai == "Bị khóa");

            return (items, totalItems, totalPages, page, tongTaiKhoan, dangHoatDong, biKhoa);
        }

        public async Task<(bool Success, string Message)> CreateAccountAsync(TaiKhoan tk)
        {
            if (string.IsNullOrWhiteSpace(tk.TenDangNhap) || string.IsNullOrWhiteSpace(tk.MatKhau))
            {
                return (false, "Tên đăng nhập và mật khẩu không được để trống.");
            }

            string username = tk.TenDangNhap.Trim();
            if (await _uow.TaiKhoans.IsUsernameTakenAsync(username))
            {
                return (false, "Tên đăng nhập này đã tồn tại trong hệ thống.");
            }

            tk.TenDangNhap = username;
            if (string.IsNullOrEmpty(tk.TrangThai)) tk.TrangThai = "Đang hoạt động";
            if (string.IsNullOrEmpty(tk.VaiTro)) tk.VaiTro = "Học viên";

            await _uow.TaiKhoans.AddAsync(tk);
            await _uow.CompleteAsync();

            return (true, "Thêm tài khoản mới thành công!");
        }

        public async Task<(bool Success, string Message)> EditAccountAsync(int maTaiKhoan, string hoTen, string email, string vaiTro, string trangThai, string? matKhauMoi)
        {
            var user = await _uow.TaiKhoans.GetByIdAsync(maTaiKhoan);
            if (user == null) return (false, "Không tìm thấy tài khoản cần chỉnh sửa.");

            user.HoTen = hoTen.Trim();
            user.Email = email.Trim();
            user.VaiTro = vaiTro;
            user.TrangThai = trangThai;
            if (!string.IsNullOrWhiteSpace(matKhauMoi))
            {
                user.MatKhau = matKhauMoi.Trim();
            }

            _uow.TaiKhoans.Update(user);
            await _uow.CompleteAsync();

            return (true, "Cập nhật tài khoản thành công!");
        }

        public async Task<(bool Success, string Message)> ToggleStatusAsync(int id)
        {
            var user = await _uow.TaiKhoans.GetByIdAsync(id);
            if (user == null) return (false, "Không tìm thấy tài khoản.");

            user.TrangThai = user.TrangThai == "Đang hoạt động" ? "Bị khóa" : "Đang hoạt động";
            _uow.TaiKhoans.Update(user);
            await _uow.CompleteAsync();

            return (true, $"Đã cập nhật trạng thái sang '{user.TrangThai}'!");
        }

        public async Task<(bool Success, string Message)> DeleteAccountAsync(int id)
        {
            var user = await _uow.TaiKhoans.GetByIdAsync(id);
            if (user == null) return (false, "Không tìm thấy tài khoản.");

            _uow.TaiKhoans.Delete(user);
            await _uow.CompleteAsync();

            return (true, "Xóa tài khoản thành công!");
        }
    }
}
