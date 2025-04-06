using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fragrance.Data;

namespace Fragrance.DataAccess.Repository
{
    internal class ParfumeRepository : Repository<Parfume>, IParfumeRepository
    {
        private ApplicationDbContext _db;
        public ParfumeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Parfume obj)
        {
            var objFromDb = _db.Parfumes.FirstOrDefault(u=>u.ParfumeId == obj.ParfumeId);
            if (objFromDb != null) 
            {
                objFromDb.Name = obj.Name;
                objFromDb.Author = obj.Author;
                objFromDb.Gender = obj.Gender;
                objFromDb.description = obj.description;
                objFromDb.TopNotes = obj.TopNotes;
                objFromDb.MiddleNotes = obj.MiddleNotes;
                objFromDb.BaseNotes = obj.BaseNotes;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Quantity = obj.Quantity;
                objFromDb.Size30 = obj.Size30;
                objFromDb.Size50 = obj.Size50;
                objFromDb.Size100 = obj.Size100;
                objFromDb.Rating = obj.Rating;
                if (obj.ImgUrl != null) 
                {
                    objFromDb.ImgUrl= obj.ImgUrl;
                }

            }

        }


    }
}

