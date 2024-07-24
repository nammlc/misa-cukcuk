namespace MyWebApp.Models
{
    public class NhanVien
    {
        public int Id { get; set; }
        public string TenNhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public DateTime NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string ViTri { get; set; }
        public string SoCMND { get; set; }
        public DateTime NgayCapCMND { get; set; }
        public string NoiCapCMND { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiCoDinh { get; set; }
        public string Email { get; set; }
        public string SoTaiKhoanNganHang { get; set; }
        public string TenNganHang { get; set; }
        public string ChiNhanhNganHang { get; set; }
    }
}
