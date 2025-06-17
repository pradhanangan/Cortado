using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Products.Application.ProductItems;

namespace Cortado.API.Pages.Admin.ProductItems
{
    [Authorize]
    public class CreateModel(ISender mediator) : PageModelBase<CreateModel>
    {
        [BindProperty]
        public Guid ProductId { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Description { get; set; } = string.Empty;
        [BindProperty]
        public string Variants { get; set; } = string.Empty;
        [BindProperty]
        public bool IsFree { get; set; }

        [BindProperty]
        public decimal UnitPrice { get; set; } = 0;

        public void OnGet(Guid productId)
        {
            ProductId = productId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();

            }

            await mediator.Send(new CreateProductItemCommand(ProductId, Name, Description, Variants, IsFree, UnitPrice));
            return RedirectToPage("/Admin/Products/Edit", new { id = ProductId } );
        }
    }
}
