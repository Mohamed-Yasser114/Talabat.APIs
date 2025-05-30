﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.IServices
{
    public interface IPaymentService
    {
        public Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
        public Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool isSucceeded);
    }
}
