using BTLWEB.Helpers;
using BTLWEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BTLWEB.ViewComponents
{
    public class CartVIewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("MYCART") ?? new List<CartItem>();
            return View("CartPanel",new CartModel
            {
                soluong = cart.Sum(p=>p.SoLuong),
                tien = cart.Sum(p=>p.ThanhTien),
            });
        }
    }
}
