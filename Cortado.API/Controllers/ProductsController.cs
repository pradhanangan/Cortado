using Microsoft.AspNetCore.Mvc;
using Products.Application.Products;
using Products.Application.ProductItems;
using Products.Application.Products.Dtos;
using Cortado.API.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Cortado.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<ProductDto>> Get()
        {
            return await Mediator.Send(new GetProductsQuery());
        }

        [HttpGet("{id}")]
        public async Task<ProductDto> Get(Guid id)
        {
           return await Mediator.Send(new GetProductByIdQuery(id));
        }

        [HttpGet("code")]
        public async Task<ProductDto> GetProductByCode([FromQuery] string code)
        {
            return await Mediator.Send(new GetProductByCodeQuery(code));
        }

        [HttpPost]
        public async Task<Guid> Post(CreateProductRequest request)
        {
            return await Mediator.Send(new CreateProductCommand(
                request.Code,
                request.Name,
                request.Description,
                request.StartDate,
                request.EndDate
            ));
        }

        [HttpPost]
        [Route("/api/products/{productId:guid}/product-items")]
        public async Task<Guid> CreateProductItem(Guid productId, CreateProductItemRequest request)
        {
            return await Mediator.Send(new CreateProductItemCommand(
                request.ProductId,
                request.Name,
                request.Description,
                request.Variants,
                request.UnitPrice
            ));
        }
    }
}
