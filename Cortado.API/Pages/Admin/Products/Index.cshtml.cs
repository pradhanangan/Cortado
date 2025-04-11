using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Products.Application.Products;
using Products.Application.Products.Dtos;

namespace Cortado.API.Pages.Admin.Products;

[Authorize]
public class IndexModel(ISender mediator) : PageModel
{
    public List<ProductDto> Products { get; set; } = new();
    public async Task OnGet()
    {
        var allProducts = await mediator.Send(new GetProductsQuery());
        Products = allProducts;
    }
}
