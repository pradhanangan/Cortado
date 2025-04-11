using Bookings.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Bookings.Infrastructure.Services;

public class StripePaymentService:IStripePaymentService
{
    private readonly string _stripeSecretKey;

    public StripePaymentService(IConfiguration configuration)
    {
        _stripeSecretKey = configuration["Stripe:SecretKey"];
        StripeConfiguration.ApiKey = _stripeSecretKey;
    }

    public async Task<PaymentIntent> CreatePaymentIntent(long amount)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                //AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true }
            };
            var a = await paymentIntentService.CreateAsync(options);
            return a;
            //return await paymentIntentService.CreateAsync(options);
            
        } catch(Exception e)
        {
            throw;
        }
    }

    public async Task<PaymentIntent> GetPaymentIntent(string paymentIntentId)
    {
        try
        {
            var paymentIntentService = new PaymentIntentService();
            return await paymentIntentService.GetAsync(paymentIntentId);
        } catch(Exception e)
        {
            throw;
        }
    }
}
