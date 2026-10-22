using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations
{
    public class TaiKhoanRepository : Repository<TaiKhoan>, ITaiKhoanRepository
    {
        public TaiKhoanRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TaiKhoan?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            return await _dbSet.FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == username.Trim().ToLower());
        }

        public async Task<TaiKhoan?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _dbSet.FirstOrDefaultAsync(t => t.Email.ToLower() == email.Trim().ToLower());
        }

        public async Task<bool> IsUsernameTakenAsync(string username, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            var query = _dbSet.Where(t => t.TenDangNhap.ToLower() == username.Trim().ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.MaTaiKhoan != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<bool> IsEmailTakenAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var query = _dbSet.Where(t => t.Email.ToLower() == email.Trim().ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.MaTaiKhoan != excludeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
