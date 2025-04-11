using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Application.Tickets;

public sealed record VerifyTicketCommand(string TicketNumber) : IRequest<VerifyTicketCommandResponse>;

public class VerifyTicketCommandHandler(IBookingsDbContext applicationDbContext) : IRequestHandler<VerifyTicketCommand, VerifyTicketCommandResponse>
{
    public async Task<VerifyTicketCommandResponse> Handle(VerifyTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await applicationDbContext.Set<Ticket>().SingleOrDefaultAsync(t => t.TicketNumber == request.TicketNumber, cancellationToken);
        if (ticket == null)
        {
            return new VerifyTicketCommandResponse("Failure", "TicketDoesNotExist"); // Ticket not found
        }

        if(ticket.IsUsed)
        {
            return new VerifyTicketCommandResponse("Failure", "TicketAlreadyUsed"); // Ticket already used
        }

        ticket.IsUsed = true;
        ticket.UsedDate = DateTime.UtcNow;

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return new VerifyTicketCommandResponse("Success", string.Empty); // Successfully updated
    }
}