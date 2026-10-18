// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Entity Kết quả học tập (Module 5)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models
{
    public class KetQuaHocTap
    {
        [Key]
        public int MaKetQua { get; set; }

        [Required]
        public int MaDangKy { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm chuyên cần phải từ 0 đến 10")]
        public double DiemChuyenCan { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm cuối khóa phải từ 0 đến 10")]
        public double DiemCuoiKhoa { get; set; }

        public double DiemTongKet { get; set; }

        [Required]
        [StringLength(50)]
        public string KetQua { get; set; } // Hoàn thành, Không hoàn thành

        public string NhanXet { get; set; }

        public DateTime NgayCapNhat { get; set; }

        [ForeignKey("MaDangKy")]
        public virtual DangKyHoc? DangKyHoc { get; set; }
    }
}
