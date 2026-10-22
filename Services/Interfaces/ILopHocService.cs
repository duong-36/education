using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces
{
    public interface ILopHocService
    {
        Task<LopHocSearchResultVM> GetFilteredClassesAsync(
            int page = 1,
            int pageSize = 6,
            string search = "",
            int? maChuongTrinh = null,
            string trinhDo = "",
            string trangThai = "",
            double? hocPhiMin = null,
            double? hocPhiMax = null,
            string hanDangKyStatus = "",
            string choTrongStatus = "",
            string sortBy = "");

        Task<LopHocKpiVM> GetKpiAsync();
        Task<LopHoc?> GetByIdAsync(int id);
        Task<LopHoc?> GetDetailsAsync(int id);
        Task<LopHocDetailsVM?> GetClassDetailsAsync(int id);
        Task<List<LopHoc>> GetAllAsync();
        Task<List<LopHoc>> GetOpenClassesAsync();
        Task<(bool Success, string Message, int? NewId)> CreateClassAsync(LopHoc lop);
        Task<(bool Success, string Message)> UpdateClassAsync(LopHoc lop);
        Task<(bool Success, string Message)> EditClassAsync(
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
            string trangThai);
        Task<(bool Success, string Message)> DeleteClassAsync(int id);
    }
}
