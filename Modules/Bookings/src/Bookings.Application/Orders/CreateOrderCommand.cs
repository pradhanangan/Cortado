using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using MediatR;
using Products.Application.Products;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;

namespace Bookings.Application.Orders;

public record CreateOrderCommand(Guid ProductId, string Email, string PhoneNumber, string FirstName,
    string LastName, List<CreateOrderItem> OrderItems, DateTime OrderDate) : IRequest<Guid>;

public record CreateOrderItem(Guid ProductItemId, int Quantity);

public class CreateOrderCommandHandler(IBookingsDbContext bookingsDbContext, IOrderRepository orderRepository, ITokenService tokenService, IEmailService emailService, ISender mediatr) : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = await bookingsDbContext.BeginTransactionAsync(cancellationToken);

            var productResult = await mediatr.Send(new GetProductByIdQuery(request.ProductId));
            if (productResult.IsFailure)
            {
                throw new NotFoundException(nameof(Product), request.ProductId);
            }
            //if (product is null)
            //{
            //    throw new NotFoundException(nameof(Product), request.ProductId);
            //}
            var product = productResult.Value;

            // Generate order number
            var orderNumberSequence = await orderRepository.GetNextOrderNumberAsync();
            var orderNumber = $"{product.Code}-{orderNumberSequence}";

            var order = CreateOrder(request, orderNumber);
            bookingsDbContext.Add(order);

            var orderItems = request.OrderItems.Select(item => CreateOrderItem(order.Id, item, product)).ToList();
            foreach (var orderItem in orderItems)
            {
                bookingsDbContext.Add(orderItem);
            }

            await bookingsDbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            var token = tokenService.GenerateBookingVerificationToken(order.Id.ToString(), order.Email);

            var url = $"http://localhost:3000/orders/verify?token={token}";
            var htmlContent = $"<p>Dear {order.FirstName}</p><p>Thank you for requesting ticket(s) for the event <b>{product.Name}</b>. Please verify you order by clicking the link below:</p><a href='{url}'>Verify your email address</a><p>Regards, <br/>NNZWFS</p>";

            // Send email to the user to verify their email address
            await emailService.SendEmailAsync(order.Email, "Verify your email address", htmlContent);
            return order.Id;
        }
        catch (Exception e) { throw; }
    }

    private Order CreateOrder(CreateOrderCommand request, string orderNumber)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            ProductId = request.ProductId,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            OrderDate = request.OrderDate,
            IsPaid = false,
            PaymentId = string.Empty,
        };
    }

    private OrderItem CreateOrderItem(Guid orderId, CreateOrderItem item, ProductDto product)
    {
        var productItem = product.ProductItems.SingleOrDefault(pi => pi.Id == item.ProductItemId);
        var price = productItem?.UnitPrice * item.Quantity ?? 0;

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductItemId = item.ProductItemId,
            OrderId = orderId,
            Quantity = item.Quantity,
            Price = price
        };
    }
}