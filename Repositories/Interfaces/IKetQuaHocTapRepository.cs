using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces
{
    public interface IKetQuaHocTapRepository : IRepository<KetQuaHocTap>
    {
        Task<KetQuaHocTap?> GetByHocVienAndLopAsync(int maHocVien, int maLop);
        Task<IEnumerable<KetQuaHocTap>> GetByHocVienAsync(int maHocVien);
        Task<IEnumerable<KetQuaHocTap>> GetByLopAsync(int maLop);
    }
}
