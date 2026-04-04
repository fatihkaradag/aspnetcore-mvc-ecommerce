using aspnetcore_mvc_ecommerce.DataAccess.Data;
using aspnetcore_mvc_ecommerce.DataAccess.Repository;
using aspnetcore_mvc_ecommerce.DataAccess.Repository.IRepository;
using aspnetcore_mvc_ecommerce.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// ===== SERVICE REGISTRATION =====

// Adds MVC controllers with views support
builder.Services.AddControllersWithViews();

// Registers the application database context with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Binds Stripe configuration section to StripeSettings model
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Registers ASP.NET Core Identity with custom roles and EF Core stores
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configures application cookie paths for login, logout and access denied
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Adds Razor Pages support — required for Identity UI
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registers application-specific services via dependency injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// ===== PIPELINE CONFIGURATION =====

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // Uses custom error handler in production
    app.UseExceptionHandler("/Home/Error");

    // Enforces HTTPS strict transport security — 30 days default
    app.UseHsts();
}

app.UseHttpsRedirection();

// Configures Stripe API key from appsettings — never hardcode secrets
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();

// Authentication must come before Authorization in the pipeline
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.MapStaticAssets();

// Maps area-based controller routes with Customer area as default
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();