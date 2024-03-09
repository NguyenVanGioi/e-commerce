using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
	public class UserModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập tài khoản")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập Email"), EmailAddress]

		public string Email { get; set; }
		[DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		public string Password { get; set; }
        [DataType(DataType.Password), Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
