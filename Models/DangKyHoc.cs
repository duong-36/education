// Họ và tên: [Tên SV3, SV4]
// Mã sinh viên: [Mã SV3, SV4]
// Nội dung thực hiện: Entity Đăng ký học (Module 3 & 4)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models
{
    public class DangKyHoc
    {
        [Key]
        public int MaDangKy { get; set; }

        [Required]
        public int MaHocVien { get; set; }

        [Required]
        public int MaLop { get; set; }

        [Required]
        public DateTime NgayDangKy { get; set; }

        public string GhiChu { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; }

        public DateTime? NgayXuLy { get; set; }

        public string NhanXetGiaoVu { get; set; }

        public DateTime? NgayDongHocPhi { get; set; }

        [Range(0, double.MaxValue)]
        public double SoTienDaDong { get; set; }

        [ForeignKey("MaHocVien")]
        public virtual HocVien? HocVien { get; set; }

        [ForeignKey("MaLop")]
        public virtual LopHoc? LopHoc { get; set; }
    }
}
