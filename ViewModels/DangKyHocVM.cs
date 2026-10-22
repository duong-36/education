using System.Collections.Generic;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class DangKyHocSearchResultVM
    {
        public List<DangKyHoc> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class DangKyHocKpiVM
    {
        public int ChoDuyetCount { get; set; }
        public int DaDuyetCount { get; set; }
        public double TongDoanhThu { get; set; }
    }
}
