using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Repositories.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Interfaces;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Services.Implementations
{
    public class ChuongTrinhDaoTaoService : IChuongTrinhDaoTaoService
    {
        private readonly IUnitOfWork _uow;

        public ChuongTrinhDaoTaoService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChuongTrinhDaoTaoSearchResultVM> GetFilteredProgramsAsync(
            int page = 1,
            int pageSize = 8,
            string search = "",
            string trangThai = "")
        {
            var query = _uow.ChuongTrinhDaoTaos.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(c => c.TenChuongTrinh.ToLower().Contains(s) ||
                                         (c.MoTa != null && c.MoTa.ToLower().Contains(s)) ||
                                         (c.EmailLienHe != null && c.EmailLienHe.ToLower().Contains(s)));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(c => c.TrangThai == trangThai);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderByDescending(c => c.MaChuongTrinh)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var ctdtIds = items.Select(c => c.MaChuongTrinh).ToList();
            var classCounts = await _uow.LopHocs.Query()
                .Where(l => ctdtIds.Contains(l.MaChuongTrinh))
                .GroupBy(l => l.MaChuongTrinh)
                .Select(g => new { MaChuongTrinh = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.MaChuongTrinh, x => x.Count);

            int tongCTDT = await _uow.ChuongTrinhDaoTaos.CountAsync();
            int dangHoatDong = await _uow.ChuongTrinhDaoTaos.CountAsync(c => c.TrangThai == "Đang hoạt động");
            int tongLopLienKet = await _uow.LopHocs.CountAsync();

            return new ChuongTrinhDaoTaoSearchResultVM
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = page,
                ClassCounts = classCounts,
                TongCTDT = tongCTDT,
                DangHoatDong = dangHoatDong,
                TongLopLienKet = tongLopLienKet
            };
        }

        public async Task<List<ChuongTrinhDaoTao>> GetAllAsync()
        {
            var list = await _uow.ChuongTrinhDaoTaos.Query()
                .OrderBy(c => c.TenChuongTrinh)
                .ToListAsync();
            return list;
        }

        public async Task<ChuongTrinhDaoTao?> GetByIdAsync(int id)
        {
            return await _uow.ChuongTrinhDaoTaos.GetByIdAsync(id);
        }

        public async Task<(bool Success, string Message, int? NewId)> CreateProgramAsync(ChuongTrinhDaoTao ctdt)
        {
            if (string.IsNullOrWhiteSpace(ctdt.TenChuongTrinh))
            {
                return (false, "Tên chương trình đào tạo bắt buộc nhập.", null);
            }

            ctdt.TenChuongTrinh = ctdt.TenChuongTrinh.Trim();
            bool exists = await _uow.ChuongTrinhDaoTaos.ExistsAsync(c => c.TenChuongTrinh.ToLower() == ctdt.TenChuongTrinh.ToLower());
            if (exists)
            {
                return (false, "Tên chương trình đào tạo này đã tồn tại.", null);
            }

            if (string.IsNullOrEmpty(ctdt.TrangThai)) ctdt.TrangThai = "Đang hoạt động";
            if (string.IsNullOrEmpty(ctdt.EmailLienHe)) ctdt.EmailLienHe = "contact@uneti.edu.vn";

            await _uow.ChuongTrinhDaoTaos.AddAsync(ctdt);
            await _uow.CompleteAsync();

            return (true, "Tạo chương trình đào tạo mới thành công!", ctdt.MaChuongTrinh);
        }

        public async Task<(bool Success, string Message)> UpdateProgramAsync(int maChuongTrinh, string tenChuongTrinh, string? moTa, string? emailLienHe, string trangThai)
        {
            var existing = await _uow.ChuongTrinhDaoTaos.GetByIdAsync(maChuongTrinh);
            if (existing == null) return (false, "Không tìm thấy chương trình đào tạo cần sửa.");

            tenChuongTrinh = tenChuongTrinh.Trim();
            bool exists = await _uow.ChuongTrinhDaoTaos.ExistsAsync(c => c.MaChuongTrinh != maChuongTrinh && c.TenChuongTrinh.ToLower() == tenChuongTrinh.ToLower());
            if (exists)
            {
                return (false, "Tên chương trình đào tạo này đã trùng với chương trình khác.");
            }

            existing.TenChuongTrinh = tenChuongTrinh;
            existing.MoTa = moTa ?? "";
            existing.EmailLienHe = emailLienHe ?? "contact@uneti.edu.vn";
            existing.TrangThai = trangThai;

            _uow.ChuongTrinhDaoTaos.Update(existing);
            await _uow.CompleteAsync();

            return (true, "Cập nhật chương trình đào tạo thành công!");
        }

        public async Task<(bool Success, string Message)> DeleteProgramAsync(int id)
        {
            var existing = await _uow.ChuongTrinhDaoTaos.GetByIdAsync(id);
            if (existing == null) return (false, "Chương trình đào tạo không tồn tại.");

            int countLop = await _uow.LopHocs.CountAsync(l => l.MaChuongTrinh == id);
            if (countLop > 0)
            {
                return (false, $"Không thể xóa chương trình '{existing.TenChuongTrinh}' vì đang có {countLop} lớp học trực thuộc. Vui lòng chuyển hoặc xóa các lớp học trước.");
            }

            _uow.ChuongTrinhDaoTaos.Delete(existing);
            await _uow.CompleteAsync();

            return (true, "Xóa chương trình đào tạo thành công!");
        }
    }
}
