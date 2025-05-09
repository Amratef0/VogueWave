using Microsoft.AspNetCore.Http; // for session handling
using Microsoft.AspNetCore.Mvc;
using Online_Store.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // for List
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Online_Store.Repository;
using Online_Store.Interface;
using Microsoft.AspNetCore.Authorization;

namespace Online_Store.Controllers
{
    [Authorize]

    public class MainController : Controller
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IOrderProductsRepository _orderProductRepository;
        private readonly IProductImagesRepository _productImagesRepository;


        public MainController(IProductsRepository productsRepository , IOrdersRepository ordersRepository , ICategoriesRepository categoriesRepository , IOrderProductsRepository orderProductsRepository ,IProductImagesRepository productImagesRepository)
        {
            _productsRepository = productsRepository;
            _ordersRepository = ordersRepository;
            _categoriesRepository = categoriesRepository;
            _orderProductRepository = orderProductsRepository;
            _productImagesRepository = productImagesRepository;
        }

        [AllowAnonymous]
        public IActionResult Index(string search, int? categoryId, int page = 1)
        {
            int pageSize = 12;

            ViewBag.Categories = _categoriesRepository.GetAllCategories();

            var products = _productsRepository.GetProductsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
                ViewBag.SearchValue = search;
            }

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            products = products.OrderByDescending(p => p.DateAdded);

            var totalProducts = products.Count();
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.HasMore = (page * pageSize) < totalProducts;

