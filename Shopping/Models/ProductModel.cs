using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int? Price { get; set; }
        public string Thumb { get; set; }
        public bool Active { get; set; }

        public string Alias { get; set; }
        public int? UnitsInstock {  get; set; }

        public int CategoryId { get; set; }

        public int BrandId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
        public ICollection<CommentModel> Comments { get; set; }

    }
}
