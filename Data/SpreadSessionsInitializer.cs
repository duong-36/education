// Họ và tên: Nguyễn Tùng Dương
// Mã sinh viên: 23103100003
// Nội dung thực hiện: Khởi tạo và phân bổ dữ liệu buổi học dàn trải theo tuần (Module 5)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Data
{
    public static class SpreadSessionsInitializer
    {
        public static async Task EnsureSpreadSessionsAsync(ApplicationDbContext context)
        {
            var lops = await context.LopHocs.ToListAsync();
            if (!lops.Any()) return;

            // Kiểm tra số lượng buổi học hiện tại
            var sampleMonday = new DateTime(2026, 10, 19);
            var sampleSunday = new DateTime(2026, 10, 25, 23, 59, 59);

            int currentWeekCount = await context.BuoiHocs
                .CountAsync(b => b.ThoiGianBatDau >= sampleMonday && b.ThoiGianBatDau <= sampleSunday);

            int totalBuoiHoc = await context.BuoiHocs.CountAsync();

            // Nếu đã có từ 120 buổi học trở lên và tuần hiện tại có nhiều hơn 5 buổi -> Đã chuẩn, không cần tạo lại
            if (totalBuoiHoc >= 120 && currentWeekCount >= 5) return;

            // Làm mới các buổi học để đảm bảo dàn trải đều 15 buổi cho tất cả các lớp qua 8 tuần liên tục
            if (totalBuoiHoc > 0)
            {
                context.BuoiHocs.RemoveRange(context.BuoiHocs);
                await context.SaveChangesAsync();
            }

            // Ngày Thứ 2 đầu tiên của tuần 1: 19/10/2026
            DateTime startWeek1 = new DateTime(2026, 10, 19);

            // Các mẫu lịch học theo ca thực tế tại UNETI:
            var schedulePatterns = new (DayOfWeek Day1, int Hour1, int Min1, DayOfWeek Day2, int Hour2, int Min2, string Phong, string Gv)[]
            {
                (DayOfWeek.Thursday, 7, 30, DayOfWeek.Monday, 7, 30, "Phòng hiệu năng cao 02/H.A9-507", "Lê Thị Thu Hiền"),
                (DayOfWeek.Tuesday, 13, 30, DayOfWeek.Friday, 13, 30, "Phòng Lab Web-01/H.A9-406", "Đoàn Tuấn Nam"),
                (DayOfWeek.Wednesday, 7, 30, DayOfWeek.Saturday, 7, 30, "Phòng Lab AI-02/H.A9-505", "Lê Hoàng C"),
                (DayOfWeek.Monday, 13, 30, DayOfWeek.Thursday, 13, 30, "Phòng 402/H.A8", "Lương Thị Thảo Hiếu"),
                (DayOfWeek.Friday, 7, 30, DayOfWeek.Saturday, 9, 45, "Phòng Lab Mobile/H.A9-506", "Trần Thị Hương"),
                (DayOfWeek.Tuesday, 7, 30, DayOfWeek.Friday, 9, 45, "Phòng học/H.A9-406", "Lê Thị Lý"),
                (DayOfWeek.Wednesday, 13, 30, DayOfWeek.Saturday, 13, 30, "Phòng Lab Cloud/H.A9-508", "Nguyễn Hoàng Long"),
                (DayOfWeek.Monday, 18, 0, DayOfWeek.Thursday, 18, 0, "Phòng Online Google Meet", "Nguyễn Tùng Dương")
            };

            string[] syllabus = new string[]
            {
                "Buổi 1: Giới thiệu khóa học, đề cương chi tiết & thiết lập môi trường phát triển",
                "Buổi 2: Kiến trúc nền tảng, cấu trúc dự án & các thành phần cốt lõi",
                "Buổi 3: Quản lý cấu hình, Dependency Injection & Service Lifecycle",
                "Buổi 4: Entity Framework Core, thiết kế CSDL Code-First & Migrations",
                "Buổi 5: Xây dựng RESTful API, Controller & Routing nâng cao",
                "Buổi 6: Kỹ thuật truy vấn tối ưu LINQ to Entities & Data Mapping",
                "Buổi 7: Kiểm tra đánh giá năng lực giữa kỳ & chữa bài tập thực hành",
                "Buổi 8: Thiết kế giao diện người dùng tương tác với Tailwind CSS",
                "Buổi 9: Tích hợp Frontend Component & Quản lý State",
                "Buổi 10: Xử lý nghiệp vụ chuyên sâu & Kiểm soát tính toàn vẹn dữ liệu",
                "Buổi 11: Lập trình bất đồng bộ (Async/Await) & Tác vụ nền (Background Tasks)",
                "Buổi 12: Kiểm thử tự động (Unit Test & Integration Test)",
                "Buổi 13: Bảo mật ứng dụng, phân quyền người dùng & chống lỗ hổng OWASP",
                "Buổi 14: Quy trình đóng gói Docker Container & Triển khai Cloud Server",
                "Buổi 15: Báo cáo đồ án tốt nghiệp, đánh giá kết quả & tổng kết khóa học"
            };

            var newBuoiHocs = new List<BuoiHoc>();

            foreach (var lop in lops)
            {
                var pat = schedulePatterns[(lop.MaLop - 1) % schedulePatterns.Length];
                int totalSessions = lop.SoBuoiHoc > 0 ? lop.SoBuoiHoc : 15;
                if (totalSessions > 15) totalSessions = 15;

                int sessionIdx = 0;
                for (int week = 0; week < 8 && sessionIdx < totalSessions; week++)
                {
                    DateTime mondayOfWeek = startWeek1.AddDays(week * 7);

                    // Buổi thứ nhất trong tuần (Day1)
                    int dayOffset1 = (7 + (pat.Day1 - DayOfWeek.Monday)) % 7;
                    DateTime dt1 = mondayOfWeek.AddDays(dayOffset1).AddHours(pat.Hour1).AddMinutes(pat.Min1);
                    DateTime dtEnd1 = dt1.AddHours(4).AddMinutes(15);

                    string status1 = dt1.Date < DateTime.Today ? "Đã hoàn thành" :
                                     dt1.Date == DateTime.Today ? "Đang diễn ra" : "Đã lên lịch";

                    newBuoiHocs.Add(new BuoiHoc
                    {
                        MaLop = lop.MaLop,
                        ThoiGianBatDau = dt1,
                        ThoiGianKetThuc = dtEnd1,
                        PhongHoc = pat.Phong,
                        GiangVien = string.IsNullOrWhiteSpace(lop.GiangVien) ? pat.Gv : lop.GiangVien,
                        NoiDung = syllabus[sessionIdx],
                        GhiChu = $"Buổi {sessionIdx + 1}/{totalSessions} - Tuần {week + 1}",
                        TrangThai = status1
                    });
                    sessionIdx++;

                    if (sessionIdx >= totalSessions) break;

                    // Buổi thứ hai trong tuần (Day2)
                    int dayOffset2 = (7 + (pat.Day2 - DayOfWeek.Monday)) % 7;
                    DateTime dt2 = mondayOfWeek.AddDays(dayOffset2).AddHours(pat.Hour2).AddMinutes(pat.Min2);
                    DateTime dtEnd2 = dt2.AddHours(4).AddMinutes(15);

                    string status2 = dt2.Date < DateTime.Today ? "Đã hoàn thành" :
                                     dt2.Date == DateTime.Today ? "Đang diễn ra" : "Đã lên lịch";

                    newBuoiHocs.Add(new BuoiHoc
                    {
                        MaLop = lop.MaLop,
                        ThoiGianBatDau = dt2,
                        ThoiGianKetThuc = dtEnd2,
                        PhongHoc = pat.Phong,
                        GiangVien = string.IsNullOrWhiteSpace(lop.GiangVien) ? pat.Gv : lop.GiangVien,
                        NoiDung = syllabus[sessionIdx],
                        GhiChu = $"Buổi {sessionIdx + 1}/{totalSessions} - Tuần {week + 1}",
                        TrangThai = status2
                    });
                    sessionIdx++;
                }
            }

            await context.BuoiHocs.AddRangeAsync(newBuoiHocs);
            await context.SaveChangesAsync();
        }
    }
}
