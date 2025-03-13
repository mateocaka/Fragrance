using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Fragrance.Data;
using Fragrance.Models;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fragrance.DataAccess.Repository;
using Fragrance.DataAccess.Repository.IRepository;

namespace Fragrance.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {

        private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
      

        public void Update(ShoppingCart obj)
        {
           _db.ShopingCarts.Update(obj);
        }
    }
}
