using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entities;

namespace DTOs
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Book, BookDto>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
                .ForMember(d => d.PromotionTitle, o => o.MapFrom(s => s.Promotion != null ? s.Promotion.Title : null));

            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.BookCount, o => o.MapFrom(s => s.Books.Count));

            
            CreateMap<OrderCreateDto, Order>()
                .ForMember(d => d.ShippingCity, o => o.MapFrom(s => s.ShippingCityId));
            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(d => d.PriceAtPurchase, o => o.Ignore())
                .ForMember(d => d.OrderId, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore());
            CreateMap<Order, OrderTrackingDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<CreatePromotionDto, Promotion>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Name));
            CreateMap<Promotion, PromotionDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Title));

            CreateMap<UpdateOrCreateBookDto, Book>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ImageUrl, o => o.Ignore());
            CreateMap<Book, BookListDto>()
                 .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name));
            CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));
            CreateMap(typeof(CategoryBooksResult<>), typeof(CategoryBooksResult<>));
            CreateMap<NewUserDto, Customer>();
            CreateMap<Customer, CustomerDto>();
            CreateMap<Customer, LoginDto>();
            CreateMap<UpdateUserDto, Customer>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // מניעת דריסה של ה-ID
        }
    }
}
