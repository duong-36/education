// Họ và tên: Nguyễn Đình Kiên
// Mã sinh viên: 23103100020
// Nội dung thực hiện: Entity Lớp học (Module 2)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models
{
    public class LopHoc
    {
        [Key]
        public int MaLop { get; set; }

        [Required(ErrorMessage = "Tên lớp bắt buộc")]
        [StringLength(200)]
        public string TenLop { get; set; }

        [Required]
        public int MaChuongTrinh { get; set; }

        [StringLength(100)]
        public string GiangVien { get; set; }

        [StringLength(50)]
        public string TrinhDoDauVao { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sĩ số tối đa phải > 0")]
        public int SiSoToiDa { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số buổi học phải > 0")]
        public int SoBuoiHoc { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Học phí phải >= 0")]
        public double HocPhi { get; set; }

        [Required]
        public DateTime NgayBatDauDangKy { get; set; }

        [Required]
        public DateTime HanDangKy { get; set; }

        [Required]
        public DateTime NgayKhaiGiang { get; set; }

        public string MoTa { get; set; }

        public string YeuCauHocVien { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } // Chưa mở, Đang mở đăng ký, Tạm dừng, Đã đóng đăng ký

        [ForeignKey("MaChuongTrinh")]
        public virtual ChuongTrinhDaoTao? ChuongTrinhDaoTao { get; set; }
    }
}
