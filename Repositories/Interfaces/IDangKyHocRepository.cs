using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces
{
    public interface IDangKyHocRepository : IRepository<DangKyHoc>
    {
        Task<DangKyHoc?> GetDetailsAsync(int id);
        Task<IEnumerable<DangKyHoc>> GetByHocVienAsync(int maHocVien);
        Task<DangKyHoc?> FindRegistrationAsync(int maHocVien, int maLop);
    }
}
