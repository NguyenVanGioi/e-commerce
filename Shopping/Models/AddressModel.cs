using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class AddressModel
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Full Name is required")]

        public string FullName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required")]

        public string Phone { get; set; }
        [Required(ErrorMessage = "Address is required")]

        public string AddressOrder { get; set; }
        public DateTime? DateCreated { get; set; }
        public AppUserModel User { get; set; }
    }
}
