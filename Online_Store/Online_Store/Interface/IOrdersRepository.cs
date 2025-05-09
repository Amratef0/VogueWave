using Online_Store.Models;
using System.Collections.Generic;

namespace Online_Store.Interface
{
    public interface IOrdersRepository
    {
        void AddOrder(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(Order order);
        void DeleteOrderById(int id);
        List<Order> GetAllOrders();
        Order GetOrderById(int id);
        void SaveOrderChanges();
        public Order GetOrderByTrackingNumber(string trackingNumber);

    }
}

