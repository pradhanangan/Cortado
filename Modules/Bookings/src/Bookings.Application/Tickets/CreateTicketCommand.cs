using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Application.Common.Models;
using Bookings.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Products.Application.Products;
using System.Text;

namespace Bookings.Application.Tickets;

public sealed record CreateTicketCommand(Guid OrderId) : IRequest<List<Guid>>;

public class CreateTicketCommandHandler(IBookingsDbContext bookingDbContext, IQrCodeService qrCodeService, IEmailService emailService, ITicketRepository ticketRepository, ISender medaitr) : IRequestHandler<CreateTicketCommand, List<Guid>>
{
    public async Task<List<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var order = await bookingDbContext.Set<Order>().
            Include(o => o.OrderItems).SingleOrDefaultAsync(b => b.Id == request.OrderId);
        
        if(order is null)
        {
            throw new NotFoundException(nameof(Order), request.OrderId);
        }

        var product = await medaitr.Send(new GetProductByIdQuery(order.ProductId));
        
        using var transaction = await bookingDbContext.BeginTransactionAsync(cancellationToken);
        var tickets = new List<Ticket>();


        foreach(var orderItem in order.OrderItems)
        {
            for (int i = 0; i < orderItem.Quantity; i++)
            {
                // Generate a ticket number
                var ticketNumberSequence = await ticketRepository.GetNextTicketNumberAsync();
                var ticketNumber = $"{product.Code}-{ticketNumberSequence}";
                // Generate a QR code for the ticket
                var qrCode = qrCodeService.GenerateQrCode(ticketNumber);
                // Create a ticket
                var ticket = new Ticket
                {
                    Id = Guid.NewGuid(),
                    OrderItemId = orderItem.Id,
                    TicketNumber = ticketNumber,
                    IsUsed = false,
                    Price = orderItem.Price,
                    Status = "Active",
                    QrCode = qrCode
                };
                bookingDbContext.Add(ticket);
                
                await bookingDbContext.SaveChangesAsync(cancellationToken);
                tickets.Add(ticket);
            }
        }

        

        await transaction.CommitAsync(cancellationToken);

        // Send email with tickets to the user with QR code/ Barcode / ticket details OR background job
        // Build the HTML body with embedded QR codes for SendGrid
        var htmlBody = new StringBuilder();
        htmlBody.Append("<html><body><p>Please find your Tickets:</p>");
        var inlineImages = new List<EmailAttachment>();
        var index = 0;
        foreach (var ticket in tickets)
        {
            //var qrCodeBase64 = Convert.ToBase64String(ticket.QrCode!);
            //htmlBody.Append($"<p>QR Code {index + 1}:</p>");
            //htmlBody.Append($"<img src='data:image/png;base64,{qrCodeBase64}' alt='QR Code {index + 1}'/><br/>");
            //index++;
            var qrCodeImage = ticket.QrCode;
            string contentId = $"QRCodeImage{index}"; // Unique Content ID for each QR code
            htmlBody.Append($"<p>QR Code {index + 1}:</p>");
            htmlBody.Append($"<img src='cid:{contentId}' alt='QR Code {index + 1}'/><br/>");

            var qrCodeImageBase64 = Convert.ToBase64String(qrCodeImage!);
            var inlineImage = new EmailAttachment
            (
                qrCodeImageBase64,
                "image/png",
                $"QRCode{index + 1}.png",
                "inline",
                contentId
            );
            inlineImages.Add(inlineImage);

            index++;

        }
        htmlBody.Append("</body></html>");
        try
        {

            await emailService.SendEmailAsync(order.Email, $"Tickets for {product.Code}", htmlBody.ToString(), inlineImages);
        } catch(Exception e)
        {

        }

        return tickets.Select(t => t.Id).ToList();
    }
}
