using Bookings.Application.Common.Interfaces;
using Bookings.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Cortado.API.Contracts;
using Stripe;

namespace Cortado.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ApiControllerBase<PaymentsController>
    {
        private readonly IStripePaymentService _stripePaymentService;

        public PaymentsController(IStripePaymentService stripePaymentService)
        {
            _stripePaymentService = stripePaymentService;
        }

        [HttpGet("get-intent")]
        public async Task<IActionResult> GetPaymentIntent([FromQuery] string paymentIntentId)
        {
            return Ok(await _stripePaymentService.GetPaymentIntent(paymentIntentId));
        }

        [HttpPost("create-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentRequest request)
        {
            return Ok(await _stripePaymentService.CreatePaymentIntent(request.Amount));
        }
    }
}
