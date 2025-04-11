using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Application.Tickets.Dtos;
using Bookings.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Application.Tickets;

public sealed record GetTicketByTicketNumberQuery(string TicketNumber) : IRequest<TicketDto>;

public sealed class GetTicketByTicketNumberQueryHandler(IBookingsDbContext applicationDbContext) : IRequestHandler<GetTicketByTicketNumberQuery, TicketDto>
{
    public async Task<TicketDto> Handle(GetTicketByTicketNumberQuery request, CancellationToken cancellationToken)
    {
        var ticket = await applicationDbContext.Set<Ticket>()
            .SingleOrDefaultAsync(x => x.TicketNumber == request.TicketNumber, cancellationToken: cancellationToken);
        if(ticket is null)
        {
            throw new NotFoundException(nameof(Ticket), request.TicketNumber);
        }
        return ticket.Adapt<TicketDto>();
    }
}
