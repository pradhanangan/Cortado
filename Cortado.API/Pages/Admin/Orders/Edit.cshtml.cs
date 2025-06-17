using Bookings.Application.Orders;
using Bookings.Application.Orders.Dtos;
using Bookings.Application.Tickets;
using Bookings.Application.Tickets.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Products.Application.Products;
using Products.Application.Products.Dtos;

namespace Cortado.API.Pages.Admin.Orders;

[Authorize]
public class EditModel(ISender mediatr) : PageModel
{
    [BindProperty]
    public OrderDto Order { get; set; }

    [BindProperty]
    public ProductDto Product { get; set; }

    public List<TicketDto> Tickets { get; set; }

    public async Task<IActionResult> OnGet(Guid? id)
    {
        if(id is null)
        {
            return NotFound();
        }

        var orderResult = await mediatr.Send(new GetOrderByIdQuery(id.Value));
        Order = orderResult.Value;
        Tickets = Order.OrderItems.SelectMany(oi => oi.Tickets).ToList();
        var productResult = await mediatr.Send(new GetProductByIdQuery(Order.ProductId));
        Product = productResult.Value;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {

        if (Order is null)
        {
            throw new InvalidOperationException("Order not found");
        }

        // Retrieve the original OrderDate value (including time and time zone)
        var originalOrderDate = DateTime.Parse(Request.Form["OriginalOrderDate"]).ToUniversalTime();

        // Combine the submitted date with the original time and time zone
        var submittedDate = DateTime.Parse(Request.Form["Order.OrderDate"]);
        var dt = new DateTime(
            submittedDate.Year,
            submittedDate.Month,
            submittedDate.Day,
            originalOrderDate.Hour,
            originalOrderDate.Minute,
            originalOrderDate.Second,
            originalOrderDate.Kind
        );

        await mediatr.Send(new UpdateOrderCommand(Order.Id,
                Order.ProductId,
                Order.Email,
                Order.PhoneNumber,
                Order.IsVerified,
                Order.IsPaid,
                Order.IsConfirmed,
                Order.OrderDate,
                Order.PaymentId ?? ""
            ));

            return RedirectToPage(new {id=Order.Id});
       
    }

    public async Task<IActionResult> OnPostMarkAsVerified()
    {
        if (Order is null)
        {
            throw new Exception("Order not found");
        }

        var result = await mediatr.Send(new MarkOrderAsVerifiedCommand(Order.Id));

        if (result.IsFailure)
        {
            throw new Exception(result.Error.Name);
        }
        
        return RedirectToPage(new { id = Order.Id });
    }

    public async Task<IActionResult> OnPostMarkAsPaid()
    {
        if (Order is null)
        {
            throw new InvalidOperationException("Order not found");
        }

        var result = await mediatr.Send(new MarkOrderAsPaidCommand(Order.Id, Order.PaymentId));
        
        if(result.IsFailure)
        {
            return new JsonResult(new { success = false, errors = new[] { result.Error.Name } });
        }
        // Optionally, you can add a success message or redirect to another page
        TempData["Message"] = "Order confirmed!";
        // Return success response
        return new JsonResult(new { success = true });
        //return RedirectToPage(new { id = Order.Id });
    }

    public async Task<IActionResult> OnPostMarkAsConfirmed()
    {

      

        if (Order is null)
        {
            throw new InvalidOperationException("Order not found");
        }

        await mediatr.Send(new MarkOrderAsConfirmedCommand(Order.Id));
        // Optionally, you can add a success message or redirect to another page
        TempData["Message"] = "Order confirmed!";
        return RedirectToPage(new { id = Order.Id });
    }

    public async Task<IActionResult> OnPostGenerateTickets()
    {

        // Logic to generate tickets
        // For example:
        // Booking.Tickets = GenerateTickets(Booking.NumTicketsRequested);

        if (Order is null)
        {
            throw new InvalidOperationException("Order not found");
        }

        await mediatr.Send(new CreateTicketCommand(Order.Id));
        // Optionally, you can add a success message or redirect to another page
        TempData["Message"] = "Tickets generated successfully!";
        return RedirectToPage(new { id = Order.Id });
    }
}
