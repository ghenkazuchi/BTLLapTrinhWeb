using System;
using System.Collections.Generic;

namespace BTLWEB.Models;

public partial class TKhachHang
{
    public string MaKhanhHang { get; set; } = null!;

    public string? Username { get; set; }

    public string? TenKhachHang { get; set; }

    public DateOnly? NgayDat { get; set; }

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public byte? LoaiKhachHang { get; set; }

    public string? CachThanhToan { get; set; }

    public string? GhiChu { get; set; }

    public string? CachVanChuyen { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<THoaDonBan> THoaDonBans { get; set; } = new List<THoaDonBan>();

    public virtual TUser? UsernameNavigation { get; set; }
}
