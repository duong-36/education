using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataNonConflicting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 30);

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 1,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Phòng máy A1", "ThS. Nguyễn Văn A", "Bài 03: Kiến trúc ASP.NET Core MVC & Dependency Injection", "P.301-A1", new DateTime(2026, 10, 12, 7, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 12, 9, 15, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 2,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Thực hành phòng Lab", "TS. Trần Thị B", "Bài 04: React Hooks & Quản lý State với Context API", "Lab Web-01", new DateTime(2026, 10, 12, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 12, 11, 45, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 3,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Môi trường Jupyter Lab", "ThS. Lê Hoàng C", "Bài 05: Phân tích và xử lý dữ liệu với thư viện Pandas", "Lab AI-02", new DateTime(2026, 10, 12, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 12, 15, 45, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 4,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Phòng đa phương tiện", "Ms. Sarah Jenkins", "Unit 04: Business Presentation & Workplace Communication", "P.402-HA8", new DateTime(2026, 10, 13, 7, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 13, 9, 15, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 5,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Máy Mac + Emulator", "ThS. Phạm Minh D", "Bài 03: Thiết kế UI Widgets & Navigation trong Flutter", "Lab Mobile-03", new DateTime(2026, 10, 13, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 13, 11, 45, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 6,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Thực hành SQL Server", "ThS. Nguyễn Văn A", "Bài 06: Entity Framework Core Code-First & LINQ Queries", "P.301-A1", new DateTime(2026, 10, 13, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 13, 15, 45, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 7,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Thực hành API", "TS. Trần Thị B", "Bài 07: Tích hợp RESTful API với Axios & Async Thunk", "Lab Web-01", new DateTime(2026, 10, 13, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 13, 18, 15, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 8,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Thực hành đồ thị", "ThS. Lê Hoàng C", "Bài 08: Trực quan hóa dữ liệu trực quan với Matplotlib & Seaborn", "Lab AI-02", new DateTime(2026, 10, 14, 7, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 14, 9, 15, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 9,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Thi trắc nghiệm + Code trực tiếp", "ThS. Nguyễn Văn A", 1, "Kiểm tra giữa kỳ: LINQ Queries & Entity Framework Core", "P.301-A1", new DateTime(2026, 10, 14, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 14, 11, 45, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 10,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Văn bản thương mại", "Ms. Sarah Jenkins", 9, "Unit 06: Email Writing & Business Negotiation Skills", "P.402-HA8", new DateTime(2026, 10, 14, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 14, 15, 45, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 11,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Kiến trúc Bloc", "ThS. Phạm Minh D", 10, "Bài 07: Quản lý State nâng cao với Bloc / Cubit", "Lab Mobile-03", new DateTime(2026, 10, 15, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 15, 11, 45, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 12,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Thi thực hành 90 phút", "TS. Trần Thị B", 2, "Kiểm tra thực hành: Xây dựng SPA React Component & Hooks", "Lab Web-01", new DateTime(2026, 10, 15, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 15, 15, 45, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 13,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Bảo mật ứng dụng", "TS. Vũ Đình E", 11, "Bài 05: Xác thực và phân quyền với ASP.NET Core Identity", "P.301-A1", new DateTime(2026, 10, 15, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 15, 18, 15, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 14,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Vấn đáp 1-1", "Ms. Sarah Jenkins", 4, "Thi đánh giá năng lực: Tiếng Anh Giao tiếp B1 (Vấn đáp)", "P.402-HA8", new DateTime(2026, 10, 16, 7, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 16, 9, 15, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 15,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Tối ưu render", "TS. Trần Thị B", 12, "Bài 08: Tối ưu hiệu năng React với useMemo, useCallback & Lazy", "Lab Web-01", new DateTime(2026, 10, 16, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 16, 11, 45, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 16,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Báo cáo tiến độ", "ThS. Lê Hoàng C", 3, "Kiểm tra tiến độ đồ án: Huấn luyện mô hình Machine Learning", "Lab AI-02", new DateTime(2026, 10, 16, 13, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 16, 15, 45, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 17,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Tokenization & TF-IDF", "ThS. Lê Hoàng C", 13, "Bài 09: Xử lý ngôn ngữ tự nhiên cơ bản với thư viện NLTK", "Lab AI-02", new DateTime(2026, 10, 16, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 16, 18, 15, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 18,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Demo ứng dụng mobile", "ThS. Phạm Minh D", 5, "Thi thực hành cuối kỳ: Xây dựng App Flutter hoàn chỉnh", "Lab Mobile-03", new DateTime(2026, 10, 17, 7, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 17, 9, 15, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 19,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "Phỏng vấn thử", "Ms. Sarah Jenkins", 14, "Unit 08: Professional Interview & CV Preparation", "P.402-HA8", new DateTime(2026, 10, 17, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 17, 11, 45, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 20,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Build APK & IPA", "ThS. Phạm Minh D", 15, "Bài 10: Đóng gói và phát hành ứng dụng lên Stores", "Lab Mobile-03", new DateTime(2026, 10, 17, 16, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 17, 18, 15, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 21,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "Sẽ xếp lịch bù sau", "ThS. Nguyễn Văn A", 1, "Lớp phụ đạo LINQ chuyên sâu (Tạm hoãn do giảng viên đi công tác)", "P.301-A1", new DateTime(2026, 10, 18, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 18, 11, 45, 0, 0, DateTimeKind.Unspecified), "Đã hủy" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 1,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 2,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 4,
                column: "TrangThai",
                value: "Đã đóng học phí");

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 5,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 1, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 6,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 1, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 7,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 9,
                column: "TrangThai",
                value: "Đã đóng học phí");

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 10,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 1, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 11,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 12,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 14,
                column: "TrangThai",
                value: "Đã đóng học phí");

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 15,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 5000000.0, "Đã đóng học phí" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 1,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 2", "Buổi học 1", "Phòng 101", new DateTime(2023, 2, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 2, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 2,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 3", "Buổi học 2", "Phòng 102", new DateTime(2023, 2, 3, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 3, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 3,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 1", "Buổi học 3", "Phòng 103", new DateTime(2023, 2, 4, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 4, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 4,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 2", "Buổi học 4", "Phòng 104", new DateTime(2023, 2, 5, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 5, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 5,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 3", "Buổi học 5", "Phòng 100", new DateTime(2023, 2, 6, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 6, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 6,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 1", "Buổi học 6", "Phòng 101", new DateTime(2023, 2, 7, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 7, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 7,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 2", "Buổi học 7", "Phòng 102", new DateTime(2023, 2, 8, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 8, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 8,
                columns: new[] { "GhiChu", "GiangVien", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 3", "Buổi học 8", "Phòng 103", new DateTime(2023, 2, 9, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 9, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 9,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 1", 9, "Buổi học 9", "Phòng 104", new DateTime(2023, 2, 10, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 10, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 10,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 2", 10, "Buổi học 10", "Phòng 100", new DateTime(2023, 2, 11, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 11, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 11,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 3", 11, "Buổi học 11", "Phòng 101", new DateTime(2023, 2, 12, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 12, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 12,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 1", 12, "Buổi học 12", "Phòng 102", new DateTime(2023, 2, 13, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 13, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 13,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 2", 13, "Buổi học 13", "Phòng 103", new DateTime(2023, 2, 14, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 14, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 14,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 3", 14, "Buổi học 14", "Phòng 104", new DateTime(2023, 2, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 15, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 15,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 1", 15, "Buổi học 15", "Phòng 100", new DateTime(2023, 2, 16, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 16, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 16,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 2", 1, "Buổi học 16", "Phòng 101", new DateTime(2023, 2, 17, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 17, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 17,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 3", 2, "Buổi học 17", "Phòng 102", new DateTime(2023, 2, 18, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 18, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 18,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 1", 3, "Buổi học 18", "Phòng 103", new DateTime(2023, 2, 19, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 19, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 19,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc" },
                values: new object[] { "", "Giảng viên 2", 4, "Buổi học 19", "Phòng 104", new DateTime(2023, 2, 20, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 20, 21, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 20,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 3", 5, "Buổi học 20", "Phòng 100", new DateTime(2023, 2, 21, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 21, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" });

            migrationBuilder.UpdateData(
                table: "BuoiHocs",
                keyColumn: "MaBuoiHoc",
                keyValue: 21,
                columns: new[] { "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[] { "", "Giảng viên 1", 6, "Buổi học 21", "Phòng 101", new DateTime(2023, 2, 22, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 22, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" });

            migrationBuilder.InsertData(
                table: "BuoiHocs",
                columns: new[] { "MaBuoiHoc", "GhiChu", "GiangVien", "MaLop", "NoiDung", "PhongHoc", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[,]
                {
                    { 22, "", "Giảng viên 2", 7, "Buổi học 22", "Phòng 102", new DateTime(2023, 2, 23, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 23, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 23, "", "Giảng viên 3", 8, "Buổi học 23", "Phòng 103", new DateTime(2023, 2, 24, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 24, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 24, "", "Giảng viên 1", 9, "Buổi học 24", "Phòng 104", new DateTime(2023, 2, 25, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 25, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 25, "", "Giảng viên 2", 10, "Buổi học 25", "Phòng 100", new DateTime(2023, 2, 26, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 26, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 26, "", "Giảng viên 3", 11, "Buổi học 26", "Phòng 101", new DateTime(2023, 2, 27, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 27, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 27, "", "Giảng viên 1", 12, "Buổi học 27", "Phòng 102", new DateTime(2023, 2, 28, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 28, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 28, "", "Giảng viên 2", 13, "Buổi học 28", "Phòng 103", new DateTime(2023, 3, 1, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 1, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 29, "", "Giảng viên 3", 14, "Buổi học 29", "Phòng 104", new DateTime(2023, 3, 2, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 2, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 30, "", "Giảng viên 1", 15, "Buổi học 30", "Phòng 100", new DateTime(2023, 3, 3, 18, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 3, 3, 21, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" }
                });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 1,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Chờ duyệt" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 2,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Đã duyệt" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 4,
                column: "TrangThai",
                value: "Đang học");

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 5,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Đã hủy" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 6,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Chờ duyệt" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 7,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Đã duyệt" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 9,
                column: "TrangThai",
                value: "Đang học");

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 10,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Đã hủy" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 11,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Chờ duyệt" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 12,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Đã duyệt" });

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 14,
                column: "TrangThai",
                value: "Đang học");

            migrationBuilder.UpdateData(
                table: "DangKyHocs",
                keyColumn: "MaDangKy",
                keyValue: 15,
                columns: new[] { "NgayDongHocPhi", "SoTienDaDong", "TrangThai" },
                values: new object[] { null, 0.0, "Đã hủy" });
        }
    }
}
