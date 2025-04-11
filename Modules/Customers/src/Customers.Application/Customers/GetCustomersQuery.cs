using Customers.Application.Common.Interfaces;
using Customers.Application.Customers.Dtos;
using Customers.Domain.Common;
using Customers.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Application.Customers;

public sealed record GetCustomersQuery : IRequest<Result<List<CustomerDto>>>;

public class GetCustomersQueryHandler(IServiceScopeFactory serviceScopeFactory) : IRequestHandler<GetCustomersQuery, Result<List<CustomerDto>>>
{
    public async Task<Result<List<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {

        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ICustomersDbContext>();
        if (dbContext == null)
        {
            throw new Exception("dbcontext is null");
        }
        
        return await dbContext.Set<Customer>().AsNoTracking().ProjectToType<CustomerDto>().ToListAsync();
        
    }
}
