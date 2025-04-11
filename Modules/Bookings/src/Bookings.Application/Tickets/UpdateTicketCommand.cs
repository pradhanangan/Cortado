using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Application.Tickets;

public sealed record UpdateTicketCommand(Guid TicketId, string TicketNumber, bool IsUsed, decimal Price, string Status) : IRequest<bool>;

public class UpdateTicketCommandHandler(IBookingsDbContext applicationDbContext) : IRequestHandler<UpdateTicketCommand, bool>
{
    public async Task<bool> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await applicationDbContext.Set<Ticket>().FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);
        if (ticket == null)
        {
            return false; // Ticket not found
        }

        ticket.TicketNumber = request.TicketNumber;
        ticket.IsUsed = request.IsUsed;
        ticket.Price = request.Price;
        ticket.Status = request.Status;
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
