namespace Shopping.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderModel Order { get; set; }
        public List<OrderDetailModel> OrderDetails { get; set; }
    }
}
