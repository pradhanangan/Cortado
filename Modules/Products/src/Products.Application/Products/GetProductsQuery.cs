using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Common.Interfaces;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;
using Shared.Common.Abstraction;

namespace Products.Application.Products;

public record GetProductsQuery : IRequest<Result<List<ProductDto>>>;

public class GetProductsQueryHandler(IServiceScopeFactory serviceScopeFactory, IProductDbContext productDbContext) : IRequestHandler<GetProductsQuery, Result<List<ProductDto>>>
{
    public async Task<Result<List<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        
       
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IProductDbContext>();
        if (dbContext == null) {
            throw new Exception("dbcontext is null");
        }
        //var products = await dbContext.Set<Product>().ToListAsync(cancellationToken);

        //return products.Select(p => new ProductDto(
        //    p.Id,
        //    p.Code,
        //    p.Name, 
        //    p.Description, 
        //    p.ProductItems.Select(pi => new ProductItemDto(pi.Id, pi.Variants, pi.Description, pi.Price)).ToList()
        //    )).ToList();
        ////return products.Select(p => new ProductDto { Id = p.Id, Name = p.Name })
        //.ToList();
        //var products = await productDbContext.Set<Product>().Include(p=>p.ProductItems).AsNoTracking().ToListAsync(cancellationToken);
        //var count = products.Count();
        //var dtos = products.Adapt<List<ProductDto>>();
        //return dtos;
        //return await products.ProjectToType<ProductDto>().ToListAsync(cancellationToken);
        //var products = await dbContext.Set<Product>().ToListAsync(cancellationToken);

        var products = dbContext.Set<Product>().Include(p => p.ProductItems).AsNoTracking();
        var count = products.Count();
        var pdto = await products.ProjectToType<ProductDto>().ToListAsync();
        return pdto;

        //var result = dbContext.Set<Product>()
        //    .Select(p => new ProductDto(
        //    p.Id,
        //    p.Code,
        //    p.Name,
        //    p.Description,
        //    p.ProductItems.Select(pi => new ProductItemDto(pi.Id, pi.Variants, pi.Description, pi.Price)).ToList()
        //    )).ToList();
        //return result;
    }
}

