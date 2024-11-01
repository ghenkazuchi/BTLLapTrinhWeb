using BTLWEB.Models;
using BTLWEB.Repository;
using Microsoft.AspNetCore.Mvc;
namespace BTLWEB.ViewComponents
{
    public class LoaiSpMenuViewComponent: ViewComponent
    {
        private readonly ILoaiSpRepository _loaisp;
        public LoaiSpMenuViewComponent(ILoaiSpRepository loaiSpRepository)
        {
            _loaisp = loaiSpRepository;
        }
        public IViewComponentResult Invoke()
        {
            var loaiSp = _loaisp.GetAll().OrderBy(X => X.Loai);
            return View(loaiSp);
        }
    }
}
