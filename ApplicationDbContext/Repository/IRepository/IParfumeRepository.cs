using Fragrance.Models;
using Fragrance.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Fragrance.DataAccess.Repository.IRepository;

namespace Fragrance.DataAccess.Repository.IRepository
{
    public interface IParfumeRepository : IRepository<Parfume>
    {
        IQueryable<Parfume> GetAll();
        void Update(Parfume obj);
    }
}