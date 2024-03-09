using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumb { get; set; }
        public bool Published { get; set; }

        public string Alias { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
