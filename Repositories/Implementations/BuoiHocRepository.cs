using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations
{
    public class BuoiHocRepository : Repository<BuoiHoc>, IBuoiHocRepository
    {
        public BuoiHocRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BuoiHoc>> GetByLopHocAsync(int maLop)
        {
            return await _dbSet
                .Include(b => b.LopHoc)
                .Where(b => b.MaLop == maLop)
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();
        }

        public async Task<IEnumerable<BuoiHoc>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int? maLop = null)
        {
            var query = _dbSet
                .Include(b => b.LopHoc)
                .Where(b => b.ThoiGianBatDau >= fromDate && b.ThoiGianBatDau <= toDate);

            if (maLop.HasValue)
            {
                query = query.Where(b => b.MaLop == maLop.Value);
            }

            return await query
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();
        }

        public async Task<IEnumerable<BuoiHoc>> GetScheduleForClassesAsync(IEnumerable<int> maLopList, DateTime fromDate, DateTime toDate)
        {
            return await _dbSet
                .Include(b => b.LopHoc)
                .Where(b => maLopList.Contains(b.MaLop) && b.ThoiGianBatDau >= fromDate && b.ThoiGianBatDau <= toDate)
                .OrderBy(b => b.ThoiGianBatDau)
                .ToListAsync();
        }
    }
}
