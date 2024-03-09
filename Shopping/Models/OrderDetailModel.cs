using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class OrderDetailModel
    {
        [Key]
        public int Id { get; set; }
        public int? Quantity { get; set; }
        public string Thumb { get; set; }
        public int? Total { get; set; }
        public DateTime? ShipDate { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public OrderModel Order { get; set; }
        public ProductModel Product { get; set; }

    }
}
