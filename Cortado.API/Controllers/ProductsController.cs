using Cortado.API.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Products.Application.ProductItems;
using Products.Application.Products;
using Products.Application.Products.Dtos;

namespace Cortado.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/products")]
    [ApiController]
    public class ProductsController: ApiControllerBase<ProductsController>
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
        {
            var products = await Mediator.Send(new GetProductsQuery());
            return Ok(products);
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

        [HttpPost]
        public async Task<ActionResult<Guid>> Post(CreateProductRequest request)
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
                request.UnitPrice
            ));
            return result.Value;
        }
    }
}
