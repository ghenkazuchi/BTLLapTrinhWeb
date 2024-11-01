using System.ComponentModel.DataAnnotations;

namespace BTLWEB.ViewModels
{
    public class RegisterVM
    {
        [Display(Name ="tên đăng nhập")]   
        [Required(ErrorMessage ="tên đăng nhập không được để trống")]
        [StringLength(20,ErrorMessage ="tên đăng nhập phải có ít nhất 8 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Tên đăng nhập phải có chữ thường chữ hoa chữ số")]
        public string UserName { get; set; }
        [Display(Name ="Mật khẩu")]
        [Required(ErrorMessage = "mật khẩu không được để trống")]
        [StringLength(20,MinimumLength=8 ,ErrorMessage = "mật khẩu có ít nhất 8 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải có chữ thường chữ hoa chữ số")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
