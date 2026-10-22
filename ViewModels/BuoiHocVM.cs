using System.Collections.Generic;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class BuoiHocSearchResultVM
    {
        public List<BuoiHoc> Items { get; set; } = new();
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public HashSet<int> ConflictIds { get; set; } = new();
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public int ScheduledSessions { get; set; }
        public int CanceledSessions { get; set; }
        public int SoThi { get; set; }
        public int SoChinhKhoa { get; set; }
        public List<string> DanhSachPhong { get; set; } = new();
        public List<string> DanhSachGiangVien { get; set; } = new();
        public List<BuoiHoc> AllBuoiHocs { get; set; } = new();
    }
}
