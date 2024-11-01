using Azure;
using BTLWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using X.PagedList;

namespace BTLWEB.Areas.Areas.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        QlbanVaLiContext db = new QlbanVaLiContext();
        private readonly IWebHostEnvironment env;
        public HomeAdminController(IWebHostEnvironment _env)
        {
            env = _env;
        }
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            var monthlyRevenue = db.THoaDonBans
                .Where(hd => hd.NgayHoaDon.HasValue) // Only consider entries with a date
                .GroupBy(hd => new { Month = hd.NgayHoaDon.Value.Month, Year = hd.NgayHoaDon.Value.Year })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalRevenue = g.Sum(hd => hd.TongTienHd) // Sum revenue for each group
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            // Extract labels (months) and data (revenues)
            var months = monthlyRevenue.Select(mr => $"{mr.Month}/{mr.Year}").ToList();
            var revenues = monthlyRevenue.Select(mr => mr.TotalRevenue ?? 0).ToList();

            ViewBag.Months = months;
            ViewBag.Revenues = revenues;

            // Calculate total revenue and total orders
            var totalRevenue = db.THoaDonBans.Sum(hd => hd.TongTienHd);
            var totalOrders = db.THoaDonBans.Count();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            return View();
        }
        [Route("danhmucsanpham")]
        public IActionResult DanhMucSanPham(int? page, string searchString, decimal? minPrice, decimal? maxPrice, string productCode)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            ViewBag.ProductCodes = db.TDanhMucSps.Select(x => x.MaSp).Distinct().ToList();

            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var lstSanPham = db.TDanhMucSps.AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                lstSanPham = lstSanPham.Where(x => x.TenSp.Contains(searchString));
            }
            if (minPrice.HasValue)
            {
                lstSanPham = lstSanPham.Where(x => x.GiaNhoNhat >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                lstSanPham = lstSanPham.Where(x => x.GiaLonNhat <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(productCode))
            {
                lstSanPham = lstSanPham.Where(x => x.MaSp == productCode);
            }

            var orderedList = lstSanPham.OrderBy(x => x.TenSp);

            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(orderedList.ToList(), pageNumber, pageSize);
            return View(lst);
        }

        [Route("themsanpham")]
        [HttpGet]
        public IActionResult ThemSanPham()
        {
            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            return View();
        }
        [Route("themsanpham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPham(TDanhMucSp sp, IFormFile AnhDaiDien)
        {
            if (ModelState.IsValid)
            {
                if (AnhDaiDien != null && AnhDaiDien.Length > 0)
                {
                    string uploadFolder = Path.Combine(env.WebRootPath, "Images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + AnhDaiDien.FileName;
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        AnhDaiDien.CopyTo(fileStream);
                    }

                    sp.AnhDaiDien = uniqueFileName;
                }

                db.TDanhMucSps.Add(sp);
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham");
            }

            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai");
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc");
            return View();
        }

        [Route("edit/{id}")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var sanPham = db.TDanhMucSps.Find(id);

            if (sanPham == null)
            {
                return NotFound();
            }

            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai", sanPham.MaLoai);
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc", sanPham.MaNuocSx);

            return View(sanPham);
        }


        [Route("edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, TDanhMucSp sanPham, IFormFile AnhDaiDien)
        {
            if (id != sanPham.MaSp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (AnhDaiDien != null && AnhDaiDien.Length > 0)
                    {
                        string uploadFolder = Path.Combine(env.WebRootPath, "Images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + AnhDaiDien.FileName;
                        string filePath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            AnhDaiDien.CopyTo(fileStream);
                        }

                        sanPham.AnhDaiDien = uniqueFileName;
                    }

                    db.Entry(sanPham).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TDanhMucSpExists(sanPham.MaSp))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("DanhMucSanPham");
            }

            ViewBag.MaLoai = new SelectList(db.TLoaiSps.ToList(), "MaLoai", "Loai", sanPham.MaLoai);
            ViewBag.MaNuocSx = new SelectList(db.TQuocGia.ToList(), "MaNuoc", "TenNuoc", sanPham.MaNuocSx);

            return View(sanPham);
        }
        [Route("details/{id}")]
        [HttpGet]
        public IActionResult Details(string id)
        {
            var sanPham = db.TDanhMucSps.Find(id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }
        private bool TDanhMucSpExists(string id)
        {
            return db.TDanhMucSps.Any(e => e.MaSp == id);
        }

        [Route("XoaSanPham")]
        [HttpGet]
        public IActionResult XoaSanPham(string maSanPham)
        {
            TempData["Message"] = "";
            var chiTietSanPham = db.TChiTietSanPhams.Where(x => x.MaSp == maSanPham).ToList();
            if (chiTietSanPham.Count > 0)
            {
                TempData["Message"] = "Can't Delete";
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            var anhSanPham = db.TAnhSps.Where(x => x.MaSp == maSanPham);
            if (anhSanPham.Any())
            {
                db.RemoveRange(anhSanPham);
            }
            db.Remove(db.TDanhMucSps.Find(maSanPham));
            db.SaveChanges();
            TempData["Message"] = "Item has been deleted";
            return RedirectToAction("DanhMucSanPham", "HomeAdmin");
        }

        //KhachHang
        [Route("DanhSachKhachHang")]
        public IActionResult DanhSachKhachHang(int? page, string searchString)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var lstKhachHang = db.TKhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                lstKhachHang = lstKhachHang.Where(kh => kh.TenKhachHang.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            var pagedListKhachHang = lstKhachHang.OrderBy(kh => kh.TenKhachHang).ToPagedList(pageNumber, pageSize);

            return View(pagedListKhachHang);
        }

        [Route("detailsKhachHang/{id}")]
        [HttpGet]
        public IActionResult DetailsKhachHang(string id)
        {
            var khachHang = db.TKhachHangs.Find(id);

            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        [Route("editKhachHang")]
        [HttpGet]
        public IActionResult EditKhachHang(string id)
        {
            var khachHang = db.TKhachHangs.Find(id);

            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }
        [Route("editKhachHang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditKhachHang(TKhachHang khachHang)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(khachHang).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DanhSachKhachHang");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TKhachHangExists(khachHang.MaKhanhHang))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(khachHang);
        }

        private bool TKhachHangExists(string id)
        {
            return db.TKhachHangs.Any(e => e.MaKhanhHang == id);
        }

        [Route("deletekhachhang/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteKhachHang(string id)
        {
            var khachHang = db.TKhachHangs.Find(id);
            if (khachHang == null)
            {
                TempData["Message"] = "Customer not found!";
                return RedirectToAction("DanhSachKhachHang");
            }

            db.TKhachHangs.Remove(khachHang);
            db.SaveChanges();

            TempData["Message"] = "Customer deleted successfully!";
            return RedirectToAction("DanhSachKhachHang");
        }
        [Route("DanhSachHoaDon")]
        public IActionResult DanhSachHoaDon(int? page, string searchString)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var lstHoaDon = db.THoaDonBans.Include(hd => hd.MaKhachHangNavigation).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                lstHoaDon = lstHoaDon.Where(hd => hd.MaHoaDon.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            var pagedListHoaDon = lstHoaDon.OrderBy(hd => hd.NgayHoaDon).ToPagedList(pageNumber, pageSize);

            return View(pagedListHoaDon);
        }
        [Route("DanhSachHoaDon/ChiTietHoaDon/{id}")]
        [HttpGet]
        public IActionResult ChiTietHoaDon(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var hoaDon = db.THoaDonBans
                .Include(hd => hd.TChiTietHdbs)
                .ThenInclude(ct => ct.MaChiTietSpNavigation)
                .FirstOrDefault(hd => hd.MaHoaDon == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }
        [Route("DanhSachNhanVien")]
        public IActionResult DanhSachNhanVien(int? page, string searchString)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var lstNhanVien = db.TNhanViens.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                lstNhanVien = lstNhanVien.Where(nv => nv.TenNhanVien.Contains(searchString));
                ViewBag.SearchString = searchString;
            }

            var pagedListNhanVien = lstNhanVien.OrderBy(nv => nv.TenNhanVien).ToPagedList(pageNumber, pageSize);

            return View(pagedListNhanVien);
        }
        [Route("editNhanVien/{id}")]
        [HttpGet]
        public IActionResult EditNhanVien(string id)
        {
            var nhanVien = db.TNhanViens.Find(id);
            if (nhanVien == null)
            {
                return NotFound();
            }

            return View(nhanVien);
        }
        [Route("editNhanVien/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNhanVien(TNhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(nhanVien).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DanhSachNhanVien");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TNhanVienExists(nhanVien.MaNhanVien))
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
        private bool TNhanVienExists(string id)
        {
            return db.TNhanViens.Any(e => e.MaNhanVien == id);
        }

        [Route("detailsNhanVien/{id}")]
        [HttpGet]
        public IActionResult DetailsNhanVien(string id)
        {
            var nhanVien = db.TNhanViens.Find(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            return View(nhanVien);
        }
        [Route("deleteNhanVien/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteNhanVien(string id)
        {
            var nhanVien = db.TNhanViens.Find(id);
            if (nhanVien == null)
            {
                TempData["Message"] = "Employee not found!";
                return RedirectToAction("DanhSachNhanVien");
            }

            db.TNhanViens.Remove(nhanVien);
            db.SaveChanges();

            TempData["Message"] = "Employee deleted successfully!";
            return RedirectToAction("DanhSachNhanVien");
        }

    }
}
