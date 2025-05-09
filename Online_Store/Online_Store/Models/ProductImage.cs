using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Store.Models
{
    public class ProductImage
    {
        public int? Id { get; set; }
        public string? ImageUrl { get; set; }

        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}
