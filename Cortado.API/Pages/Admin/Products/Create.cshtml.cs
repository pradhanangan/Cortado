using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public string Address { get; set; } = string.Empty;
    [BindProperty]
    public DateOnly StartDate { get; set; }
    [BindProperty]
    public DateOnly EndDate { get; set; }
    [BindProperty]
    public TimeOnly StartTime { get; set; }
    [BindProperty]
    public TimeOnly EndTime { get; set; }
    [BindProperty]
    public IFormFile? Image { get; set; } // Bind the uploaded file

    public void OnGet()
    {
        StartDate = DateOnly.FromDateTime(DateTime.Today);
        EndDate = DateOnly.FromDateTime(DateTime.Today);
        StartTime = TimeOnly.Parse("08:00 AM"); 
        EndTime = TimeOnly.Parse("08:00 AM");
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

        // Convert the file to a byte array or stream (if needed)
        using var memoryStream = new MemoryStream();
        if (Image != null)
        {
            await Image.CopyToAsync(memoryStream);
        }
        
        var productId = await mediator.Send(new CreateProductCommand(
            customerId.Value,
            Code,
            Name,
            Description,
            memoryStream,
            Image?.FileName ?? "",
            Address,
            StartDate,
            EndDate,
            StartTime,
            EndTime
        ));
        return RedirectToPage("./Index");
    }
}

