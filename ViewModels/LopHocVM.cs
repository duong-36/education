using System.Collections.Generic;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class LopHocSearchResultVM
    {
        public List<LopHoc> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Dictionary<int, int> SiSoMap { get; set; } = new();
        public Dictionary<int, int> BuoiHocMap { get; set; } = new();
    }

    public class LopHocKpiVM
    {
        public int TongSoLop { get; set; }
        public int LopDangMo { get; set; }
        public int TongHocVienDangHoc { get; set; }
    }

    public class LopHocDetailsVM
    {
        public LopHoc LopHoc { get; set; } = null!;
        public List<BuoiHoc> BuoiHocs { get; set; } = new();
        public List<DangKyHoc> DangKys { get; set; } = new();
        public int SoHocVien { get; set; }
    }
}
