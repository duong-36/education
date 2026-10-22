using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces
{
    public interface IChuongTrinhDaoTaoService
    {
        Task<ChuongTrinhDaoTaoSearchResultVM> GetFilteredProgramsAsync(
            int page = 1,
            int pageSize = 8,
            string search = "",
            string trangThai = "");

        Task<List<ChuongTrinhDaoTao>> GetAllAsync();
        Task<ChuongTrinhDaoTao?> GetByIdAsync(int id);
        Task<(bool Success, string Message, int? NewId)> CreateProgramAsync(ChuongTrinhDaoTao ctdt);
        Task<(bool Success, string Message)> UpdateProgramAsync(int maChuongTrinh, string tenChuongTrinh, string? moTa, string? emailLienHe, string trangThai);
        Task<(bool Success, string Message)> DeleteProgramAsync(int id);
    }
}
