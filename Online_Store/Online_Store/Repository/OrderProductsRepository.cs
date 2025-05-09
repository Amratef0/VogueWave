using Online_Store.Interface;
using Online_Store.Models;

namespace Online_Store.Repository
{
    public class OrderProductsRepository : IOrderProductsRepository
    {
        private readonly OnlineStoreContext _context;

        public OrderProductsRepository(OnlineStoreContext context)
        {
            _context = context;
        }

        // إضافة OrderProduct جديد
        public void AddOrderProduct(OrderProduct orderProduct)
        {
            _context.Add(orderProduct);
        }

        // تحديث OrderProduct
        public void UpdateOrderProduct(OrderProduct orderProduct)
        {
            _context.Update(orderProduct);
        }

        // حذف OrderProduct
        public void DeleteOrderProduct(OrderProduct orderProduct)
        {
            _context.Remove(orderProduct);
        }

        // حذف OrderProduct بواسطة الـ ID
        public void DeleteOrderProductById(int id)
        {
            OrderProduct orderProduct = GetOrderProductByOrderAndProductId(id , id);
            _context.Remove(orderProduct);
        }

        // الحصول على جميع OrderProducts
        public List<OrderProduct> GetAllOrderProducts()
        {
            return _context.OrderProducts.ToList();
        }

        // الحصول على OrderProduct بواسطة الـ ID
        public OrderProduct GetOrderProductByOrderAndProductId(int orderId, int productId)
        {
            return _context.OrderProducts
                .FirstOrDefault(op => op.OrderId == orderId && op.ProductId == productId);
        }


        // حفظ التغييرات في OrderProducts
        public void SaveOrderProductChanges()
        {
            _context.SaveChanges();
        }
    }

}