            // جلب البيانات الجانبية زي الأول
            ViewBag.NewArrivals = products.Take(4).ToList();
            ViewBag.PopularProducts = _productsRepository.GetPopularProducts(4);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductList", pagedProducts);
            }

            return View(pagedProducts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.CategoryList = new SelectList(_categoriesRepository.GetAllCategories(), "Id", "Name");
            return View("Add", new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SaveAdd(Product newProduct, string galleryImagesNames)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = new SelectList(_categoriesRepository.GetAllCategories(), "Id", "Name");
                return View("Add", newProduct);
            }

_productsRepository.AddProduct(newProduct);
            await _productsRepository.SaveProductChangesAsync();
            if (!string.IsNullOrEmpty(galleryImagesNames))
            {
                var images = galleryImagesNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                               .Select(i => i.Trim())
                                               .ToList();

                foreach (var img in images)
                {
                    var productImage = new ProductImage
                    {
                        ProductId = newProduct.Id,
                        ImageUrl = img
                    };
                    _productImagesRepository.AddProductImage(productImage);
                }
                await _productsRepository.SaveProductChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]

        public IActionResult Edit(int id)
        {
            var product = _productsRepository.GetProductByIdWithImages(id);
                           

            if (product == null) return NotFound();

            ViewBag.CategoryList = new SelectList(_categoriesRepository.GetAllCategories(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> SaveEdit(Product profromrequest, string? galleryImageNames)
        {
            if (profromrequest == null) return BadRequest("Product data is missing.");

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = new SelectList(_categoriesRepository.GetAllCategories(), "Id", "Name", profromrequest.CategoryId);
                return View("Edit", profromrequest);
            }

            var productFromDb = _productsRepository.GetProductByIdWithImages(profromrequest.Id);

            if (productFromDb == null) return NotFound();

            productFromDb.Name = profromrequest.Name;
            productFromDb.Price = profromrequest.Price;
            productFromDb.Description = profromrequest.Description;
            productFromDb.CategoryId = profromrequest.CategoryId;
            productFromDb.ImageUrl = profromrequest.ImageUrl?.Trim();

            _productImagesRepository.DeleteImagesByProductId(profromrequest.Id);

            if (!string.IsNullOrEmpty(galleryImageNames))
            {
                var newImages = galleryImageNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim())
                                .Where(x => !string.IsNullOrEmpty(x))
                                .ToList();

                foreach (var imgName in newImages)
                {
                    _productImagesRepository.AddProductImage(new ProductImage
                    {
                        ProductId = productFromDb.Id,
                        ImageUrl = imgName
                    });
                }
            }

            await _productsRepository.SaveProductChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Delete(int id)
        {
            var product = _productsRepository.GetProductById(id);  // استدعاء الريبو للحصول على المنتج
            if (product != null)
            {
                _productsRepository.DeleteProductById(id);
                _productsRepository.SaveProductChanges();// استخدام الميثود في الريبو لحذف المنتج
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            try
            {
                var product = _productsRepository.GetProductByIdWithImages(id);

                if (product == null)
                {
                    return NotFound();
                }

                return View("Details", product);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
                return View("Details", new Product()); // بترجع View فاضية مع رسالة الخطأ
            }
        }

        public IActionResult ManageProducts()
        {
            var products = _productsRepository.GetAllProducts();
            return View(products);
        }

        [HttpGet]

        public IActionResult ViewCart()
        {
            // Retrieve the cart from the session, or use an empty list if it doesn't exist
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Check if cart is empty and handle accordingly
            if (cart == null || cart.Count == 0)
            {
                TempData["Message"] = "Your cart is empty!";
            }

            return View(cart); // Return the cart to the View
        }
        [HttpPost]

        public IActionResult AddToCart(int id)
        {
            try
            {
                var product =_productsRepository.GetProductById(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                // جلب السلة من السيشن أو إنشاء سلة جديدة إذا كانت فارغة
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // البحث عن المنتج في السلة
                var existingItem = cart.FirstOrDefault(c => c.ProductId == product.Id);

                // إذا كان المنتج موجود بالفعل في السلة، زيادة الكمية
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    // إذا مش موجود، أضف المنتج إلى السلة مع الكمية 1
                    cart.Add(new CartItem
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        ImageUrl = product.ImageUrl,
                        Quantity = 1 // هنا الكمية بتبدأ من 1
                    });
                }

                // حفظ السلة في السيشن بعد التعديل
                HttpContext.Session.SetObjectAsJson("Cart", cart);

                TempData["Message"] = "Product added to cart successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding to cart: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]

        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);

                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove);
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                    TempData["Message"] = "Product removed from cart successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Product not found in cart!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while removing from cart: " + ex.Message;
            }

            return RedirectToAction("ViewCart");
        }

        [HttpPost]

        public IActionResult UpdateQuantity(int productId, string actionType)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (actionType == "increase")
                    item.Quantity++;
                else if (actionType == "decrease" && item.Quantity > 1)
                    item.Quantity--;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("ViewCart");
        }

        public IActionResult About()
        {
            return View("About");
        }
        // أكشن لعرض أحدث 4 منتجات فقط
        public IActionResult NewArrivalsHome()
        {
            // بنجيب أحدث 10 منتجات تم إضافتها
            var newProducts = _productsRepository.GetNewProductsArrivals(10);

            // نعرض فقط 4 منتجات في الـ view
            var productsToShow = newProducts.Take(4).ToList();

            return View(productsToShow); // يعرض 4 منتجات في الـ View
        }

        // أكشن لعرض جميع المنتجات الجديدة
        public IActionResult NewArrivals()
        {
            // بنجيب أحدث 10 منتجات تم إضافتها
            var newProducts = _productsRepository.GetNewProductsArrivals(10);

            return View(newProducts); // يعرض جميع المنتجات في الـ View
        }

        // أكشن لعرض 4 منتجات مشهورة في الصفحة الرئيسية
        public IActionResult PopularProductsHome()
        {
            // جلب أول 4 منتجات مشهورة
            var popularProducts = _productsRepository.GetPopularProducts(4); // تمرير العدد هنا 4

            return View(popularProducts); // عرض الـ 4 منتجات في الـ View
        }


        // أكشن لعرض جميع المنتجات المشهورة
        public IActionResult PopularProducts()
        {
            // جلب المنتجات المشهورة التي تحتوي على SalesCount أكبر من 0
            var popularProducts = _productsRepository.GetPopularProducts(10); // تمرير العدد هنا 10

            return View(popularProducts); // عرض المنتجات في الـ View
        }

        // عرض كل الـ Categories
        public IActionResult CategoryPage(int categoryId)
        {
            // جلب الفئة المطلوبة بناءً على الـ categoryId
            var category = _categoriesRepository.GetCategoryById(categoryId);

            if (category == null)
            {
                // إذا لم يتم العثور على الفئة
                return NotFound();
            }

            // جلب المنتجات الخاصة بتلك الفئة
            var products = _productsRepository.GetProductsByCategory(categoryId);

            // إرسال الفئة والمنتجات إلى الـ View
            ViewBag.CategoryName = category.Name;
            return View("CategoryPage", products);
        }

        public IActionResult CategoriesIndex()
        {
            if (_productsRepository == null)
            {
                throw new Exception("DB Context is null!");
            }

            var categories = _categoriesRepository.GetAllCategories();
            return View("CategoriesIndex", categories ?? new List<Category>());
        }

        // عرض صفحة الإضافة
        [HttpGet]
        public IActionResult AddCategory()
        {
            return View("AddCategory", new Category());
        }

        // حفظ القسم الجديد
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult SaveAddCategory(Category category)
        {
            try
            {
                if (!ModelState.IsValid || category == null)
                {
                    return View("AddCategory", category ?? new Category());
                }

                _categoriesRepository.AddCategory(category);
                _categoriesRepository.SaveCategoryChanges();

                return RedirectToAction("CategoriesIndex");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
                return View("AddCategory", category ?? new Category());
            }
        }

        // عرض صفحة التعديل
        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var category = _categoriesRepository.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View("EditCategory", category);
        }

        // حفظ التعديلات
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult SaveEditCategory(Category category)
        {
            if (category == null)
            {
                return View("EditCategory", new Category());
            }

            if (!ModelState.IsValid)
            {
                return View("EditCategory", category);
            }

            var existingCategory = _categoriesRepository.GetCategoryById(category.Id);
            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = category.Name;
            _categoriesRepository.SaveCategoryChanges();

            return RedirectToAction("CategoriesIndex");
        }

        // حذف القسم
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult DeleteCategory(int id)
        {
            var category = _categoriesRepository.GetCategoryById(id);
            if (category != null)
            {
                _categoriesRepository.DeleteCategory(category);
                _categoriesRepository.SaveCategoryChanges();
            }

            return RedirectToAction("CategoriesIndex");
        }

        [AllowAnonymous]

        public IActionResult Contact()
        {
            return View("Contact");
        }
        [AllowAnonymous]
        public IActionResult ServicePage()
        {
            return View("ServicePage");
        }

    }

}

    



