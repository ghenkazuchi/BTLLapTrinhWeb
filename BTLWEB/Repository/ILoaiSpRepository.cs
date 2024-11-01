using BTLWEB.Models;
namespace BTLWEB.Repository
{
    public interface ILoaiSpRepository
    {
        TLoaiSp Add(TLoaiSp loaiSp);
        TLoaiSp Update(TLoaiSp loaiSp);
        TLoaiSp Delete(string maloaisp);
        TLoaiSp GetLoaiSp(string maloaisp);
        IEnumerable<TLoaiSp> GetAll();
    }
}
