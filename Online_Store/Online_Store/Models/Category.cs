using System.ComponentModel.DataAnnotations;

namespace Online_Store.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category Name Requried")]
        [StringLength(100, ErrorMessage = "Category Name Must be less than 100 words")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
