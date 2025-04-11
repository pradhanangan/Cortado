using Bookings.Application.Orders;
using Bookings.Application.Orders.Dtos;
using Microsoft.AspNetCore.Mvc;
using Cortado.API.Contracts;

namespace Cortado.API.Controllers;

[Route("api/orders")]
[ApiController]
public class OrdersController : ApiControllerBase
{
    [HttpGet("{id}")]
    public async Task<OrderDto> Get(Guid id)
    {
        return await Mediator.Send(new GetOrderByIdQuery(id));
    }

    [HttpPost]
    public async Task<Guid> Post(CreateOrderRequest request)
    {
        return await Mediator.Send(new CreateOrderCommand(
            request.ProductId,
            request.Email,
            request.PhoneNumber,
            request.FirstName,
            request.LastName,
            request.OrderItems.Select(item => new CreateOrderItem(item.ProductItemId, item.Quantity)).ToList(),
            request.OrderDate
        ));
    }

    [HttpPost("with-payment")]
    public async Task<Guid> CreateOrderWithoutPayment(CreateOrderWithPaymentRequest request)
    {
         return await Mediator.Send(new CreateOrderWithPaymentCommand(
            request.ProductId,
            request.Email,
            request.PhoneNumber,
            request.FirstName,
            request.LastName,
            request.OrderItems.Select(item => new CreateOrderItem(item.ProductItemId, item.Quantity)).ToList(),
            request.OrderDate,
            request.IsPaid,
            request.PaymentId
        ));
    }

    [HttpPut]
    public async Task<Guid> Put(UpdateOrderRequest request)
    {
        return await Mediator.Send(new UpdateOrderCommand(request.Id,
            request.ProductId,
            request.Email,
            request.PhoneNumber,
            true,
            request.IsPaid,
            true,
            request.OrderDate,
            request.OrderItems.Select(item => new CreateOrderItem(item.ProductItemId, item.Quantity)).ToList(),
            request.PaymentId
        ));
    }

    [HttpGet("verify-order")]
    public async Task<ActionResult<VerifyOrderResponse>> VerifyOrder(string token)
    {
        var response = await Mediator.Send(new VerifyOrderCommand(token));
        return Ok(response);
    }

    [HttpPut("{orderId}/mark-as-paid")]
    public async Task<IActionResult> MarkAsPaid(Guid orderId, [FromBody] MarkAsPaidRequest request)
    {
        if (orderId == Guid.Empty || string.IsNullOrEmpty(request.PaymentId))
        {
            return BadRequest("Invalid order ID or payment ID.");
        }

        await Mediator.Send(new MarkOrderAsPaidCommand(orderId, request.PaymentId));
        return Ok();
    }

    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusRequest request)
    {
        await Mediator.Send(new UpdateOrderStatusCommand(request.OrderId, request.PaymentId, request.IsPaid, request.IsConfirmed));
        return Ok();
    }
}
