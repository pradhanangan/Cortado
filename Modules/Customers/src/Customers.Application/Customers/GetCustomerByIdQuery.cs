using Customers.Application.Common.Exceptions;
using Customers.Application.Common.Interfaces;
using Customers.Application.Customers.Dtos;
using Customers.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Abstraction;

namespace Customers.Application.Customers;

public sealed record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;

public class GetCustomerByIdQueryHandler(ICustomersDbContext dbContext) : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Set<Customer>()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (customer is null)
        {
            throw new NotFoundException(nameof(Customer), request.Id);
        }
        return customer.Adapt<CustomerDto>();
    }
}