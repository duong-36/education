using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces
{
    public interface IBuoiHocRepository : IRepository<BuoiHoc>
    {
        Task<IEnumerable<BuoiHoc>> GetByLopHocAsync(int maLop);
        Task<IEnumerable<BuoiHoc>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int? maLop = null);
        Task<IEnumerable<BuoiHoc>> GetScheduleForClassesAsync(IEnumerable<int> maLopList, DateTime fromDate, DateTime toDate);
    }
}
