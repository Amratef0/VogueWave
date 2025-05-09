using Online_Store.Models;
using System.Collections.Generic;
namespace Online_Store.Interface
{
    public interface IOrderProductsRepository
    {
        void AddOrderProduct(OrderProduct orderProduct);
        void UpdateOrderProduct(OrderProduct orderProduct);
        void DeleteOrderProduct(OrderProduct orderProduct);
        void DeleteOrderProductById(int id);
        List<OrderProduct> GetAllOrderProducts();
        OrderProduct GetOrderProductByOrderAndProductId(int orderId, int productId);
        void SaveOrderProductChanges();
    }
}
