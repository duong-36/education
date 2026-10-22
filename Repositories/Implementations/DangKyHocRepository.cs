using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations
{
    public class DangKyHocRepository : Repository<DangKyHoc>, IDangKyHocRepository
    {
        public DangKyHocRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<DangKyHoc?> GetDetailsAsync(int id)
        {
            return await _dbSet
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);
        }

        public async Task<IEnumerable<DangKyHoc>> GetByHocVienAsync(int maHocVien)
        {
            return await _dbSet
                .Include(d => d.LopHoc)
                    .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(d => d.MaHocVien == maHocVien)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();
        }

        public async Task<DangKyHoc?> FindRegistrationAsync(int maHocVien, int maLop)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.MaHocVien == maHocVien && d.MaLop == maLop);
        }
    }
}
