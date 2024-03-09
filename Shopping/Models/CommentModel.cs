using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class CommentModel
    {
        [Key]

        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập nhận xét của bạn.")]
        [Display(Name = "Nhận xét của bạn")]
        public string Message { get; set; }
        public DateTime? DateCreated { get; set; }
        public int? rate { get; set; }

        public ProductModel Product { get; set; }
        public AppUserModel User { get; set; }

    }
}
