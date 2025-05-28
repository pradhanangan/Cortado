import { useState, useEffect } from "react";
import { useStripe } from "@stripe/react-stripe-js";
import { OrderDto, PaymentStatus } from "@/types/orders-module";

interface PaymentCompleteState {
  loading: boolean;
  error: string | null;
  order: OrderDto | null;
  status: PaymentStatus;
  intentId: string | null;
}

export function usePaymentComplete(): PaymentCompleteState {
  const stripe = useStripe();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [status, setStatus] = useState<PaymentStatus>("default");
  const [intentId, setIntentId] = useState<string | null>(null);

  const retrievePaymentIntent = async (clientSecret: string) => {
    if (!stripe) {
      throw new Error("Stripe is not initialized");
    }

    const { paymentIntent } = await stripe.retrievePaymentIntent(clientSecret);

    if (!paymentIntent) {
      throw new Error("No payment intent found");
    }

    console.log("Payment Intent:", paymentIntent);
    return paymentIntent;
  };

  useEffect(() => {
    const initializePayment = async () => {
      try {
        // Get encoded order data
        const searchParams = new URLSearchParams(window.location.search);
        const encodedData = searchParams.get("data");
        if (!encodedData) {
          throw new Error("No order data found");
        }

        // Decode order data
        const decodedData = JSON.parse(atob(encodedData));
        setOrder(decodedData);

        // Get payment intent
        const clientSecret = searchParams.get("payment_intent_client_secret");
        if (!clientSecret) {
          throw new Error("Client secret not found in URL");
        }

        const paymentIntent = await retrievePaymentIntent(clientSecret);
        setStatus(paymentIntent.status as PaymentStatus);
        setIntentId(paymentIntent.id);
      } catch (error) {
        const errorMessage =
          error instanceof Error
            ? error.message
            : "An unexpected error occurred";
        setError(errorMessage);
        console.error("Error initializing payment:", error);
      } finally {
        setLoading(false);
      }
    };

    initializePayment();
  }, [stripe]);

  return { loading, error, order, status, intentId };
}
