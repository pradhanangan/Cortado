import { Stripe } from "@stripe/stripe-js";

export function useStripePayment() {
  const createPaymentIntent = async (amount: number) => {
    const response = await fetch(
      "https://localhost:7159/api/payments/create-intent",
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ amount }),
      }
    );

    if (!response.ok) {
      throw new Error("Failed to create payment intent");
    }

    const data = await response.json();
    return data.clientSecret;
  };

  const retrievePaymentIntent = async (
    stripe: Stripe | null,
    clientSecret: string
  ) => {
    try {
      if (!stripe) {
        throw new Error("useStripePayment: Stripe is not initialized");
        // console.error("Stripe is not initialized");
        // return;
      }

      const { paymentIntent } = await stripe.retrievePaymentIntent(
        clientSecret
      );
      if (!paymentIntent) {
        throw new Error("No payment intent found");
      }

      return paymentIntent;
    } catch (error) {
      console.error("Error retrieving payment intent:", error);
      throw error;
    }
  };

  return { retrievePaymentIntent, createPaymentIntent };
}
