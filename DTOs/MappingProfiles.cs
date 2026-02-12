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
                .ForMember(d => d.PromotionTitle, o => o.MapFrom(s => s.Promotion.Title));

            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.BookCount, o => o.MapFrom(s => s.Books.Count));

            
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderItemDto, OrderItem>();
        }
    }
}
