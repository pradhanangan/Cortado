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

        Order = await mediatr.Send(new GetOrderByIdQuery(id.Value));
        Tickets = Order.OrderItems.SelectMany(oi => oi.Tickets).ToList();
        Product = await mediatr.Send(new GetProductByIdQuery(Order.ProductId));
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {

        if (Order is null)
        {
            throw new InvalidOperationException("Order not found");
        }

        await mediatr.Send(new UpdateOrderCommand(Order.Id,
                Order.ProductId,
                Order.Email,
                Order.PhoneNumber,
                Order.IsEmailVerified,
                Order.IsPaid,
                Order.IsConfirmed,
                Order.OrderDate,
                new(),
                Order.PaymentId
            ));

            return RedirectToPage(new {id=Order.Id});
       
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
