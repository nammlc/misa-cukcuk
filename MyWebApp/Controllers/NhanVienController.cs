using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using X.PagedList;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace MyWebApp.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly MyDbContext _context;

        public NhanVienController(MyDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách nhân viên với phân trang
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 10;
            var nhanViens = await _context.NhanVien.ToListAsync();
            var pagedNhanViens = nhanViens.ToPagedList(page, pageSize);

            return View(pagedNhanViens);
        }

        // Hiển thị form tạo mới nhân viên
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý việc tạo mới nhân viên
        [HttpPost]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                return RedirectToAction("Index", "NhanVien");
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm nhân viên.";
            return View(nhanVien);
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var nhanVien = _context.NhanVien.Find(id);
            if (nhanVien == null)
            {
                return NotFound("Nhân viên không tồn tại.");
            }

            try
            {
                _context.NhanVien.Remove(nhanVien);
                _context.SaveChanges();
                return Ok("Xóa thành công.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi hệ thống: " + ex.Message);
            }
        }


    }
}
