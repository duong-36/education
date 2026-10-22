using System.Collections.Generic;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class ThongKeReportVM
    {
        public double TongDoanhThu { get; set; }
        public double AvgLapDay { get; set; }
        public int TongChoHoc { get; set; }
        public int ChoDaDangKy { get; set; }
        public int TongBuoiHoc { get; set; }
        public int DangKyChoDuyet { get; set; }
        public double TyLeHoanThanh { get; set; }

        public IEnumerable<ThongKeDoanhThuVM> DoanhThuTheoThang { get; set; } = new List<ThongKeDoanhThuVM>();
        public IEnumerable<dynamic> LopTheoCTDT { get; set; } = new List<dynamic>();
        public IEnumerable<dynamic> DKTheoLop { get; set; } = new List<dynamic>();
        public IEnumerable<dynamic> DKTheoTrangThai { get; set; } = new List<dynamic>();
        public dynamic? LopNhieuDKNhat { get; set; }
        public IEnumerable<dynamic> DoanhThuTheoCTDT { get; set; } = new List<dynamic>();
        public IEnumerable<dynamic> DKTheoThang { get; set; } = new List<dynamic>();
        public IEnumerable<dynamic> BuoiHocTheoThang { get; set; } = new List<dynamic>();
        public IEnumerable<dynamic> DiemTBTheoLop { get; set; } = new List<dynamic>();
        public IEnumerable<TyLeLapDayVM> TyLeLapDay { get; set; } = new List<TyLeLapDayVM>();
        public IEnumerable<dynamic> TopGiangVien { get; set; } = new List<dynamic>();
    }
}
