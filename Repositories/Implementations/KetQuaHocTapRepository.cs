using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations
{
    public class KetQuaHocTapRepository : Repository<KetQuaHocTap>, IKetQuaHocTapRepository
    {
        public KetQuaHocTapRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<KetQuaHocTap?> GetByHocVienAndLopAsync(int maHocVien, int maLop)
        {
            return await _dbSet
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                        .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.HocVien)
                .FirstOrDefaultAsync(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == maHocVien && k.DangKyHoc.MaLop == maLop);
        }

        public async Task<IEnumerable<KetQuaHocTap>> GetByHocVienAsync(int maHocVien)
        {
            return await _dbSet
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.LopHoc)
                        .ThenInclude(l => l.ChuongTrinhDaoTao)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaHocVien == maHocVien)
                .ToListAsync();
        }

        public async Task<IEnumerable<KetQuaHocTap>> GetByLopAsync(int maLop)
        {
            return await _dbSet
                .Include(k => k.DangKyHoc)
                    .ThenInclude(d => d.HocVien)
                .Where(k => k.DangKyHoc != null && k.DangKyHoc.MaLop == maLop)
                .ToListAsync();
        }
    }
}
