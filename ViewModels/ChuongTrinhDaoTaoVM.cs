using System.Collections.Generic;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class ChuongTrinhDaoTaoSearchResultVM
    {
        public List<ChuongTrinhDaoTao> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Dictionary<int, int> ClassCounts { get; set; } = new();
        public int TongCTDT { get; set; }
        public int DangHoatDong { get; set; }
        public int TongLopLienKet { get; set; }
    }
}
