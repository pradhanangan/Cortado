using Mapster;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;

namespace Products.Application.Mappings;

public class ProductMappingRegister:IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        
        // Mapping for Booking to BookingDto
        //config.ForType<Product, ProductDto>()
        //    .Map(dest => dest.ProductItems, src => src.ProductItems.Adapt<List<ProductItemDto>>());

        config.NewConfig<Product, ProductDto>()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Code, src => src.Code)
              .Map(dest => dest.Name, src => src.Name)
              .Map(dest => dest.Description, src => src.Description)
              .Map(dest => dest.ProductItems,
                   src => src.ProductItems.Select(pi => new ProductItemDto(
                       pi.Id, pi.Name, pi.Description, pi.Variants, pi.UnitPrice)).ToList());


    }
}
