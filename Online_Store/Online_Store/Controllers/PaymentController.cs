using Microsoft.AspNetCore.Mvc;
using Online_Store.Models;  
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Online_Store.Repository;
using Online_Store.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Online_Store.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IOrderProductsRepository _orderProductRepository;
        private readonly IProductImagesRepository _productImagesRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public PaymentController(IProductsRepository productsRepository, IOrdersRepository ordersRepository, IOrderProductsRepository orderProductRepository, ICategoriesRepository categoriesRepository, IProductImagesRepository productImagesRepository , UserManager<ApplicationUser> userManager)
        {
            _productsRepository = productsRepository;
            _ordersRepository = ordersRepository;
            _orderProductRepository = orderProductRepository;
            _categoriesRepository = categoriesRepository;
            _productImagesRepository = productImagesRepository;
            _userManager = userManager;
        }

        public ActionResult Confirm()
        {
            try
            {
                // استرجاع السلة من السيشن
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // تمرير السلة إلى الـ View عبر ViewBag
                if (cart.Count == 0)
                {
                    ViewBag.Error = "Your cart is empty.";
                }
                ViewBag.Cart = cart;

                // إرجاع الـ View مع الـ Model
                return View(new Order()); // إرجاع نموذج Order فارغ
            }
            catch (Exception ex)
            {
                // في حالة حدوث أي استثناء
                ViewBag.Error = "An error occurred while retrieving the cart: " + ex.Message;
                // إعادة عرض الصفحة مع عرض رسالة الخطأ
                return View("Confirm");
            }
        }

        public ActionResult ThankYou(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                ViewBag.Error = "Order number is missing.";
                return View("Error");
            }

            var order = _ordersRepository.GetOrderByTrackingNumber(orderNumber);

            if (order == null)
            {
                ViewBag.Error = "Order not found.";
                return View("Error");
            }

            return View(order); // ابعت الموديل نفسه
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Process(Order order)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Confirm", order);
                }

                order.OrderDate = DateTime.Now;

                // ✅ رقم تتبع فريد للحجز
                order.OrderNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // ✅ حالة مبدئية
                order.Status = OrderStatus.Pending;

                // ✅ ربط الطلب بالمستخدم الحالي
                                                 // أو استخدام _userManager للحصول على الـ UserId للمستخدم:
                                                 // var userId = _userManager.GetUserId(User);


                // محاولة إضافة الطلب إلى قاعدة البيانات
                _ordersRepository.AddOrder(order);
                _ordersRepository.SaveOrderChanges();

                var savedOrder = _ordersRepository.GetOrderById(order.Id);

                if (savedOrder == null)
                {
                    ViewBag.Error = "An error occurred while saving the order.";
                    return View("Confirm", order);
                }

                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // محاولة معالجة العناصر في السلة
                foreach (var item in cart)
                {
                    var product = _productsRepository.GetProductById(item.ProductId);
                    if (product != null)
                    {
                        var orderProduct = new OrderProduct
                        {
                            OrderId = savedOrder.Id,
                            ProductId = product.Id,
                            Quantity = item.Quantity
                        };
                        _orderProductRepository.AddOrderProduct(orderProduct);

                        // تحديث عدد المبيعات
                        product.SalesCount += item.Quantity;
                    }
                }

                _productsRepository.SaveProductChanges();
                HttpContext.Session.Remove("Cart");

                // التعديل هنا: تمرير رقم الطلب إلى صفحة "ThankYou"
                return RedirectToAction("ThankYou", new { orderNumber = savedOrder.OrderNumber });
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred: " + ex.Message;
                ViewBag.StackTrace = ex.StackTrace;
                return View("Confirm", order);
            }
        }

        [HttpGet]
        public ActionResult TrackOrder()
        {
            return View();  // عرض صفحة إدخال رقم التتبع
        }


        public ActionResult OrderDetails(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                ViewBag.Error = "Order number is required.";
                return View("Error");
            }

            var order = _ordersRepository.GetOrderByTrackingNumber(orderNumber);

            if (order == null)
            {
                ViewBag.Error = "Order not found.";
                return View("Error");
            }

            return View(order);
        }

    }
}
