using System.Collections.Generic;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class HocVienSearchResultVM
    {
        public List<HocVien> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<string> TrinhDoList { get; set; } = new();
    }

    public class HocVienKpiVM
    {
        public int TongSoHocVien { get; set; }
        public int DangHoc { get; set; }
        public int TotNghiep { get; set; }
    }

    public class HocVienDetailsVM
    {
        public HocVien HocVien { get; set; } = null!;
        public List<DangKyHoc> DangKys { get; set; } = new();
        public List<KetQuaHocTap> KetQuas { get; set; } = new();
    }
}
