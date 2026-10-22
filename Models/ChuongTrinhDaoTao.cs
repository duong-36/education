// Họ và tên: Vũ Đình Trường
// Mã sinh viên: 23103100026
// Nội dung thực hiện: Entity CTĐT (Module 1)

using System.ComponentModel.DataAnnotations;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models
{
    public class ChuongTrinhDaoTao
    {
        [Key]
        public int MaChuongTrinh { get; set; }

        [Required(ErrorMessage = "Tên chương trình bắt buộc")]
        [StringLength(200)]
        public string TenChuongTrinh { get; set; }

        public string MoTa { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string EmailLienHe { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; }
    }
}
