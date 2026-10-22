using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations
{
    public class LopHocRepository : Repository<LopHoc>, ILopHocRepository
    {
        public LopHocRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LopHoc?> GetDetailsWithProgramAsync(int id)
        {
            return await _dbSet
                .Include(l => l.ChuongTrinhDaoTao)
                .FirstOrDefaultAsync(l => l.MaLop == id);
        }

        public async Task<IEnumerable<LopHoc>> GetOpenClassesWithProgramAsync()
        {
            return await _dbSet
                .Include(l => l.ChuongTrinhDaoTao)
                .Where(l => l.TrangThai == "Mở đăng ký" || l.TrangThai == "Đang mở đăng ký" || l.TrangThai == "Đang học")
                .ToListAsync();
        }
    }
}
