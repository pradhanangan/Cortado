using Bookings.Domain.Entities;
using MediatR;
using Products.Application.Products.Dtos;
using Products.Application.Products;
using Bookings.Application.Common.Exceptions;
using Products.Domain.Entities;
using Bookings.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Abstraction;

namespace Bookings.Application.Orders;

public record CreateOrderWithPaymentCommand(Guid ProductId, string Email, string PhoneNumber, string FirstName,
    string LastName, DateTime OrderDate, List<CreateOrderItem> OrderItems, string PaymentId) : IRequest<Result<Guid>>;


public class CreateOrderWithPaymentCommandHandler(IBookingsDbContext bookingsDbContext, IOrderRepository orderRepository, ISender mediatr) : IRequestHandler<CreateOrderWithPaymentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderWithPaymentCommand request, CancellationToken cancellationToken)
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

            // Generate order id
            var orderId = Guid.NewGuid();

            // Create order items
            var orderItems = request.OrderItems.Select(item => CreateOrderItem(orderId, item, product)).ToList();

            // Generate order number
            var orderNumberSequence = await orderRepository.GetNextOrderNumberAsync();
            var orderNumber = $"{product.Code}-{orderNumberSequence}";

            // Calculate sub total and total amount
            var subTotal = orderItems.Sum(oi => oi.LineTotal);
            var totalAmount = subTotal; // + product.Tax; // TODO: Add tax to order item

            // Create order
            var order = CreateOrder(request, orderId, orderNumber, subTotal, totalAmount);

            // Add order to the database
            bookingsDbContext.Add(order);

            // Add order items to the database
            foreach (var orderItem in orderItems)
            {
                bookingsDbContext.Add(orderItem);
            }

            // Save changes to the database
            await bookingsDbContext.SaveChangesAsync(cancellationToken);

            // Commit the transaction
            await transaction.CommitAsync(cancellationToken);
                        
            return order.Id;
        }
        catch (Exception e) { throw; }
    }

    private Order CreateOrder(CreateOrderWithPaymentCommand request, Guid orderId, string orderNumber, decimal subTotal, decimal totalAmount)
    {
        return new Order
        {
            Id = orderId,
            OrderNumber = orderNumber,
            ProductId = request.ProductId,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FirstName = request.FirstName,
            LastName = request.LastName,
            OrderDate = request.OrderDate,
            SubTotal = subTotal,
            TotalAmount = totalAmount,
            IsPaid = true,
            PaymentId = request.PaymentId,
        };
    }

    private OrderItem CreateOrderItem(Guid orderId, CreateOrderItem item, ProductDto product)
    {
        var productItem = product.ProductItems.SingleOrDefault(pi => pi.Id == item.ProductItemId);

        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductItemId = item.ProductItemId,
            Quantity = item.Quantity,
            UnitPrice = productItem?.UnitPrice ?? 0,
            LineTotal = productItem?.UnitPrice * item.Quantity ?? 0,
        };
    }
}

