using Bookings.Domain.Entities;
using MediatR;
using Products.Application.Products.Dtos;
using Products.Application.Products;
using Bookings.Application.Common.Exceptions;
using Products.Domain.Entities;
using Bookings.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bookings.Application.Orders;

public record CreateOrderWithPaymentCommand(Guid ProductId, string Email, string PhoneNumber, string FirstName,
    string LastName, List<CreateOrderItem> OrderItems, DateTime OrderDate, bool IsPaid, string PaymentId) : IRequest<Guid>;


public class CreateOrderWithPaymentCommandHandler(IBookingsDbContext bookingsDbContext, IOrderRepository orderRepository, ISender mediatr) : IRequestHandler<CreateOrderWithPaymentCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderWithPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = await bookingsDbContext.BeginTransactionAsync(cancellationToken);

            // check whether order for payment already exist
            var found = await bookingsDbContext.Set<Order>().AsNoTracking().FirstOrDefaultAsync(x => x.PaymentId == request.PaymentId);
            if (found is not null)
            {
                throw new BadRequestException("Order with this payment id already exists");
            }
            //try
            //{
            //    var found = await mediatr.Send(new GetOrderByPaymentIdQuery(request.PaymentId));
            //}
            //catch (NotFoundException)
            //{
            //    // Ignore exception if order not found
            //}

            var productResult = await mediatr.Send(new GetProductByIdQuery(request.ProductId));
            if (productResult.IsFailure)
            {
                throw new NotFoundException(nameof(Product), request.ProductId);
            }

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
                        
            return order.Id;
        }
        catch (Exception e) { throw; }
    }

    private Order CreateOrder(CreateOrderWithPaymentCommand request, string orderNumber)
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
            IsPaid = request.IsPaid,
            PaymentId = request.PaymentId,
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

