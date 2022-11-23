using BookMarket.DataLayer.Repository.CartRepository;
using BookMarket.DataLayer.Repository.UserRepository;
using BookMarket.DataLayer.Repository.AdminRepository;
using BookMarket.IdentityServer.Context;
using BookMarket.Infrastracture;
using Microsoft.AspNetCore.Identity;
using BookMarket.ExcelHandler.Interfaces;
using BookMarket.ExcelHandler.Readers;
using BookMarket.DataLayer.Repository.OrderRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddSession();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<ICartRepository, CartRepository>();
builder.Services.AddTransient<AdminRepository>();
builder.Services.AddTransient<UploadRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();

builder.Services.AddTransient<IExcelFileReader, ExcelHandler>();

builder.Services.AddDbContext<BookMarketIdentityContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<BookMarketIdentityContext>();

builder.Services.AddAuthentication("Cookies").AddCookie(opt => opt.LoginPath = "/Home/Login");
builder.Services.AddAuthorization();

builder.Services.AddTransient<CartService>();
builder.Services.AddTransient<AdminService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.SessionGuidMiddlware();

app.UseStaticFiles();

app.MapDefaultControllerRoute();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    var admin_service = app.Services.GetRequiredService<AdminService>();
    await admin_service.InitAdminRoleAndAccount();
});

app.Run();