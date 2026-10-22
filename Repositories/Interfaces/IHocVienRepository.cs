using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces
{
    public interface IHocVienRepository : IRepository<HocVien>
    {
        Task<HocVien?> GetByTaiKhoanIdAsync(int maTaiKhoan);
        Task<HocVien?> GetWithTaiKhoanAsync(int maHocVien);
    }
}
