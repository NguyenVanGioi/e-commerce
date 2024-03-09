using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;

namespace Shopping.Models
{
    public class PostModel
    {
        [Key]

        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public string Thumb { get; set; }
        public bool Published { get; set; }
        public string Alias { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
