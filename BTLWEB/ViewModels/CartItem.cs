namespace BTLWEB.ViewModels
{
    public class CartItem
    {
        public string maSp {  get; set; }
        public string anhDaiDien {  get; set; }
        public string tensp {  get; set; }
        public decimal Dongia {  get; set; }
        public int SoLuong {  get; set; }
        public decimal ThanhTien => Dongia * SoLuong;
    }
}
