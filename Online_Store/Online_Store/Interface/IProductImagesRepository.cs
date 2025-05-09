using Online_Store.Models;
using System.Collections.Generic;

namespace Online_Store.Interface
{
    public interface IProductImagesRepository
    {
        void AddProductImage(ProductImage productImage);
        void UpdateProductImage(ProductImage productImage);
        void DeleteProductImage(ProductImage productImage);
        void DeleteProductImageById(int id);
        List<ProductImage> GetAllProductImages();
        ProductImage GetProductImageById(int id);
        void SaveProductImageChanges();
        void DeleteImagesByProductId(int productId);
    }
}
