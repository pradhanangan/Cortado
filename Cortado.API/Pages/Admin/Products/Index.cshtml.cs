using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Products.Application.Products;
using Products.Application.Products.Dtos;

namespace Cortado.API.Pages.Admin.Products;

[Authorize]
public class IndexModel(ISender mediator) : PageModelBase<IndexModel>
{
    public List<ProductDto> Products { get; set; } = new();
    public async Task OnGet()
    {
        var customerId = await GetCustomerIdAsync();
        if(customerId is null)
        {
            return;
        }
        var allProducts = await mediator.Send(new GetProductsByCustomerIdQuery(customerId.Value));
        Products = allProducts;
    }
}
