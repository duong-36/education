// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Entity Buổi học - Quản lý lịch học của lớp (Module 5)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models
{
    public class BuoiHoc
    {
        [Key]
        public int MaBuoiHoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn lớp học")]
        public int MaLop { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        public DateTime ThoiGianBatDau { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        public DateTime ThoiGianKetThuc { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phòng học")]
        [StringLength(50)]
        public string PhongHoc { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên giảng viên")]
        [StringLength(100)]
        public string GiangVien { get; set; }

        public string NoiDung { get; set; }

        public string GhiChu { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } // Đã lên lịch, Đã hoàn thành, Đã hủy

        [ForeignKey("MaLop")]
        public virtual LopHoc? LopHoc { get; set; }
    }
}
