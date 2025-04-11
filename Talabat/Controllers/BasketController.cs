using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;

namespace Talabat.APIs.Controllers
{
    public class BasketController : BaseAPIController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository,
            IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string basketId)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId);
            return Ok(basket ?? new CustomerBasket(basketId)); 
        }
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket([FromBody] CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto,CustomerBasket>(basket);
            var createdOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
            if (createdOrUpdatedBasket == null)
                return BadRequest(new APIsResponse(400));
            return Ok(createdOrUpdatedBasket);
        }
        [HttpDelete]
        public async Task DeleteBasket(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }
    }
}
