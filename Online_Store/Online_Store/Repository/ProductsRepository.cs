using Online_Store.Models;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Online_Store;
using Microsoft.EntityFrameworkCore;
using Online_Store.Interface;
namespace Online_Store.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly OnlineStoreContext _context;

        public ProductsRepository(OnlineStoreContext context)
        {
            _context = context;
        }


        
        public void AddProduct(Product product)
        {
            _context.Add(product);    
        }
        public void UpdateProduct(Product product)
        {
            _context.Update(product);
        }
        public void DeleteProduct(Product product)
        {
            _context.Remove(product);
        }
        public void DeleteProductById(int id)
        {
            Product product = GetProductById(id);
            _context.Remove(product);
        }
        public List<Product> GetAllProducts()
        {
             return _context.Products.ToList();
        }
        public Product GetProductById(int id)
        {
            return _context.Products.FirstOrDefault(p=>p.Id==id);
        }
        public void SaveProductChanges()
        {
            _context.SaveChanges();
        }
        public async Task SaveProductChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public IQueryable<Product> GetProductsQueryable()
        {
            return _context.Products.AsQueryable();
        }
        public Product GetProductByIdWithImages(int id)
        {
            return _context.Products
                           .Include(p => p.ProductImages)
                           .FirstOrDefault(p => p.Id == id);
        }
        public List<Product> GetNewProductsArrivals(int count = 10)
        {
            return _context.Products
                           .OrderByDescending(p => p.DateAdded)
                           .Take(count)
                           .ToList();
        }
        public List<Product> GetPopularProducts(int count = 10)
        {
            return _context.Products
                           .Where(p => p.SalesCount > 0)  // تصفية المنتجات التي لديها SalesCount أكبر من 0
                           .OrderByDescending(p => p.SalesCount)  // ترتيب المنتجات من الأكثر مبيعًا إلى الأقل
                           .Take(count)  // أخذ العدد المطلوب من المنتجات
                           .ToList();
        }


        public List<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products
                           .Where(p => p.CategoryId == categoryId)
                           .ToList();
        }

    }

}
