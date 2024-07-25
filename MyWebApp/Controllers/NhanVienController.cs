// File: Controllers/NhanVienController.cs
using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using System.Linq;

namespace MyWebApp.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly MyDbContext _context;

        public NhanVienController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var nhanViens = _context.NhanVien.ToList();
            return View(nhanViens);
        }

        public IActionResult CheckConnection()
        {
            try
            {
                _context.Database.CanConnect();
                return Content("Kết nối cơ sở dữ liệu thành công.");
            }
            catch (Exception ex)
            {
                return Content($"Lỗi kết nối cơ sở dữ liệu: {ex.Message}");
            }
        }
    }
}