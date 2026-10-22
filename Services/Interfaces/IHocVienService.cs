using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces
{
    public interface IHocVienService
    {
        Task<HocVienSearchResultVM> GetFilteredStudentsAsync(
            string search = "",
            string trangThai = "",
            string trinhDo = "",
            int page = 1,
            int pageSize = 10);

        Task<HocVienKpiVM> GetKpiAsync();
        Task<HocVien?> GetByIdAsync(int id);
        Task<HocVien?> GetByTaiKhoanIdAsync(int maTaiKhoan);
        Task<HocVienDetailsVM?> GetDetailsWithRegistrationsAsync(int id);
        Task<(bool Success, string Message, int? NewId, string? Username)> CreateStudentAsync(HocVien model, string tenDangNhap, string matKhau);
        Task<(bool Success, string Message)> UpdateStudentAsync(HocVien model);
        Task<(bool Success, string Message)> DeleteStudentAsync(int id);
    }
}
