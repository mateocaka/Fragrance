using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Fragrance.Data;
using Fragrance.DataAccess.DbInitializer;
using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.DataAccess.Repository;
using Fragrance.Utility;
using Azure.Identity;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddAuthentication().AddFacebook(option => {
    option.AppId = "531484386144763";
    option.AppSecret = "9bb337d930842fd5bf2c497c37cf81b4";
}).AddGoogle(option =>
{
    option.ClientId = "640937380930-m0up3a52qe5gh4o8psm10107ak12p2nc.apps.googleusercontent.com";
    option.ClientSecret = "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";
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

SeedDatabase();
app.MapRazorPages();

app.MapControllerRoute(
    name: "combinedFilters",
    pattern: "gender/{gender}/brand/{brand}/rating/{rating}",
    defaults: new { area = "Costumer", controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "genderBrand",
    pattern: "gender/{gender}/brand/{brand}",
    defaults: new { area = "Costumer", controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "genderRating",
    pattern: "gender/{gender}/rating/{rating}",
    defaults: new { area = "Costumer", controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "brandRating",
    pattern: "brand/{brand}/rating/{rating}",
    defaults: new { area = "Costumer", controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "genderOnly",
    pattern: "gender/{gender}",
    defaults: new { area = "Costumer", controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "brandOnly",
    pattern: "brand/{brand}",
    defaults: new { area = "Costumer", controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "ratingOnly",
    pattern: "rating/{rating}",
    defaults: new { area = "Costumer", controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Costumer}/{controller=Home}/{action=Index}/{id?}");

app.Use(async (context, next) =>
{
    var path = context.Request.Path;
    var query = context.Request.Query;

    if (query.Any() && !path.Value.Contains("/brand/") &&
                      !path.Value.Contains("/gender/") &&
                      !path.Value.Contains("/rating/"))
    {
        var segments = new List<string>();

        if (query.ContainsKey("gender"))
            segments.Add($"gender/{query["gender"]}");
        if (query.ContainsKey("brand"))
            segments.Add($"brand/{query["brand"]}");
        if (query.ContainsKey("rating"))
            segments.Add($"rating/{query["rating"]}");

        if (segments.Any())
        {
            var newPath = $"/{string.Join("/", segments)}";
            var remainingParams = query.Where(q =>
                !new[] { "gender", "brand", "rating" }.Contains(q.Key));

            if (remainingParams.Any())
            {
                newPath += "?" + string.Join("&",
                    remainingParams.Select(q => $"{q.Key}={q.Value}"));
            }

            context.Response.Redirect(newPath);
            return;
        }
    }

    await next();
});
app.Run();


void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}