using Online_Store.Models;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Mvc;
using Online_Store;
using Microsoft.EntityFrameworkCore;
using Online_Store.Interface;
namespace Online_Store.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly OnlineStoreContext _context;

        public OrdersRepository(OnlineStoreContext context)
        {
            _context = context;
        }
        public void AddOrder(Order order)
        {
            _context.Add(order);
        }

        // تحديث طلب
        public void UpdateOrder(Order order)
        {
            _context.Update(order);
        }

        // حذف طلب
        public void DeleteOrder(Order order)
        {
            _context.Remove(order);
        }

        // حذف طلب بواسطة الـ ID
        public void DeleteOrderById(int id)
        {
            Order order = GetOrderById(id);
            _context.Remove(order);
        }

        // الحصول على جميع الطلبات
        public List<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        // الحصول على طلب بواسطة الـ ID
        public Order GetOrderById(int id)
        {
            return _context.Orders.FirstOrDefault(o => o.Id == id);
        }

        // حفظ التغييرات في الطلبات
        public void SaveOrderChanges()
        {
            _context.SaveChanges();
        }
        public Order GetOrderByTrackingNumber(string trackingNumber)
        {
            return _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefault(o => o.OrderNumber == trackingNumber); // البحث باستخدام رقم التتبع
        }

       
    }
}
