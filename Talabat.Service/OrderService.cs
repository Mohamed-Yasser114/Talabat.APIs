using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.IRepositories;
using Talabat.Core.IServices;
using Talabat.Core.Specifications.OrderSpecifications;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepo,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService)
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);

            var orderItems = new List<OrderItem>();

            if (basket?.Items?.Count > 0)
            {
                foreach(var item in basket.Items)
                {
                    var product = await _unitOfWork.repository<Product>().GetAsync(item.Id);
                    var ProductItemOrder = new ProductItemOrdered(item.Id, product.Name, product.PictureUrl);
                    var orderItem = new OrderItem(ProductItemOrder, product.Price, item.Quantity);
                    orderItems.Add(orderItem);
                }
            }

            var subTotal = orderItems.Sum(order => order.Price * order.Quantity);

            var deliveryMethod = await _unitOfWork.repository<DeliveryMethod>().GetAsync(deliveryMethodId);

            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal, basket.PaymentIntentId);

            var spec = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);

            var existsOrder = await _unitOfWork.repository<Order>().GetWithSpecAsync(spec);

            if(existsOrder != null)
            {
                _unitOfWork.repository<Order>().Delete(existsOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            await _unitOfWork.repository<Order>().AddAsync(order);

            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return null;
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetAllDeliveryMethodsAsync()
        {
            var deliveryMethodsRepo = _unitOfWork.repository<DeliveryMethod>();
            var deliveryMethods = await deliveryMethodsRepo.GetAllAsync();
            return deliveryMethods;
        }

        public async Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var orderRepo = _unitOfWork.repository<Order>();
            var spec = new OrderSpec(orderId, buyerEmail);
            var order = await orderRepo.GetWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUser(string buyerEmail)
        {
            var ordersRepo = _unitOfWork.repository<Order>();
            var spec = new OrderSpec(buyerEmail);
            var orders = await ordersRepo.GetAllWithSpecAsync(spec);
            return orders;
        }
    }
}
