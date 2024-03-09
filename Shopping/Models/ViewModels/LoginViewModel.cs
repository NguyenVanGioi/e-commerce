using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.ViewModels
{
	public class LoginViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        public string Email { get; set; }

        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
