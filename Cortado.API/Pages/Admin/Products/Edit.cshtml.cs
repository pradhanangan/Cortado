using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Products.Application.Products;
using Products.Application.Products.Dtos;

namespace Cortado.API.Pages.Admin.Products
{
    [Authorize]
    public class EditModel (ISender mediator): PageModel
    {
        public ProductDto Product { get; set; }
        public async Task<IActionResult> OnGet(Guid? id)
        {
            if(id is null) { return NotFound(); }

            var productResult = await mediator.Send(new GetProductByIdQuery(id.Value));
            Product = productResult.Value;
            return Page();
        }
    }
}
