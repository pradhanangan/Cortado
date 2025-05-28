using Cortado.API.Contracts;
using Cortado.API.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.ProductItems;
using Products.Application.Products;
using Products.Application.Products.Dtos;

namespace Cortado.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + CookieAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/products")]
    [ApiController]
    public class ProductsController: ApiControllerBase<ProductsController>
    {
        [AllowAnonymous]
        [HttpGet("token")]
        public async Task<ActionResult<ProductDto>> GetProductByToken([FromQuery] string token)
        {
            var result = await Mediator.Send(new GetProductByTokenQuery(token));
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToProblemDetails());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
        {
            Logger.LogInformation("{Controller} called", nameof(ProductsController));
            var customerId = await GetCustomerIdAsync();
            if (customerId == null)
            {
                return Unauthorized("Customer not found.");
            }
            var products = await Mediator.Send(new GetProductsQuery(customerId.Value));
            return Ok(products.Value);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(Guid id)
        {
            var result = await Mediator.Send(new GetProductByIdQuery(id));
            return result.Value;
        }
        
        [HttpGet("code")]
        public async Task<ActionResult<ProductDto>> GetProductByCode([FromQuery] string code)
        {
            var result = await Mediator.Send(new GetProductByCodeQuery(code));
            return result.Value;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProductsByCode([FromQuery] string code)
        {
            var result = await Mediator.Send(new SearchProductsByCodeQuery(code));
            return result.Value;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Guid>> Post([FromForm] CreateProductRequest request)
        {
            var customerId = await GetCustomerIdAsync();
            if (customerId == null)
            {
                return Unauthorized("Customer not found.");
            }
            
            using var imgStream = request.Image.OpenReadStream();
            var productId = await Mediator.Send(new CreateProductCommand(
                customerId.Value,
                request.Code,
                request.Name,
                request.Description,
                imgStream,
                request.Image.FileName,
                request.Address,
                request.StartDate,
                request.EndDate,
                request.StartTime,
                request.EndTime
            ));
            return CreatedAtAction(nameof(Post), productId);
        }

        [HttpPost]
        [Route("/api/products/{productId:guid}/product-items")]
        public async Task<ActionResult<Guid>> CreateProductItem(Guid productId, CreateProductItemRequest request)
        {
            var result = await Mediator.Send(new CreateProductItemCommand(
                request.ProductId,
                request.Name,
                request.Description,
                request.Variants,
                request.IsFree,
                request.UnitPrice
            ));
            return result.Value;
        }
    }
}