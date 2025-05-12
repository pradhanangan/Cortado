using Bookings.Application.Orders;
using Bookings.Application.Orders.Dtos;
using Bookings.Application.Tickets.Dtos;
using Bookings.Domain.Entities;
using Mapster;

namespace Bookings.Application.Mappings;

public class BookingMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //config.ForType<CreateBookingCommand, Booking>().Map(dest => dest.Email, src => src.Email)
        //    .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
        //    .Map(dest => dest.ActivityCode, src => src.ActivityCode)
        //    .Map(dest => dest.NumAdults, src => src.NumAdults)
        //    .Map(dest => dest.IsPaid, src => src.IsPaid)
        //    .Map(dest => dest.BookingDate, src => src.BookingDate);

        // Mapping for Booking to BookingDto
        //config.ForType<Booking, BookingDto>()
        //    .Map(dest => dest.Tickets, src => src.Tickets.Adapt<List<TicketDto>>());

        // Mapping for Ticket to TicketDto
        config.ForType<Ticket, TicketDto>();

        //// Mapping to OrderDto
        //config.ForType<(Order, Product), OrderDto>()
        //    .Map(dest => dest.)

        config.ForType<UpdateOrderCommand, Order>().Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            
            
            .Map(dest => dest.IsPaid, src => src.IsPaid)
            ;

       

        // Mapping for Order to OrderDto
        config.ForType<Order, OrderDto>()
            .ConstructUsing(src => new OrderDto(
                src.Id,
                src.OrderNumber,
                src.ProductId,
                src.Email,
                src.PhoneNumber,
                src.OrderDate,
                src.OrderItems.Adapt<List<OrderItemDto>>(), // OrderItems
                src.SubTotal,
                src.TotalAmount,
                src.IsVerified,
                src.IsPaid,
                src.PaymentId,
                src.IsConfirmed
            ));
    }
}
