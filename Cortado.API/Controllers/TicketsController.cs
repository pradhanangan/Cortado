using Bookings.Application.Tickets;
using Microsoft.AspNetCore.Mvc;
using Cortado.API.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Cortado.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/tickets")]
    [ApiController]
    public class TicketsController : ApiControllerBase<TicketsController>
    {
        //public async Task<IEnumerable<TicketDto>> Get()
        //{
        //    return await Mediator.Send(new GetTicketsQuery());
        //}

        //public async Task<TicketDto> GetTicketByTicketNumber(Guid id)
        //{
        //    return await Mediator.Send(new GetTicketByIdQuery(id));
        //}

        [HttpPost]
        public async Task<ActionResult<List<Guid>>> Post(CreateTicketRequest request)
        {
            var response = await Mediator.Send(new CreateTicketCommand(request.OrderId));
            return Ok(response);
        }

        [HttpPost("verify-ticket")]
        public async Task<IActionResult> VerifyTicket(VerifyTicketWithQrCodeRequest request)
        {
            var response = await Mediator.Send(new VerifyTicketCommand(request.QrCode));
            return Ok(response);
        }
    }
}
