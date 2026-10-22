// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Nội dung thực hiện: Entity Tài khoản (Module 1)

using System.ComponentModel.DataAnnotations;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập bắt buộc")]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Mật khẩu bắt buộc")]
        [StringLength(255)]
        public string MatKhau { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string VaiTro { get; set; } // Admin, Giáo vụ, Học viên

        [Required]
        [StringLength(20)]
        public string TrangThai { get; set; } // Đang hoạt động, Bị khóa
    }
}
