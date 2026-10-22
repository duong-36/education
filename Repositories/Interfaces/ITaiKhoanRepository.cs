using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces
{
    public interface ITaiKhoanRepository : IRepository<TaiKhoan>
    {
        Task<TaiKhoan?> GetByUsernameAsync(string username);
        Task<TaiKhoan?> GetByEmailAsync(string email);
        Task<bool> IsUsernameTakenAsync(string username, int? excludeId = null);
        Task<bool> IsEmailTakenAsync(string email, int? excludeId = null);
    }
}
