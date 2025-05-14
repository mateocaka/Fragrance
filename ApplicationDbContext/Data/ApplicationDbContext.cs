using Microsoft.EntityFrameworkCore;
using Fragrance.Models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Fragrance.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Parfume> Parfumes { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShopingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Parfume>().HasData(
     new Parfume
     {
         ParfumeId = 1,
         Name = "Le Male Elixir",
         Author= "Jean Paul Gaultier",
         Gender = "Male",
         description = "A bold and intense fragrance.",
         TopNotes = "Cardamom,Lavender",
         MiddleNotes = "Vanilla,Benzoin",
         BaseNotes = "Tonka Bean,Amber",
         ListPrice = 120,
         Price = 100,
         Price50 = 90,
         Price100 = 80,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.1
     },
     new Parfume
     {
         ParfumeId = 2,
         Name = "Herod",
         Author = "Parfume de Marly",
         Gender = "Unisex",
         description = "A sweet and spicy fragrance with tobacco notes.",
         TopNotes = "Cinnamon,Pepper",
         MiddleNotes = "Tobacco,Incense",
         BaseNotes = "Vanilla,Cedarwood",
         ListPrice = 150,
         Price = 140,
         Price50 = 130,
         Price100 = 120,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.7
     },
     new Parfume
     {
         ParfumeId = 3,
         Name = "Spicebomb Extreme",
         Author = "Viktor Rolf",
         Gender = "Male",
         description = "A powerful and warm spicy scent.",
         TopNotes = "Pepper,Grapefruit",
         MiddleNotes = "Cinnamon,Saffron",
         BaseNotes = "Tobacco,Vanilla",
         ListPrice = 110,
         Price = 100,
         Price50 = 90,
         Price100 = 85,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.5
     },
     new Parfume
     {
         ParfumeId = 4,
         Name = "Jazz Club",
         Author = "Maison Margiela",
         Gender = "Male",
         description = "A warm and cozy fragrance with boozy notes.",
         TopNotes = "Pink Pepper,Neroli",
         MiddleNotes = "Rum,Clary Sage",
         BaseNotes = "Tobacco,Vanilla",
         ListPrice = 130,
         Price = 120,
         Price50 = 110,
         Price100 = 100,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.9
     },
     new Parfume
     {
         ParfumeId = 5,
         Name = "Stronger With You Intensely",
         Author = "Armani",
         Gender = "Male",
         description = "A sweet and gourmand fragrance.",
         TopNotes = "Pink Pepper,Juniper",
         MiddleNotes = "Lavender,Sage",
         BaseNotes = "Vanilla,Tonka Bean",
         ListPrice = 140,
         Price = 130,
         Price50 = 120,
         Price100 = 110,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.8
     },
     new Parfume
     {
         ParfumeId = 6,
         Name = "The Most Wanted Parfum",
         Author = "Azzaro",
         Gender = "Male",
         description = "A warm, woody fragrance with an intense character.",
         TopNotes = "Ginger,Bergamot",
         MiddleNotes = "Wood Accord",
         BaseNotes = "Vanilla,Amber",
         ListPrice = 150,
         Price = 140,
         Price50 = 130,
         Price100 = 120,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.2
     },
     new Parfume
     {
         ParfumeId = 7,
         Name = "Tobacco Vanille",
         Author = "Tom Ford",
         Gender = "Unisex",
         description = "A luxurious blend of tobacco and vanilla.",
         TopNotes = "Tobacco Leaf,Spices",
         MiddleNotes = "Vanilla,Cacao",
         BaseNotes = "Dried Fruits,Wood",
         ListPrice = 200,
         Price = 190,
         Price50 = 180,
         Price100 = 170,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.3
     },
     new Parfume
     {
         ParfumeId = 8,
         Name = "Ombre Leather",
         Author = "Tom Ford",
         Gender = "Unisex",
         description = "A leather fragrance with a bold, smoky feel.",
         TopNotes = "Cardamom",
         MiddleNotes = "Leather,Jasmine Sambac",
         BaseNotes = "Amber,Moss",
         ListPrice = 160,
         Price = 150,
         Price50 = 140,
         Price100 = 130,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.0
     },
     new Parfume
     {
         ParfumeId = 9,
         Name = "La Nuit de L'Homme",
         Author = "Yves Saint Laurent",
         Gender = "Male",
         description = "A seductive fragrance with a blend of spices and woods.",
         TopNotes = "Cardamom",
         MiddleNotes = "Lavender,Bergamot",
         BaseNotes = "Vetiver,Cedar",
         ListPrice = 110,
         Price = 100,
         Price50 = 95,
         Price100 = 90,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.3
     },
     new Parfume
     {
         ParfumeId = 10,
         Name = "Bleu de Chanel",
         Author = "Chanel",
         Gender = "Male",
         description = "A fresh, woody fragrance that embodies freedom.",
         TopNotes = "Grapefruit,Lemon",
         MiddleNotes = "Ginger,Nutmeg",
         BaseNotes = "Sandalwood,Cedar",
         ListPrice = 180,
         Price = 170,
         Price50 = 160,
         Price100 = 150,
         ImgUrl = "",
         Quantity = 99,
         Size30 = 30,
         Size50 = 50,
         Size100 = 100,
         Rating = 4.5
     }
 );
        }
    }
}