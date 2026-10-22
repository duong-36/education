// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Nội dung thực hiện: ViewModel Đăng ký tài khoản học viên (Module 1)

using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ gồm chữ cái, số và dấu gạch dưới")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận lại mật khẩu")]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số (bắt đầu bằng 0)")]
        public string? SoDienThoai { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string? GioiTinh { get; set; } = "Nam";

        public string? DiaChi { get; set; }
    }
}
