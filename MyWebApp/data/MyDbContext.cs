// File: Data/MyDbContext.cs
using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data
{
    public class MyDbContext : DbContext
    {
        public DbSet<NhanVien> NhanVien { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình thêm cho mô hình dữ liệu nếu cần
        }
    }
}

