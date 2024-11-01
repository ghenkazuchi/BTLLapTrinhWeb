using BTLWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTLWEB.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly QlbanVaLiContext _context;
        public HoaDonController(QlbanVaLiContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            try
            {
                var tenuser = HttpContext.Session.GetString("UserName");
                TempData["dt"]= tenuser;
                if (tenuser != null)
                {
                    var kh = _context.TKhachHangs.SingleOrDefault(p => p.Username == tenuser);

                    if (kh != null)
                    {
                        var lstdonhang = _context.THoaDonBans
                            .AsNoTracking()
                            .Where(p => p.MaKhachHang == kh.MaKhanhHang)
                            .ToList();

                        return View(lstdonhang);
                    }
                }
                else
                {
                    return View("Home","Index");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return View("Error");
            }

            return View();
        }
        public IActionResult ChiTietHoaDon(string mhd)
        {
            var cthd = _context.TChiTietHdbs.Where(p => p.MaHoaDon == mhd).ToList();
            return View(cthd);
        }
    }
}
