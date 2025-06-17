using Bookings.Application.Orders;
using Bookings.Application.Orders.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cortado.API.Pages.Admin.Orders;

[Authorize]
public class IndexModel(ISender mediatr) : PageModel
{
    public string ProductCode { get; set; } = string.Empty;
    public List<OrderDto> Orders { get; set; } = new();
    public async Task OnGet(string productCode)
    {
        ProductCode = productCode;
        if (!string.IsNullOrEmpty(ProductCode))
        {
            var result = await mediatr.Send(new GetOrdersByProductCodeQuery(ProductCode));
            if (result.IsSuccess)
            {

                Orders = result.Value;
            }
        }
    }
}