using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations
{
    public class HocVienRepository : Repository<HocVien>, IHocVienRepository
    {
        public HocVienRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<HocVien?> GetByTaiKhoanIdAsync(int maTaiKhoan)
        {
            return await _dbSet
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
        }

        public async Task<HocVien?> GetWithTaiKhoanAsync(int maHocVien)
        {
            return await _dbSet
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaHocVien == maHocVien);
        }
    }
}
