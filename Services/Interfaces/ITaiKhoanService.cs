using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces
{
    public interface ITaiKhoanService
    {
        Task<(bool Success, string Message, TaiKhoan? User, HocVien? HocVien)> AuthenticateAsync(string username, string password);
        Task<TaiKhoan?> GetQuickLoginUserAsync(string role);
        Task<(bool Success, string Message, TaiKhoan? User, HocVien? HocVien)> RegisterStudentAsync(RegisterViewModel model);
        Task<(bool Success, string Message)> ChangePasswordAsync(int maTaiKhoan, string oldPass, string newPass);
        Task<(bool Success, string Message)> UpdateProfileAsync(int maTaiKhoan, string hoTen, string email);
        Task<TaiKhoan?> GetByIdAsync(int id);
        Task<(List<TaiKhoan> Items, int TotalItems, int TotalPages, int CurrentPage, int TongTaiKhoan, int DangHoatDong, int BiKhoa)> GetFilteredAccountsAsync(
            int page = 1,
            int pageSize = 10,
            string search = "",
            string vaiTro = "",
            string trangThai = "");
        Task<(bool Success, string Message)> CreateAccountAsync(TaiKhoan tk);
        Task<(bool Success, string Message)> EditAccountAsync(int maTaiKhoan, string hoTen, string email, string vaiTro, string trangThai, string? matKhauMoi);
        Task<(bool Success, string Message)> ToggleStatusAsync(int id);
        Task<(bool Success, string Message)> DeleteAccountAsync(int id);
    }
}
