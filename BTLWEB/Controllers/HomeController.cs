using BTLWEB.Models;
using BTLWEB.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Diagnostics;
using X.PagedList;
namespace BTLWEB.Controllers
{
    public class HomeController : Controller
    {
        QlbanVaLiContext db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, QlbanVaLiContext context)
        {   
             db = context;
            _logger = logger;
        }
        
        public IActionResult Index(int? page)
        {
            int pagesize = 9;
            int pagenumber = page == null || page < 0 ? 1 : page.Value;
            var lstsanpham = db.TDanhMucSps.Include(x => x.MaLoaiNavigation).Include(x=>x.MaHangSxNavigation).Include(x=>x.MaDtNavigation).OrderBy(X => X.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(lstsanpham,pagenumber,pagesize);
            return View(lst);
        }

        public IActionResult SanPhamTheoLoai(string maloai, int? page)
        {
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstsanpham = db.TDanhMucSps.Include(x => x.MaLoaiNavigation).Include(x => x.MaHangSxNavigation).Include(x => x.MaDtNavigation).Where(x => x.MaLoai == maloai).OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(lstsanpham, pageNumber, pageSize);
            ViewBag.maloai = maloai;
            return View(lst);
        }
        public IActionResult ChiTietSanPham(string masp)
        {
            var sanpham= db.TDanhMucSps.Include(x => x.MaLoaiNavigation).Include(x => x.MaHangSxNavigation).Include(x => x.MaDtNavigation).SingleOrDefault(x=>x.MaSp==masp);
            var anhSanPham = db.TAnhSps.Where(x=>x.MaSp==masp).ToList();
            ViewBag.anhSanPham = anhSanPham;
            return View(sanpham);
        }
        public IActionResult TimKiemSp(string tensp,int? page)
        {

            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var lstsanpham = db.TDanhMucSps.Include(x => x.MaLoaiNavigation).Include(x => x.MaHangSxNavigation).Include(x => x.MaDtNavigation).Where(x => x.TenSp.Contains(tensp)).OrderBy(x => x.TenSp);
            PagedList<TDanhMucSp> lst = new PagedList<TDanhMucSp>(lstsanpham, pageNumber, pageSize);
            ViewBag.tensp=tensp;
            return View(lst);
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
}
