using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Store.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Please upload a product image.")]
        public string? ImageUrl { get; set; } // دي الصورة الرئيسية

        public ICollection<ProductImage>? ProductImages { get; set; } // دي باقي الصور

        public ICollection<OrderProduct>? OrderProducts { get; set; }

        public DateTime? DateAdded { get; set; } = DateTime.Now;

        public int SalesCount { get; set; } = 0;

        [Required(ErrorMessage = "Please select a category.")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
