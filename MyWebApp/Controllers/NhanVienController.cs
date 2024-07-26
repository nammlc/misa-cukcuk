using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Models;
using System.Linq;
using System.Threading.Tasks;

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

        // Hiển thị form thêm mới nhân viên
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                return RedirectToAction("Index", "Home");
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm nhân viên.";
            return RedirectToAction("Index", "Home");
        }

    }
}
