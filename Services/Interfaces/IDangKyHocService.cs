using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces
{
    public interface IDangKyHocService
    {
        Task<DangKyHocSearchResultVM> GetFilteredRegistrationsAsync(
            string search = "",
            int? maChuongTrinh = null,
            int? maLop = null,
            string trangThai = "",
            DateTime? tuNgay = null,
            DateTime? denNgay = null,
            string sortBy = "",
            int page = 1,
            int pageSize = 10);

        Task<DangKyHocKpiVM> GetKpiAsync();
        Task<DangKyHoc?> GetDetailsAsync(int id);
        Task<List<DangKyHoc>> GetByHocVienAsync(int maHocVien);
        Task<List<DangKyHoc>> GetEligibleForGradingAsync();
        Task<(bool Success, string Message, int? DangKyId)> RegisterClassAsync(int maHocVien, int maLop, string? ghiChu = null);
        Task<(bool Success, string Message)> ApproveRegistrationAsync(int id, string? nhanXet = null);
        Task<(bool Success, string Message)> RejectRegistrationAsync(int id, string lyDo);
        Task<(bool Success, string Message)> ProcessPaymentAsync(int id, double soTien, string? ghiChu = null);
        Task<(bool Success, string Message)> StartLearningAsync(int id);
        Task<(bool Success, string Message)> DeleteRegistrationAsync(int id);
    }
}
