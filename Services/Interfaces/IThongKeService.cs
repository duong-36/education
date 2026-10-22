using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces
{
    public interface IThongKeService
    {
        Task<DashboardVM> GetDashboardDataAsync(int page = 1, string? status = "all", string? search = "");
        Task<(bool Success, string Message)> QuickApproveRegistrationAsync(int id);
        Task<ThongKeReportVM> GetStatisticsReportAsync();
    }
}
