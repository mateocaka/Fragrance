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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {

        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(OrderHeader obj)
        {
            _db.OrderHeader.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
           var orderFromDb = _db.OrderHeader.FirstOrDefault(u=>u.Id==id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = _db.OrderHeader.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId)) 
            { 
                orderFromDb.SessionId=sessionId;
                
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;

            }
        }
    }
}

