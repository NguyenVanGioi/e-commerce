using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderCode { get; set; }
        public bool? Deleted { get; set; }
        public string UserId { get; set; }
        public int TransactStatusId { get; set; }
        public int? AddressId { get; set; }

        public TransactStatusModel TransactStatus { get; set; }
        public AppUserModel User { get; set; }
        public AddressModel Address { get; set; }


    }
}
