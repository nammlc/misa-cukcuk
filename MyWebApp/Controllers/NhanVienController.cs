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
        public async Task<IActionResult> Index(string searchQuery, int page = 1)
        {
            int pageSize = 10;
            var nhanViens = _context.NhanVien.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                nhanViens = nhanViens.Where(nv => nv.ma_nhan_vien.Contains(searchQuery) || nv.ten_nhan_vien.Contains(searchQuery));
            }

            var pagedNhanViens = nhanViens.ToPagedList(page, pageSize);
            ViewBag.SearchQuery = searchQuery;

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
                // Tạo mã nhân viên tự động
                nhanVien.ma_nhan_vien = GenerateUniqueEmployeeCode();

                _context.Add(nhanVien);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm nhân viên.";
            return View(nhanVien);
        }

        // Phương thức để tạo mã nhân viên tự động duy nhất
        private string GenerateUniqueEmployeeCode()
        {
            var lastEmployee = _context.NhanVien
                .OrderByDescending(e => e.ma_nhan_vien)
                .FirstOrDefault();

            if (lastEmployee == null)
            {
                return "NV001"; // Hoặc một mã khởi đầu khác nếu không có nhân viên nào
            }

            string lastCode = lastEmployee.ma_nhan_vien;
            string newCode = "NV" + (int.Parse(lastCode.Substring(2)) + 1).ToString("D3");

            return newCode;
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var nhanVien = await _context.NhanVien.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return PartialView("_EditEmployeeModal", nhanVien); // Return the view with the employee data
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,ten_nhan_vien,ma_nhan_vien,ngay_sinh,gioi_tinh,vi_tri,so_cmnd,ngay_cap_cmnd,noi_cap_cmnd,dia_chi,so_dien_thoai,so_dien_thoai_co_dinh,email,so_tai_khoan_ngan_hang,ten_ngan_hang,chi_nhanh_ngan_hang")] NhanVien nhanVien)
        {
            if (id != nhanVien.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVien);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienExists(nhanVien.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(nhanVien);
        }


        private bool NhanVienExists(int id)
        {
            return _context.NhanVien.Any(e => e.id == id);
        }



    }
}
