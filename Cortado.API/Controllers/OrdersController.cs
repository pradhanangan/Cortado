using Bookings.Application.Orders;
using Bookings.Application.Orders.Dtos;
using Cortado.API.Contracts;
using Cortado.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Abstraction;

namespace Cortado.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/orders")]
[ApiController]
public class OrdersController : ApiControllerBase<OrdersController>
{
    [HttpGet("{id}")]
    public async Task<OrderDto> Get(Guid id)
    {
        var response = await Mediator.Send(new GetOrderByIdQuery(id));
        return response.Value;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<Guid>> Post(CreateOrderRequest request)
    {
        var response = await Mediator.Send(new CreateOrderCommand(
            request.ProductId,
            request.Email,
            request.PhoneNumber,
            request.FirstName,
            request.LastName,
            request.OrderItems.Select(item => new CreateOrderItem(item.ProductItemId, item.Quantity)).ToList(),
            request.OrderDate
        ));
        return CreatedAtAction(nameof(Post), response.Value);
    }

    [AllowAnonymous]
    [HttpPost("with-email")]
    public async Task<ActionResult<Guid>> CreateOrderWithEmail(CreateOrderRequest request)
    {
        var response = await Mediator.Send(new CreateOrderWithEmailCommand(
            request.ProductId,
            request.Email,
            request.PhoneNumber,
            request.FirstName,
            request.LastName,
            request.OrderItems.Select(item => new CreateOrderItem(item.ProductItemId, item.Quantity)).ToList(),
            request.OrderDate
        ));
        return CreatedAtAction(nameof(Post), response.Value);
    }

    [AllowAnonymous]
    [HttpPost("with-payment")]
    public async Task<ActionResult<Guid>> CreateOrderWithPayment(CreateOrderWithPaymentRequest request)
    {
         var response = await Mediator.Send(new CreateOrderWithPaymentCommand(
            request.ProductId,
            request.Email,
            request.PhoneNumber,
            request.FirstName,
            request.LastName,
            request.OrderDate,
            request.OrderItems.Select(item => new CreateOrderItem(item.ProductItemId, item.Quantity)).ToList(),
            request.PaymentId
        ));
        return CreatedAtAction(nameof(Post), response.Value);
    }

    [HttpPut]
    public async Task<ActionResult<Guid>> Put(UpdateOrderRequest request)
    {
        var response = await Mediator.Send(new UpdateOrderCommand(request.Id,
            request.ProductId,
            request.Email,
            request.PhoneNumber,
            true,
            request.IsPaid,
            true,
            request.OrderDate,
            request.PaymentId
        ));
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("verify-order")]
    public async Task<ActionResult<VerifyOrderResponse>> VerifyOrder(string token)
    {
        var result = await Mediator.Send(new VerifyOrderCommand(token));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToProblemDetails());
    }

    [HttpPut("{orderId}/mark-as-verified")]
    public async Task<IActionResult> MarkAsVerified(Guid orderId)
    {
        var response = await Mediator.Send(new MarkOrderAsVerifiedCommand(orderId));
        return Ok();
    }

    [AllowAnonymous]
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

    [HttpPut("{orderId}/mark-as-confirmed")]
    public async Task<IActionResult> MarkAsConfirmed(Guid orderId)
    {
        await Mediator.Send(new MarkOrderAsConfirmedCommand(orderId));
        return Ok();
    }

    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusRequest request)
    {
        await Mediator.Send(new UpdateOrderStatusCommand(request.OrderId, request.PaymentId, request.IsPaid, request.IsConfirmed));
        return Ok();
    }
}
