using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.OrderSpecifications
{
    public class OrderSpec : BaseSpecifications<Order>
    {
        public OrderSpec(string buyerEmail) : base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);

            OrderByDescending = O => O.OrderDate;
        }
        public OrderSpec(int orderId, string buyerEmail) : base(O => O.Id == orderId && O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.DeliveryMethod);
            Includes.Add(O => O.Items);
        }
    }
}
