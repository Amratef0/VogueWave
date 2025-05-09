using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Online_Store.Interface;
using Online_Store.Models;
using Online_Store.Repository;

var builder = WebApplication.CreateBuilder(args);

// إعدادات الـ DbContext للاتصال بقاعدة البيانات
builder.Services.AddDbContext<OnlineStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// إعدادات الهوية مع إضافة مزودي الرموز الافتراضيين
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;  // فرض تأكيد الحساب
})
    .AddEntityFrameworkStores<OnlineStoreContext>()
    .AddDefaultTokenProviders();  // إضافة مزودي الرموز الافتراضيين

// إعداد مدة صلاحية التوكن لإعادة تعيين كلمة المرور (ساعتين)
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(2);
});

// إعداد Anti-Forgery Token بشكل آمن
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "AntiForgeryCookie";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.HttpOnly = true;
});

// إضافة خدمات الـ Controllers و Views
builder.Services.AddControllersWithViews();

// إعداد سياسة الكوكيز بشكل عام
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

// إضافة خدمة السيشن
builder.Services.AddSession(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.HttpOnly = true;
});

// إضافة إعدادات الـ SMTP
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();

// إضافة الخدمات الأخرى
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<IOrderProductsRepository, OrderProductsRepository>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<IProductImagesRepository, ProductImagesRepository>();

// بناء التطبيق
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Main/Error");
    app.UseHsts();  // تفعيل HSTS لتعزيز الأمان في بيئة الإنتاج
}

app.UseHttpsRedirection();  // التأكد من أن كل الزيارات تتم عبر HTTPS
app.UseStaticFiles();  // إذا كان لديك ملفات ثابتة مثل CSS أو JavaScript

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();  // تفعيل استخدام السيشن
app.UseCookiePolicy();  // تفعيل سياسة الكوكيز

// إعداد التوجيه (Routing) للمسارات
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();  // تشغيل التطبيق
