using System.Collections.Generic;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class KetQuaHocTapSearchResultVM
    {
        public List<KetQuaHocTap> Items { get; set; } = new();
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int TotalGraded { get; set; }
        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public double AverageScore { get; set; }
        public double PassRate { get; set; }
        public int CountA { get; set; }
        public int CountB { get; set; }
        public int CountC { get; set; }
        public int CountD { get; set; }
        public int CountF { get; set; }
    }
}
