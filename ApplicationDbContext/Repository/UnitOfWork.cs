using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fragrance.DataAccess.Repository;
using Fragrance.Models;
using Fragrance.DataAccess.Repository;

namespace Fragrance.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {


        private ApplicationDbContext _db;
        public IParfumeRepository Parfume { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; } 
        public IOrderDetailRepository OrderDetail { get; private set; }
        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Parfume = new ParfumeRepository(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);   

        }
       

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
