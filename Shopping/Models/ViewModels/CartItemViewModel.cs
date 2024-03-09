using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.ViewModels
{
	public class CartItemViewModel
	{
		public List<CartItemModel > CartItems { get; set; }
		public int? GrandTotal { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]

        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]

        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]

        public string AddressOrder { get; set; }
    }
}
