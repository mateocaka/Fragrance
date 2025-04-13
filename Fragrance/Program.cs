using Fragrance.Data;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Fragrance.Utility;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using CookieAuthenticationDefaults = Microsoft.Owin.Security.Cookies.CookieAuthenticationDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));



builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationType;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})

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
        options.CallbackPath = "/signin-twitter";
    });
  
builder.Services.AddDistributedMemoryCache();
//ading seesion ne fillim
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthorization();
app.UseSession();



app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Costumer}/{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();
app.Run();
