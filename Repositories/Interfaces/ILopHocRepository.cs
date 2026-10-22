using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces
{
    public interface ILopHocRepository : IRepository<LopHoc>
    {
        Task<LopHoc?> GetDetailsWithProgramAsync(int id);
        Task<IEnumerable<LopHoc>> GetOpenClassesWithProgramAsync();
    }
}
