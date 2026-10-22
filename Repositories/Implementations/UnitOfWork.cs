using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private ITaiKhoanRepository? _taiKhoans;
        private ILopHocRepository? _lopHocs;
        private IHocVienRepository? _hocViens;
        private IDangKyHocRepository? _dangKyHocs;
        private IBuoiHocRepository? _buoiHocs;
        private IKetQuaHocTapRepository? _ketQuaHocTaps;
        private IChuongTrinhDaoTaoRepository? _chuongTrinhDaoTaos;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITaiKhoanRepository TaiKhoans =>
            _taiKhoans ??= new TaiKhoanRepository(_context);

        public ILopHocRepository LopHocs =>
            _lopHocs ??= new LopHocRepository(_context);

        public IHocVienRepository HocViens =>
            _hocViens ??= new HocVienRepository(_context);

        public IDangKyHocRepository DangKyHocs =>
            _dangKyHocs ??= new DangKyHocRepository(_context);

        public IBuoiHocRepository BuoiHocs =>
            _buoiHocs ??= new BuoiHocRepository(_context);

        public IKetQuaHocTapRepository KetQuaHocTaps =>
            _ketQuaHocTaps ??= new KetQuaHocTapRepository(_context);

        public IChuongTrinhDaoTaoRepository ChuongTrinhDaoTaos =>
            _chuongTrinhDaoTaos ??= new ChuongTrinhDaoTaoRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
