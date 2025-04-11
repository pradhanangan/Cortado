namespace Bookings.API.Contracts;

public sealed record CreateBookingRequest(string ActivityCode, string Email, string PhoneNumber, string FirstName, string LastName, int NumAdults, int NumKids, bool IsPaid, DateTime BookingDate);
