using BTLWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BTLWEB.Controllers
{
    public class LoginController : Controller
    {   
        private readonly QlbanVaLiContext _context;
        public LoginController(QlbanVaLiContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        
        public IActionResult DangNhap(TUser user)
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                var u = _context.TUsers.Where(x => x.Username.Equals(user.Username) && x.Password.Equals(user.Password)).FirstOrDefault();
                if (u != null)
                {
                    HttpContext.Session.SetString("UserName", user.Username.ToString());
                    if (u.LoaiUser == 1)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["dn"] = "thông tin tài khoản hoặc mật khẩu không chính xác !";
                }
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("DangNhap", "Login");
        }
    }
}
