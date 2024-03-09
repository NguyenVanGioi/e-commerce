using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class TransactStatusModel
    {
        [Key]
        public int Id { get; set; }
        public int? Status { get; set; }
        public string Description { get; set; }
    }
}
