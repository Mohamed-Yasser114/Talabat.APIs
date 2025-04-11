using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.IServices;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class OrderController : BaseAPIController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
   
        public OrderController(IOrderService orderService,
            IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderToReturnDto), 200)]
        [ProducesResponseType(typeof(APIsResponse), 400)]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var address = _mapper.Map<AddressDto, Address>(orderDto.Address);
            var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, address);
            if (order is null)
                return BadRequest(new APIsResponse(400));
            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>>GetOrdersforUser()
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrdersForUser(buyerEmail);
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderToReturnDto), 200)]
        [ProducesResponseType(typeof(APIsResponse), 404)]
        public async Task<ActionResult<OrderToReturnDto>>GetOrdersForUSer(int id)
        {
            var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderByIdForUserAsync(id, buyerEmail);
            if (order is null)
                return NotFound(new APIsResponse(404));
            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _orderService.GetAllDeliveryMethodsAsync();
            return Ok(deliveryMethods);
        }
    }
}
