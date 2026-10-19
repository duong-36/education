// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: ViewModel cho trang Dashboard (Module 5)

using System.Collections.Generic;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class DashboardVM
    {
        public int TongSoCTDT { get; set; }
        public int TongSoLop { get; set; }
        public int LopDangMo { get; set; }
        public int TongSoHocVien { get; set; }
        public int TongSoDangKy { get; set; }
        public int DangKyChoDuyet { get; set; }
        public int DangKyDaDuyet { get; set; }
        public int BuoiHocSapToi { get; set; }
        public int HocVienHoanThanh { get; set; }
        public double TyLeHoanThanh { get; set; }

        public List<DangKyHoc> HoatDongGanDay { get; set; } = new List<DangKyHoc>();
        public List<BuoiHoc> DanhSachBuoiHoc { get; set; } = new List<BuoiHoc>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public string SelectedStatus { get; set; } = "all";
        public string Search { get; set; } = "";
    }
}
