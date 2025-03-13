using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fragrance.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IParfumeRepository Parfume { get; }

        ICompanyRepository Company { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderDetailRepository OrderDetail { get; } 
        IOrderHeaderRepository OrderHeader { get; }
        void Save();
    }
}
