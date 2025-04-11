using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IServices;
using Stripe;
using Microsoft.Extensions.Configuration;
using Talabat.Core.IRepositories;
using Talabat.Core;
using Product = Talabat.Core.Entities.Product;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Specifications.OrderSpecifications;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration,
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:Secretkey"];
            var basket = await _basketRepo.GetBasketAsync(basketId);
            if(basket is null)
                return null;
            if(basket.Items.Count > 0)
            {
                foreach(var item in basket.Items)
                {
                    var product = await _unitOfWork.repository<Product>().GetAsync(item.Id);
                    if (item.Price != product.Price)
                        item.Price = product.Price;
                }
            }
            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod.Cost;
                basket.ShippingPrice = deliveryMethod.Cost;
            }
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(I => I.Price * I.Quantity * 100) + (long)basket.ShippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>(){ "card" }
                };
                paymentIntent = await paymentIntentService.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(I => I.Price * I.Quantity * 100) + (long)basket.ShippingPrice * 100,
                };
                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);
            }
            var ubdatedBasket = await _basketRepo.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceededOrFailed(string paymentIntentId, bool isSucceeded)
        {
            var spec = new OrderWithPaymentIntentSpecification(paymentIntentId);
            var order = await _unitOfWork.repository<Order>().GetWithSpecAsync(spec);
            if (isSucceeded)
                order.Status = OrderStatus.PaymentSucceded;
            else
                order.Status = OrderStatus.PaymentFailed;
            _unitOfWork.repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();
            return order;
        }
    }
}
