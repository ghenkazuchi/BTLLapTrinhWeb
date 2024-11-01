using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTLWEB.Models;

public partial class TUser
{
    [Display(Name = "tên đăng nhập")]
    [Required(ErrorMessage = "tên đăng nhập không được để trống")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "tên đăng nhập phải có ít nhất 8 ký tự")]
    [RegularExpression(@"[A-Za-z0-9.-]", ErrorMessage = "tên đăng nhập phải có chữ thường chữ hoa chữ số")]
    public string Username { get; set; } = null!;
    [Display(Name = "Mật khẩu")]
    [Required(ErrorMessage = "mật khẩu không được để trống")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "mật khẩu có ít nhất 8 ký tự")]
    [RegularExpression(@"[A-Za-z0-9.-]", ErrorMessage = "mật khẩu phải có chữ thường chữ hoa chữ số")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    public byte? LoaiUser { get; set; }

    public virtual ICollection<TKhachHang> TKhachHangs { get; set; } = new List<TKhachHang>();

    public virtual ICollection<TNhanVien> TNhanViens { get; set; } = new List<TNhanVien>();
}
