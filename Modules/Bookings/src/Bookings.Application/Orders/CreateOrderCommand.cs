using Bookings.Application.Common.Exceptions;
using Bookings.Application.Common.Interfaces;
using Bookings.Domain.Entities;
using FluentValidation;
using MediatR;
using Products.Application.Products;
using Products.Application.Products.Dtos;
using Products.Domain.Entities;
using Shared.Common.Abstraction;
using Shared.Common.Exceptions;

namespace Bookings.Application.Orders;

public record CreateOrderCommand(
    Guid ProductId,
    string Email,
    string PhoneNumber,
    string FirstName,
    string LastName,
    List<CreateOrderItem> OrderItems,
    DateTime OrderDate) : IRequest<Result<Guid>>;

public record CreateOrderItem(
    Guid ProductItemId,
    int Quantity);

public class CreateOrderCommandHandler(
    IBookingsDbContext bookingsDbContext,
    IOrderRepository orderRepository,
    ISender mediatr,
    IValidator<CreateOrderCommand> validator) : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {

        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new RequestValidationException(validationResult.Errors);
        }

        try
        {
            using var transaction = await bookingsDbContext.BeginTransactionAsync(cancellationToken);

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

    private Order CreateOrder(CreateOrderCommand request, Guid orderId, string orderNumber, decimal subTotal, decimal totalAmount)
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
            IsPaid = false,
            PaymentId = string.Empty,
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