using Microsoft.AspNetCore.Mvc;
using Dapper;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Linq;
using X.PagedList;
using MyWebApp.Models;
using X.PagedList.Extensions;

namespace MyWebApp.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly string _connectionString;

        public NhanVienController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Hiển thị danh sách nhân viên với phân trang
        public async Task<IActionResult> Index(string searchQuery, int page = 1)
        {
            int pageSize = 10;
            using (var db = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM NhanVien WHERE @SearchQuery IS NULL OR ma_nhan_vien LIKE @SearchQuery OR ten_nhan_vien LIKE @SearchQuery";
                var nhanViens = (await db.QueryAsync<NhanVien>(query, new { SearchQuery = "%" + searchQuery + "%" })).ToList();
                var pagedNhanViens = nhanViens.ToPagedList(page, pageSize);
                ViewBag.SearchQuery = searchQuery;
                return View(pagedNhanViens);
            }
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
                nhanVien.ma_nhan_vien = GenerateUniqueEmployeeCode();

                using (var db = new MySqlConnection(_connectionString))
                {
                    string query = "INSERT INTO NhanVien (ten_nhan_vien, ma_nhan_vien, ngay_sinh, gioi_tinh, vi_tri, so_cmnd, ngay_cap_cmnd, noi_cap_cmnd, dia_chi, so_dien_thoai, so_dien_thoai_co_dinh, email, so_tai_khoan_ngan_hang, ten_ngan_hang, chi_nhanh_ngan_hang) VALUES (@ten_nhan_vien, @ma_nhan_vien, @ngay_sinh, @gioi_tinh, @vi_tri, @so_cmnd, @ngay_cap_cmnd, @noi_cap_cmnd, @dia_chi, @so_dien_thoai, @so_dien_thoai_co_dinh, @email, @so_tai_khoan_ngan_hang, @ten_ngan_hang, @chi_nhanh_ngan_hang)";
                    await db.ExecuteAsync(query, nhanVien);
                }

                TempData["SuccessMessage"] = "Thêm nhân viên thành công!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm nhân viên.";
            return View(nhanVien);
        }

        // Phương thức để tạo mã nhân viên tự động duy nhất
        private string GenerateUniqueEmployeeCode()
        {
            using (var db = new MySqlConnection(_connectionString))
            {
                string query = "SELECT ma_nhan_vien FROM NhanVien ORDER BY ma_nhan_vien DESC LIMIT 1";
                var lastEmployeeCode = db.QueryFirstOrDefault<string>(query);

                if (lastEmployeeCode == null)
                {
                    return "NV001";
                }

                int newCodeNumber = int.Parse(lastEmployeeCode.Substring(2)) + 1;
                return "NV" + newCodeNumber.ToString("D3");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            using (var db = new MySqlConnection(_connectionString))
            {
                string query = "DELETE FROM NhanVien WHERE id = @Id";
                var affectedRows = await db.ExecuteAsync(query, new { Id = id });

                if (affectedRows == 0)
                {
                    return NotFound("Nhân viên không tồn tại.");
                }

                return Ok("Xóa thành công.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using (var db = new MySqlConnection(_connectionString))
            {
                string query = "SELECT * FROM NhanVien WHERE id = @Id";
                var nhanVien = await db.QueryFirstOrDefaultAsync<NhanVien>(query, new { Id = id });

                if (nhanVien == null)
                {
                    return NotFound();
                }
                return PartialView("_EditEmployeeModal", nhanVien);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhanVien nhanVien)
        {
            if (id != nhanVien.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                using (var db = new MySqlConnection(_connectionString))
                {
                    string query = "UPDATE NhanVien SET ten_nhan_vien = @ten_nhan_vien, ma_nhan_vien = @ma_nhan_vien, ngay_sinh = @ngay_sinh, gioi_tinh = @gioi_tinh, vi_tri = @vi_tri, so_cmnd = @so_cmnd, ngay_cap_cmnd = @ngay_cap_cmnd, noi_cap_cmnd = @noi_cap_cmnd, dia_chi = @dia_chi, so_dien_thoai = @so_dien_thoai, so_dien_thoai_co_dinh = @so_dien_thoai_co_dinh, email = @email, so_tai_khoan_ngan_hang = @so_tai_khoan_ngan_hang, ten_ngan_hang = @ten_ngan_hang, chi_nhanh_ngan_hang = @chi_nhanh_ngan_hang WHERE id = @id";
                    var affectedRows = await db.ExecuteAsync(query, nhanVien);

                    if (affectedRows == 0)
                    {
                        return NotFound();
                    }

                    TempData["SuccessMessage"] = "Cập nhật nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(nhanVien);
        }
    }
}
