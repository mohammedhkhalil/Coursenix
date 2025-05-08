// Program.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Coursenix.Models;

// Add Identity related usings
using Microsoft.AspNetCore.Identity;

// Add Authentication using
using Microsoft.AspNetCore.Authentication.Cookies; // Often needed explicitly or implicitly by Identity's cookie setup

var builder = WebApplication.CreateBuilder(args);

// --- بداية إعداد الخدمات (Services) ---

// إضافة خدمة المتحكمات مع Views
builder.Services.AddControllersWithViews();

// تسجيل CoursenixContext
builder.Services.AddDbContext<CoursenixContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- بداية إعداد وتكوين ASP.NET Core Identity (ضروري لتقديم خدمة UserManager) ---
// هذا يضيف خدمات Identity الأساسية، بما في ذلك UserManager<Teacher> و SignInManager<Teacher> و RoleManager<IdentityRole>
// .AddEntityFrameworkStores<CoursenixContext>() يخبر Identity باستخدام EF و CoursenixContext لتخزين بياناته
// .AddDefaultTokenProviders() يضيف مولدات التوكن اللازمة (مطلوبة لإعادة تعيين كلمة المرور)
// IMPORTANT: Replace 'Teacher' and 'IdentityRole' with your actual Identity user and role types if they are different.
// Your Teacher model must be compatible with the Identity user type configured here (e.g., inherit from IdentityUser).
//builder.Services.AddIdentity<Teacher, IdentityRole>(options =>
//{
//    // اختياري: تكوين خيارات Identity (مثل متطلبات كلمة المرور)
//    // options.SignIn.RequireConfirmedAccount = false;
//    // options.Password.RequiredLength = 6;
//    // ...
//})
//    .AddEntityFrameworkStores<CoursenixContext>() // Identity سيخزن بياناته في نفس الـ DbContext
//    .AddDefaultTokenProviders(); // إضافة مولدات التوكن (مطلوبة لإعادة تعيين كلمة المرور)
// --- نهاية إعداد وتكوين ASP.NET Core Identity ---

// عند تهيئة المصادقة باستخدام AddIdentity، يتم غالباً تسجيل Cookie Authentication Scheme تلقائياً.
// يمكنك تكوينه بشكل صريح هنا إذا لزم الأمر لتحديد مسار صفحة تسجيل الدخول:
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // المسار لصفحة تسجيل الدخول
    options.AccessDeniedPath = "/Account/AccessDenied"; // المسار لصفحة ممنوع الوصول
});


// يمكنك إضافة خدمات أخرى هنا إذا لزم الأمر (مثل CORS، خدمات البريد الإلكتروني لإعادة تعيين كلمة المرور)

// --- نهاية إعداد الخدمات ---


// بناء التطبيق من خلال الـ builder الذي تم إعداده
var app = builder.Build();

// --- بداية تهيئة خط معالجة الطلبات (Request Pipeline) ---

// تهيئة خط الأنابيب لبيئة التطوير
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Identity غالباً يضيف Endpoints لـ Identity UI إذا استخدمتها، وقد تتطلب هذا Middleware في بيئة التطوير
    //app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// --- Middleware المصادقة (Authentication) والتفويض (Authorization) ---
// Middleware المصادقة يجب أن يأتي بعد UseRouting وقبل UseAuthorization
// هذا الـ Middleware يقرأ هوية المستخدم من (مثل الكوكيز)
app.UseAuthentication();

// Middleware التفويض يجب أن يأتي بعد UseAuthentication
// هذا الـ Middleware يطبق قواعد التفويض (مثل سمات [Authorize])
// إذا أردت تجاوز متطلبات التفويض مؤقتاً، يمكنك **تعليق هذا السطر**.
app.UseAuthorization();
// --- نهاية Middleware المصادقة والتفويض ---


// تعيين نقاط نهاية المتحكمات
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Home}/{id?}");

// إذا كنت تستخدم Razor Pages المتعلقة بـ Identity التلقائي (Identity UI)، ستحتاج لهذا
// app.MapRazorPages();


// إضافة نقاط نهاية أخرى هنا إذا لزم الأمر (مثال: لـ APIs)

// --- نهاية تهيئة خط معالجة الطلبات ---

// تشغيل التطبيق وبدء الاستماع للطلبات
app.Run();