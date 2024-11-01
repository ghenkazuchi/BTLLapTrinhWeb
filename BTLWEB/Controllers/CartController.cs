using Microsoft.AspNetCore.Mvc;
using BTLWEB.Helpers;
using BTLWEB.Models;
using BTLWEB.ViewModels;
using System.Drawing.Printing;
using Microsoft.EntityFrameworkCore;
namespace BTLWEB.Controllers
{
    public class CartController : Controller
    {   
        private readonly QlbanVaLiContext _context;

        public CartController(QlbanVaLiContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(Cart);
        }
        const string CART_KEY = "MYCART";
        public List<CartItem> Cart
        {
            get
            {
                return HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new
                List<CartItem>();
            }
        }
        public IActionResult AddToCart(string masp, int soluong = 1)
        {
            var giohang = Cart;
            var item = giohang.SingleOrDefault(p => p.maSp == masp);
            if (item == null) {
                var sanpham = _context.TDanhMucSps.SingleOrDefault(p => p.MaSp == masp);
                item = new CartItem
                {
                    maSp = sanpham.MaSp,
                    tensp = sanpham.TenSp,
                    anhDaiDien = sanpham.AnhDaiDien,
                    Dongia = sanpham.GiaLonNhat ?? 0,
                    SoLuong = soluong
                };
                giohang.Add(item);
            }
            else
            {
                item.SoLuong += soluong;

            }
            HttpContext.Session.Set(CART_KEY, giohang);
            return RedirectToAction("Index");
        }
        public IActionResult RemoveCart(string masp)
        {
            var giohang = Cart;
            var item = giohang.SingleOrDefault(p => p.maSp == masp);
            if (item != null)
            {
                giohang.Remove(item);
                HttpContext.Session.Set(CART_KEY,giohang);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult CheckOut()
        {
            var giohang = Cart;
            if(giohang.Count == 0)
            {
                return Redirect("/");
            }
            return View(Cart);
        }
        public static string GenerateRandomCode(int length)
        {
            const string chars = "0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                                  .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        [HttpPost]
        public  IActionResult CheckOut(CheckOutVM model)
        {   
            if (ModelState.IsValid)
            {
                
                var khachhang = new TKhachHang
                {
                    MaKhanhHang = "Kh" + GenerateRandomCode(5),
                    TenKhachHang = model.tenkhachhang,
                    Username = HttpContext.Session.GetString("UserName"),
                    DiaChi = model.diachi,
                    SoDienThoai = model.sodienthoai,
                    GhiChu = model.ghichu,
                    CachThanhToan = "COD",
                    CachVanChuyen = "GHTK",
                    TrangThai = "Đang xử lý",
                    LoaiKhachHang = 0,
                    NgayDat = DateOnly.FromDateTime(DateTime.Now)
                };
                var u = _context.TKhachHangs.Where(p => p.Username == khachhang.Username).FirstOrDefault();
                if(u== null)
                {
                    _context.TKhachHangs.Add(khachhang);
                    _context.SaveChanges();
                }
                var kh = _context.TKhachHangs.Where(p => p.Username == khachhang.Username).FirstOrDefault();
                var hoadon = new THoaDonBan
                {
                    MaKhachHang = kh.MaKhanhHang,
                    MaHoaDon = "HD" + GenerateRandomCode(5),
                    TongTienHd = Cart.Sum(p => p.ThanhTien),
                    PhuongThucThanhToan = "Thanh toán khi nhận hàng",
                    TrangThai = "Đang Giao Hàng",
                    NgayHoaDon = DateTime.Now,
                    GhiChu = kh.GhiChu,
                   
                };
                _context.Database.BeginTransaction();
                try
                {
                   
                    _context.THoaDonBans.Add(hoadon);
                    
                    _context.SaveChanges();
                    var cthd = new List<TChiTietHdb>();
                    foreach(var item in Cart)
                    {
                        cthd.Add(new TChiTietHdb
                        {
                            MaHoaDon = hoadon.MaHoaDon,
                            SoLuongBan = item.SoLuong,
                            DonGiaBan = item.Dongia,
                            MaChiTietSp= "atud20230001gr",
                            TenSp = item.tensp,

                        });
                    }
                    _context.AddRange(cthd);
                    _context.SaveChanges();
                    _context.Database.CommitTransaction();
                }
                catch
                {
                    _context.Database.RollbackTransaction();
                }
               
                HttpContext.Session.Set<List<CartItem>>(CART_KEY,new List<CartItem>());
                return View("Success");
            }
            return View(model);
        }
    }
}
