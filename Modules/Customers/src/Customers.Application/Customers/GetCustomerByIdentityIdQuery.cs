using Customers.Application.Common.Exceptions;
using Customers.Application.Common.Interfaces;
using Customers.Application.Customers.Dtos;
using Customers.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Abstraction;

namespace Customers.Application.Customers;

public sealed record GetCustomerByIdentityIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;

public class GetCustomerByIdentityIdQueryHandler(ICustomersDbContext dbContext) : IRequestHandler<GetCustomerByIdentityIdQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdentityIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Set<Customer>()
            .FirstOrDefaultAsync(c => c.IdentityId == request.Id, cancellationToken);
        if (customer is null)
        {
            throw new NotFoundException(nameof(Customer), request.Id);
        }
        return customer.Adapt<CustomerDto>();
    }
}