using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Models;

namespace QuanLyDangKyKhoaHoc_UNETI04_TI17A1HN.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        if (vaiTro == "Admin" || vaiTro == "Giáo vụ")
        {
            return RedirectToAction("Dashboard", "ThongKe");
        }
        return RedirectToAction("DangKy", "CongHocVien");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
