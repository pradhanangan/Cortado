using Bookings.Application.Tickets;
using Microsoft.AspNetCore.Mvc;
using Cortado.API.Contracts;

namespace Cortado.API.Controllers
{
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
        public async Task<List<Guid>> Post(CreateTicketRequest request)
        {
            return await Mediator.Send(new CreateTicketCommand(request.OrderId));
        }

        [HttpPost("verify-ticket")]
        public async Task<IActionResult> VerifyTicket(VerifyTicketWithQrCodeRequest request)
        {
            var response = await Mediator.Send(new VerifyTicketCommand(request.QrCode));
            return Ok(response);
        }
    }
}
