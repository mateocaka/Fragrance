using Fragrance.Data;
using Fragrance.DataAccess.Repository;
using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Identity configuration
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Authentication configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddFacebook(options =>
{
    options.AppId = "531484386144763";
    options.AppSecret = "9bb337d930842fd5bf2c497c37cf81b4";
})
.AddGoogle(options =>
{
    options.ClientId = "640937380930-m0up3a52qe5gh4o8psm10107ak12p2nc.apps.googleusercontent.com";
    options.ClientSecret = "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";
})
.AddTwitter(options =>
{
    options.ConsumerKey = "tVYLg4Sxiq5SQfB7RjeQIDH69";
    options.ConsumerSecret = "iAMZfqPcoSMVIwkBOfKHSI0vuLvb9VPBBIm6t76xv6a77pErF0";
    options.RetrieveUserDetails = true;
});

// Session and other services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register IHttpClientFactory with named client for Dashboard API
builder.Services.AddHttpClient("DashboardApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7046/api/DashboardApi/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Costumer}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers(); // Ensure API controllers are mapped

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    
    var unitOfWork = services.GetRequiredService<IUnitOfWork>();
 
   
}

app.Run();

