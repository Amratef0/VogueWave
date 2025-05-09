using Online_Store.Models;
using System.Collections.Generic;

namespace Online_Store.Interface
{
    public interface IProductsRepository
    {
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
        void DeleteProductById(int id);
        List<Product> GetAllProducts();
        Product GetProductById(int id);
        void SaveProductChanges();
        Task SaveProductChangesAsync();
        IQueryable<Product> GetProductsQueryable();
        Product GetProductByIdWithImages(int id);
        List<Product> GetNewProductsArrivals(int count = 10);
        List<Product> GetPopularProducts(int count = 10);
        List<Product> GetProductsByCategory(int categoryId);
    }
}
