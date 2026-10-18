using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data
{
    public static class SeedDataExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var taiKhoans = new List<TaiKhoan>();
            taiKhoans.Add(new TaiKhoan { MaTaiKhoan = 1, TenDangNhap = "admin1", MatKhau = "123456", HoTen = "Admin 1", VaiTro = "Admin", TrangThai = "Đang hoạt động", Email = "admin1@uneti.edu.vn" });
            taiKhoans.Add(new TaiKhoan { MaTaiKhoan = 2, TenDangNhap = "admin2", MatKhau = "123456", HoTen = "Admin 2", VaiTro = "Admin", TrangThai = "Đang hoạt động", Email = "admin2@uneti.edu.vn" });
            for(int i=1; i<=3; i++) taiKhoans.Add(new TaiKhoan { MaTaiKhoan = 2+i, TenDangNhap = $"giaovu{i}", MatKhau = "123456", HoTen = $"Giáo vụ {i}", VaiTro = "Giáo vụ", TrangThai = "Đang hoạt động", Email = $"giaovu{i}@uneti.edu.vn" });
            for(int i=1; i<=30; i++) taiKhoans.Add(new TaiKhoan { MaTaiKhoan = 5+i, TenDangNhap = $"hocvien{i}", MatKhau = "123456", HoTen = $"Học viên {i}", VaiTro = "Học viên", TrangThai = "Đang hoạt động", Email = $"hocvien{i}@gmail.com" });
            modelBuilder.Entity<TaiKhoan>().HasData(taiKhoans);

            var hocViens = new List<HocVien>();
            for(int i=1; i<=30; i++) hocViens.Add(new HocVien { MaHocVien = i, MaTaiKhoan = 5+i, HoTen = $"Học viên {i}", NgaySinh = new DateTime(2000, 1, 1).AddDays(i*30), GioiTinh = i%2==0 ? "Nam" : "Nữ", SoDienThoai = "0987654" + i.ToString("D3"), Email = $"hocvien{i}@gmail.com", DiaChi = "Hà Nội", TrinhDoHocVan = "Đại học", NgheNghiep = "Sinh viên", TrangThai = "Đang học" });
            modelBuilder.Entity<HocVien>().HasData(hocViens);

            var ctdts = new List<ChuongTrinhDaoTao>();
            string[] tenCtdts = { "Lập trình Web ASP.NET Core", "Lập trình ReactJS", "Khoa học dữ liệu với Python", "Tiếng Anh Giao Tiếp", "Lập trình Mobile Flutter" };
            for(int i=1; i<=5; i++) ctdts.Add(new ChuongTrinhDaoTao { MaChuongTrinh = i, TenChuongTrinh = tenCtdts[i-1], MoTa = "Chương trình " + tenCtdts[i-1], EmailLienHe = "contact@uneti.edu.vn", TrangThai = "Đang hoạt động" });
            modelBuilder.Entity<ChuongTrinhDaoTao>().HasData(ctdts);

            var lopHocs = new List<LopHoc>();
            string[] trangThaiLop = { "Đang mở đăng ký", "Đã đóng đăng ký", "Đang học", "Đã kết thúc", "Hết hạn đăng ký" };
            for(int i=1; i<=15; i++) lopHocs.Add(new LopHoc { 
                MaLop = i, MaChuongTrinh = ((i-1)%5)+1, TenLop = $"Lớp {tenCtdts[((i-1)%5)]} Khóa {i}", 
                GiangVien = $"Giảng viên {i%3+1}", TrinhDoDauVao = "Cơ bản", 
                SiSoToiDa = 30, SoBuoiHoc = 15, HocPhi = 5000000, 
                NgayBatDauDangKy = new DateTime(2023, 1, 1).AddMonths(i%12), 
                HanDangKy = new DateTime(2023, 1, 20).AddMonths(i%12), 
                NgayKhaiGiang = new DateTime(2023, 2, 1).AddMonths(i%12), 
                MoTa = "Mô tả lớp học", YeuCauHocVien = "Không", 
                TrangThai = trangThaiLop[(i-1)%5] 
            });
            modelBuilder.Entity<LopHoc>().HasData(lopHocs);

            var dangKyHocs = new List<DangKyHoc>();
            string[] trangThaiDK = { "Chờ duyệt", "Đã duyệt", "Đã đóng học phí", "Đang học", "Đã hủy" };
            for(int i=1; i<=45; i++) {
                string tt = i <= 15 ? "Đã đóng học phí" : trangThaiDK[(i-1)%5];
                dangKyHocs.Add(new DangKyHoc { 
                    MaDangKy = i, MaHocVien = ((i-1)%30)+1, MaLop = ((i-1)%15)+1, 
                    NgayDangKy = new DateTime(2023, 1, 15).AddDays(i), 
                    GhiChu = "", TrangThai = tt, 
                    NgayXuLy = new DateTime(2023, 1, 16).AddDays(i), 
                    NhanXetGiaoVu = "Đã kiểm tra", 
                    NgayDongHocPhi = tt == "Đã đóng học phí" || tt == "Đang học" ? new DateTime(2023, 1, 17).AddDays(i) : null, 
                    SoTienDaDong = tt == "Đã đóng học phí" || tt == "Đang học" ? 5000000 : 0 
                });
            }
            modelBuilder.Entity<DangKyHoc>().HasData(dangKyHocs);

            var buoiHocs = new List<BuoiHoc>
            {
                // Thứ 2 (2026-10-12)
                new BuoiHoc { MaBuoiHoc = 1, MaLop = 1, ThoiGianBatDau = new DateTime(2026, 10, 12, 7, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 12, 9, 15, 0), PhongHoc = "P.301-A1", GiangVien = "ThS. Nguyễn Văn A", NoiDung = "Bài 03: Kiến trúc ASP.NET Core MVC & Dependency Injection", GhiChu = "Phòng máy A1", TrangThai = "Đã hoàn thành" },
                new BuoiHoc { MaBuoiHoc = 2, MaLop = 2, ThoiGianBatDau = new DateTime(2026, 10, 12, 9, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 12, 11, 45, 0), PhongHoc = "Lab Web-01", GiangVien = "TS. Trần Thị B", NoiDung = "Bài 04: React Hooks & Quản lý State với Context API", GhiChu = "Thực hành phòng Lab", TrangThai = "Đã hoàn thành" },
                new BuoiHoc { MaBuoiHoc = 3, MaLop = 3, ThoiGianBatDau = new DateTime(2026, 10, 12, 13, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 12, 15, 45, 0), PhongHoc = "Lab AI-02", GiangVien = "ThS. Lê Hoàng C", NoiDung = "Bài 05: Phân tích và xử lý dữ liệu với thư viện Pandas", GhiChu = "Môi trường Jupyter Lab", TrangThai = "Đã hoàn thành" },

                // Thứ 3 (2026-10-13)
                new BuoiHoc { MaBuoiHoc = 4, MaLop = 4, ThoiGianBatDau = new DateTime(2026, 10, 13, 7, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 13, 9, 15, 0), PhongHoc = "P.402-HA8", GiangVien = "Ms. Sarah Jenkins", NoiDung = "Unit 04: Business Presentation & Workplace Communication", GhiChu = "Phòng đa phương tiện", TrangThai = "Đã hoàn thành" },
                new BuoiHoc { MaBuoiHoc = 5, MaLop = 5, ThoiGianBatDau = new DateTime(2026, 10, 13, 9, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 13, 11, 45, 0), PhongHoc = "Lab Mobile-03", GiangVien = "ThS. Phạm Minh D", NoiDung = "Bài 03: Thiết kế UI Widgets & Navigation trong Flutter", GhiChu = "Máy Mac + Emulator", TrangThai = "Đã hoàn thành" },
                new BuoiHoc { MaBuoiHoc = 6, MaLop = 6, ThoiGianBatDau = new DateTime(2026, 10, 13, 13, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 13, 15, 45, 0), PhongHoc = "P.301-A1", GiangVien = "ThS. Nguyễn Văn A", NoiDung = "Bài 06: Entity Framework Core Code-First & LINQ Queries", GhiChu = "Thực hành SQL Server", TrangThai = "Đã hoàn thành" },
                new BuoiHoc { MaBuoiHoc = 7, MaLop = 7, ThoiGianBatDau = new DateTime(2026, 10, 13, 16, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 13, 18, 15, 0), PhongHoc = "Lab Web-01", GiangVien = "TS. Trần Thị B", NoiDung = "Bài 07: Tích hợp RESTful API với Axios & Async Thunk", GhiChu = "Thực hành API", TrangThai = "Đã hoàn thành" },

                // Thứ 4 (2026-10-14)
                new BuoiHoc { MaBuoiHoc = 8, MaLop = 8, ThoiGianBatDau = new DateTime(2026, 10, 14, 7, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 14, 9, 15, 0), PhongHoc = "Lab AI-02", GiangVien = "ThS. Lê Hoàng C", NoiDung = "Bài 08: Trực quan hóa dữ liệu trực quan với Matplotlib & Seaborn", GhiChu = "Thực hành đồ thị", TrangThai = "Đã hoàn thành" },
                new BuoiHoc { MaBuoiHoc = 9, MaLop = 1, ThoiGianBatDau = new DateTime(2026, 10, 14, 9, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 14, 11, 45, 0), PhongHoc = "P.301-A1", GiangVien = "ThS. Nguyễn Văn A", NoiDung = "Kiểm tra giữa kỳ: LINQ Queries & Entity Framework Core", GhiChu = "Thi trắc nghiệm + Code trực tiếp", TrangThai = "Đã hoàn thành" },
                new BuoiHoc { MaBuoiHoc = 10, MaLop = 9, ThoiGianBatDau = new DateTime(2026, 10, 14, 13, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 14, 15, 45, 0), PhongHoc = "P.402-HA8", GiangVien = "Ms. Sarah Jenkins", NoiDung = "Unit 06: Email Writing & Business Negotiation Skills", GhiChu = "Văn bản thương mại", TrangThai = "Đã lên lịch" },

                // Thứ 5 (2026-10-15)
                new BuoiHoc { MaBuoiHoc = 11, MaLop = 10, ThoiGianBatDau = new DateTime(2026, 10, 15, 9, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 15, 11, 45, 0), PhongHoc = "Lab Mobile-03", GiangVien = "ThS. Phạm Minh D", NoiDung = "Bài 07: Quản lý State nâng cao với Bloc / Cubit", GhiChu = "Kiến trúc Bloc", TrangThai = "Đã lên lịch" },
                new BuoiHoc { MaBuoiHoc = 12, MaLop = 2, ThoiGianBatDau = new DateTime(2026, 10, 15, 13, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 15, 15, 45, 0), PhongHoc = "Lab Web-01", GiangVien = "TS. Trần Thị B", NoiDung = "Kiểm tra thực hành: Xây dựng SPA React Component & Hooks", GhiChu = "Thi thực hành 90 phút", TrangThai = "Đã lên lịch" },
                new BuoiHoc { MaBuoiHoc = 13, MaLop = 11, ThoiGianBatDau = new DateTime(2026, 10, 15, 16, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 15, 18, 15, 0), PhongHoc = "P.301-A1", GiangVien = "TS. Vũ Đình E", NoiDung = "Bài 05: Xác thực và phân quyền với ASP.NET Core Identity", GhiChu = "Bảo mật ứng dụng", TrangThai = "Đã lên lịch" },

                // Thứ 6 (2026-10-16)
                new BuoiHoc { MaBuoiHoc = 14, MaLop = 4, ThoiGianBatDau = new DateTime(2026, 10, 16, 7, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 16, 9, 15, 0), PhongHoc = "P.402-HA8", GiangVien = "Ms. Sarah Jenkins", NoiDung = "Thi đánh giá năng lực: Tiếng Anh Giao tiếp B1 (Vấn đáp)", GhiChu = "Vấn đáp 1-1", TrangThai = "Đã lên lịch" },
                new BuoiHoc { MaBuoiHoc = 15, MaLop = 12, ThoiGianBatDau = new DateTime(2026, 10, 16, 9, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 16, 11, 45, 0), PhongHoc = "Lab Web-01", GiangVien = "TS. Trần Thị B", NoiDung = "Bài 08: Tối ưu hiệu năng React với useMemo, useCallback & Lazy", GhiChu = "Tối ưu render", TrangThai = "Đã lên lịch" },
                new BuoiHoc { MaBuoiHoc = 16, MaLop = 3, ThoiGianBatDau = new DateTime(2026, 10, 16, 13, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 16, 15, 45, 0), PhongHoc = "Lab AI-02", GiangVien = "ThS. Lê Hoàng C", NoiDung = "Kiểm tra tiến độ đồ án: Huấn luyện mô hình Machine Learning", GhiChu = "Báo cáo tiến độ", TrangThai = "Đã lên lịch" },
                new BuoiHoc { MaBuoiHoc = 17, MaLop = 13, ThoiGianBatDau = new DateTime(2026, 10, 16, 16, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 16, 18, 15, 0), PhongHoc = "Lab AI-02", GiangVien = "ThS. Lê Hoàng C", NoiDung = "Bài 09: Xử lý ngôn ngữ tự nhiên cơ bản với thư viện NLTK", GhiChu = "Tokenization & TF-IDF", TrangThai = "Đã lên lịch" },

                // Thứ 7 (2026-10-17)
                new BuoiHoc { MaBuoiHoc = 18, MaLop = 5, ThoiGianBatDau = new DateTime(2026, 10, 17, 7, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 17, 9, 15, 0), PhongHoc = "Lab Mobile-03", GiangVien = "ThS. Phạm Minh D", NoiDung = "Thi thực hành cuối kỳ: Xây dựng App Flutter hoàn chỉnh", GhiChu = "Demo ứng dụng mobile", TrangThai = "Đã lên lịch" },
                new BuoiHoc { MaBuoiHoc = 19, MaLop = 14, ThoiGianBatDau = new DateTime(2026, 10, 17, 9, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 17, 11, 45, 0), PhongHoc = "P.402-HA8", GiangVien = "Ms. Sarah Jenkins", NoiDung = "Unit 08: Professional Interview & CV Preparation", GhiChu = "Phỏng vấn thử", TrangThai = "Đã lên lịch" },
                new BuoiHoc { MaBuoiHoc = 20, MaLop = 15, ThoiGianBatDau = new DateTime(2026, 10, 17, 16, 0, 0), ThoiGianKetThuc = new DateTime(2026, 10, 17, 18, 15, 0), PhongHoc = "Lab Mobile-03", GiangVien = "ThS. Phạm Minh D", NoiDung = "Bài 10: Đóng gói và phát hành ứng dụng lên Stores", GhiChu = "Build APK & IPA", TrangThai = "Đã lên lịch" },

                // Chủ nhật (2026-10-18) - 1 ca Đã Hủy
                new BuoiHoc { MaBuoiHoc = 21, MaLop = 1, ThoiGianBatDau = new DateTime(2026, 10, 18, 9, 30, 0), ThoiGianKetThuc = new DateTime(2026, 10, 18, 11, 45, 0), PhongHoc = "P.301-A1", GiangVien = "ThS. Nguyễn Văn A", NoiDung = "Lớp phụ đạo LINQ chuyên sâu (Tạm hoãn do giảng viên đi công tác)", GhiChu = "Sẽ xếp lịch bù sau", TrangThai = "Đã hủy" }
            };
            modelBuilder.Entity<BuoiHoc>().HasData(buoiHocs);

            var ketQuaHocTaps = new List<KetQuaHocTap>();
            int kqId = 1;
            for(int i=1; i<=45; i++) {
                if(trangThaiDK[(i-1)%5] == "Đang học" || trangThaiDK[(i-1)%5] == "Đã đóng học phí") {
                    ketQuaHocTaps.Add(new KetQuaHocTap {
                        MaKetQua = kqId++, MaDangKy = i, DiemChuyenCan = 8 + (i%3), DiemCuoiKhoa = 7 + (i%4), 
                        DiemTongKet = (8 + (i%3))*0.3 + (7 + (i%4))*0.7, 
                        KetQua = ((8 + (i%3))*0.3 + (7 + (i%4))*0.7) >= 5 ? "Hoàn thành" : "Không hoàn thành",
                        NhanXet = "Tốt", NgayCapNhat = new DateTime(2023, 5, 1).AddDays(i)
                    });
                }
            }
            modelBuilder.Entity<KetQuaHocTap>().HasData(ketQuaHocTaps);
        }
    }
}
