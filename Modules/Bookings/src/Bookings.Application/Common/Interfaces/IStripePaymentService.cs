using Stripe;

namespace Bookings.Application.Common.Interfaces;

public interface IStripePaymentService
{
    Task<PaymentIntent> GetPaymentIntent(string paymentIntentId);
    Task<PaymentIntent> CreatePaymentIntent(long amount);
}
