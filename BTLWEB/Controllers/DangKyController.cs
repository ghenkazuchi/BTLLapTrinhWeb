using BTLWEB.Models;
using BTLWEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BTLWEB.Controllers
{
    public class DangKyController : Controller
    {   
        private readonly QlbanVaLiContext _context;
        public DangKyController(QlbanVaLiContext context) {
            _context = context;        
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var u = _context.TUsers.Where(x => x.Username.Equals(model.UserName) || x.Password.Equals(model.Password)).FirstOrDefault();
                if (u != null)
                {
                    TempData["loi"] = "tên đăng nhập hoặc mật khẩu đã được sử dụng !";
                }
                else
                {
                    var user = new TUser()
                    {
                        Username = model.UserName,
                        Password = model.Password,
                        LoaiUser = 0
                    };
                    _context.TUsers.Add(user);
                    _context.SaveChanges();
                    TempData["dk"] = "bạn đã đăng ký thành công";
                }
                return RedirectToAction("DangKy", "DangKy");
            }
            return View();
        }
    }
}
