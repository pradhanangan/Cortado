using Customers.Application.Customers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Products.Application.Products;

namespace Cortado.API.Pages.Admin.Products;

[Authorize]
public class CreateModel(ISender mediator) : PageModelBase<CreateModel>
{
    [BindProperty]
    public string Code { get; set; } = string.Empty;
    [BindProperty]
    public string Name { get; set; } = string.Empty;
    [BindProperty] 
    public string Description { get; set; } = string.Empty;
    [BindProperty]
    public DateOnly StartDate { get; set; }
    [BindProperty]
    public DateOnly EndDate { get; set; }

    public void OnGet()
    {
        StartDate = DateOnly.FromDateTime(DateTime.Today);
        EndDate = DateOnly.FromDateTime(DateTime.Today);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Retrieve the authenticated customer ID
        var customerId = await GetCustomerIdAsync();
        if (customerId == null)
        {
            // Handle the case where the customer ID is not found
            return Unauthorized();
        }
       
        await mediator.Send(new CreateProductCommand(
            customerId.Value,
            Code,
            Name,
            Description,
            StartDate,
            EndDate
        ));
        return RedirectToPage("./Index");
    }
}

