using Customers.Application.Customers;
using Customers.Application.Customers.Dtos;
using Microsoft.AspNetCore.Mvc;
using Cortado.API.Contracts;

namespace Cortado.API.Controllers;

[Route("api/customers")]
[ApiController]
public class CustomersController : ApiControllerBase<CustomersController>
{
    [HttpGet]
    public async Task<IEnumerable<CustomerDto>> Get()
    {
        var result = await Mediator.Send(new GetCustomersQuery());
        return result.Value;
    }

    [HttpGet("{id}")]
    public async Task<CustomerDto> Get(Guid id)
    {
        var result = await Mediator.Send(new GetCustomerByIdQuery(id));
        return result.Value;
    }

    [HttpGet("username")]
    public async Task<CustomerDto> GetCustomerByUsername([FromQuery] string username)
    {
        var result = await Mediator.Send(new GetCustomerByUsernameQuery(username));
        return result.Value;
    }

    [HttpGet("email")]
    public async Task<CustomerDto> GetCustomerByEmail([FromQuery] string email)
    {
        var result = await Mediator.Send(new GetCustomerByEmailQuery(email));
        return result.Value;
    }

    [HttpPost]
    public async Task<Guid> Post(CreateCustomerRequest request)
    {
        var result = await Mediator.Send(new CreateCustomerCommand(
            request.Username,
            request.Email,
            request.IdentityId
        ));
        return result.Value;
    }
}
