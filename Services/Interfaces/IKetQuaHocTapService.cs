using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces
{
    public interface IKetQuaHocTapService
    {
        Task<KetQuaHocTapSearchResultVM> GetFilteredResultsAsync(
            int? maLop = null,
            string? ketQua = null,
            string? search = null,
            int page = 1,
            int pageSize = 8);

        Task<KetQuaHocTap?> GetByIdAsync(int id);
        Task<List<KetQuaHocTap>> GetByHocVienAsync(int maHocVien);
        Task<(bool Success, string Message, int? NewId)> CreateResultAsync(KetQuaHocTap kq);
        Task<(bool Success, string Message)> UpdateResultAsync(int maKetQua, double diemChuyenCan, double diemCuoiKhoa, string? nhanXet, string? ketQua);
        Task<(bool Success, string Message)> DeleteResultAsync(int id);
        (double DiemTongKet, string KetQua, string XepLoai) CalculateGrade(double diemChuyenCan, double diemCuoiKhoa);
    }
}
