using System;
using System.Threading.Tasks;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITaiKhoanRepository TaiKhoans { get; }
        ILopHocRepository LopHocs { get; }
        IHocVienRepository HocViens { get; }
        IDangKyHocRepository DangKyHocs { get; }
        IBuoiHocRepository BuoiHocs { get; }
        IKetQuaHocTapRepository KetQuaHocTaps { get; }
        IChuongTrinhDaoTaoRepository ChuongTrinhDaoTaos { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
