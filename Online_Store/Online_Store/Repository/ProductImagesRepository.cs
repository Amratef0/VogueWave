using Microsoft.EntityFrameworkCore;
using Online_Store.Interface;
using Online_Store.Models;

namespace Online_Store.Repository
{
    public class ProductImagesRepository : IProductImagesRepository
    {
        private readonly OnlineStoreContext _context;

        public ProductImagesRepository(OnlineStoreContext context)
        {
            _context = context;
        }

        public void AddProductImage(ProductImage productImage)
        {
            _context.Add(productImage);
        }

        public void UpdateProductImage(ProductImage productImage)
        {
            _context.Update(productImage);
        }

        public void DeleteProductImage(ProductImage productImage)
        {
            _context.Remove(productImage);
        }

        public void DeleteProductImageById(int id)
        {
            ProductImage productImage = GetProductImageById(id);
            _context.Remove(productImage);
        }

        public List<ProductImage> GetAllProductImages()
        {
            return _context.ProductImages.ToList();
        }

        public ProductImage GetProductImageById(int id)
        {
            return _context.ProductImages.FirstOrDefault(pi => pi.Id == id);
        }

        public void SaveProductImageChanges()
        {
            _context.SaveChanges();
        }
        public void DeleteImagesByProductId(int productId)
        {
            var productFromDb = _context.Products
                .Include(p => p.ProductImages) 
                .FirstOrDefault(p => p.Id == productId);

            if (productFromDb != null && productFromDb.ProductImages != null && productFromDb.ProductImages.Any())
            {
                _context.ProductImages.RemoveRange(productFromDb.ProductImages);  
            }
        }
    }

}
