using Shopping.Models;

namespace Shopping.Models
{
	public class CartItemModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Quantity { get; set; }
		public int? Price { get; set; }
		public int? Total
		{
			get { return Quantity * Price; }
		}
		public string Thumb;
        public string UserId { get; set; }
        public CartItemModel() { }
		public CartItemModel(ProductModel product)
		{
			Id = product.Id;
			Name = product.Name;
			Price  = product.Price;
			Quantity = 1;
			Thumb = product.Thumb;
		}
	}
}
