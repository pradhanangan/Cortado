using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.Products;
using Products.Application.Products.Dtos;

namespace Cortado.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + CookieAuthenticationDefaults.AuthenticationScheme)]
[Route("api/admin")]
[ApiController]
public class AdminController : ApiControllerBase<AdminController>
{
    // GET: /api/admin/products
    [HttpGet("products")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsForAdmin()
    {
        var customerId = await GetCustomerIdAsync();
        if (customerId == null)
        {
            return Unauthorized("Customer not found.");
        }
        var products = await Mediator.Send(new GetProductsForAdminQuery());
        return Ok(products.Value);
    }
}
