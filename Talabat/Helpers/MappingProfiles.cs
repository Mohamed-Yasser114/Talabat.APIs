using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using UserAddress = Talabat.Core.Entities.Identity.Address;
using OrderAddress = Talabat.Core.Entities.Order_Aggregate.Address;

namespace Talabat.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        

        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(D => D.Brand, O => O.MapFrom(S => S.Brand.Name))
                .ForMember(D => D.Category, O => O.MapFrom(S => S.Category.Name))
                .ForMember(D => D.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBasketDto, CustomerBasket>();

            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<AddressDto, OrderAddress>();

            CreateMap<UserAddress, AddressDto>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(D => D.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName))
                .ForMember(D => D.DeliveryMethodCost, O => O.MapFrom(S => S.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(D => D.ProductId, O => O.MapFrom(S => S.product.ProductId))
                .ForMember(D => D.ProductName, O => O.MapFrom(S => S.product.ProductName))
                .ForMember(D => D.PictureUrl, O => O.MapFrom(S => S.product.PictureUrl))
                .ForMember(D => D.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());
        }
    }
}
