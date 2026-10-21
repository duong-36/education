// Họ và tên: [Tên SV3]
// Mã sinh viên: [Mã SV3]
// Nội dung thực hiện: Entity Học viên (Module 3)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models
{
    public class HocVien
    {
        [Key]
        public int MaHocVien { get; set; }

        [Required]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên bắt buộc")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Ngày sinh hợp lệ")]
        public DateTime NgaySinh { get; set; }

        [StringLength(10)]
        public string GioiTinh { get; set; }

        [Phone(ErrorMessage = "Số điện thoại hợp lệ")]
        [StringLength(20)]
        public string SoDienThoai { get; set; }

        [EmailAddress(ErrorMessage = "Email hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; }

        public string DiaChi { get; set; }

        [StringLength(50)]
        public string TrinhDoHocVan { get; set; }

        [StringLength(100)]
        public string NgheNghiep { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; }

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
