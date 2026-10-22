using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces
{
    public interface IBuoiHocService
    {
        Task<BuoiHocSearchResultVM> GetFilteredSessionsAsync(
            int? maLop = null,
            string? phong = null,
            string? trangThai = null,
            string? search = null,
            int page = 1,
            int pageSize = 8);

        Task<BuoiHoc?> GetByIdAsync(int id);
        Task<List<BuoiHoc>> GetByLopHocAsync(int maLop);
        Task<List<BuoiHoc>> GetScheduleForClassesAsync(IEnumerable<int> classIds, DateTime fromDate, DateTime toDate);
        Task<(bool Success, string Message, int? NewId)> CreateSessionAsync(BuoiHoc buoiHoc);
        Task<(bool Success, string Message)> UpdateSessionAsync(BuoiHoc buoiHoc);
        Task<(bool Success, string Message)> DeleteSessionAsync(int id);
        Task<(bool Success, string Message)> CompleteSessionAsync(int id);
        Task<(bool Success, string Message)> CancelSessionAsync(int id);
    }
}
