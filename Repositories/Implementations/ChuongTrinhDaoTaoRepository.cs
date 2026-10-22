using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations
{
    public class ChuongTrinhDaoTaoRepository : Repository<ChuongTrinhDaoTao>, IChuongTrinhDaoTaoRepository
    {
        public ChuongTrinhDaoTaoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
