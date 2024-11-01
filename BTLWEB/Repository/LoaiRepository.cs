using BTLWEB.Models;

namespace BTLWEB.Repository
{
    public class LoaiRepository : ILoaiSpRepository
    {
        private readonly QlbanVaLiContext db;
        public LoaiRepository(QlbanVaLiContext db)
        {
            this.db = db;
        }
        public TLoaiSp Add(TLoaiSp loaiSp)
        {
            db.TLoaiSps.Add(loaiSp);
            db.SaveChanges();
            return loaiSp;
        }

        public TLoaiSp Delete(string maloaisp)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TLoaiSp> GetAll()
        {
            return db.TLoaiSps;
        }

        public TLoaiSp GetLoaiSp(string maloaisp)
        {
            return db.TLoaiSps.Find(maloaisp);
        }

        public TLoaiSp Update(TLoaiSp loaiSp)
        {
            db.Update(loaiSp);
            db.SaveChanges();
            return loaiSp;
        }
    }
}
