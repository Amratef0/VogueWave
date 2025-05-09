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

        public void AddOrderProduct(OrderProduct orderProduct)
        {
            _context.Add(orderProduct);
        }

        public void UpdateOrderProduct(OrderProduct orderProduct)
        {
            _context.Update(orderProduct);
        }

        public void DeleteOrderProduct(OrderProduct orderProduct)
        {
            _context.Remove(orderProduct);
        }

        public void DeleteOrderProductById(int id)
        {
            OrderProduct orderProduct = GetOrderProductByOrderAndProductId(id , id);
            _context.Remove(orderProduct);
        }

        public List<OrderProduct> GetAllOrderProducts()
        {
            return _context.OrderProducts.ToList();
        }

        public OrderProduct GetOrderProductByOrderAndProductId(int orderId, int productId)
        {
            return _context.OrderProducts
                .FirstOrDefault(op => op.OrderId == orderId && op.ProductId == productId);
        }


        public void SaveOrderProductChanges()
        {
            _context.SaveChanges();
        }
    }

}
