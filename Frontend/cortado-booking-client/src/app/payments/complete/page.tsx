"use client";
import React, { useEffect, useState, useRef } from "react";
import { useStripe, Elements } from "@stripe/react-stripe-js";
import { loadStripe } from "@stripe/stripe-js";
import { Backdrop, Button, CircularProgress, Container } from "@mui/material";
import { useRouter } from "next/navigation";
import { OrderDto, PaymentStatus } from "@/types/orders-module";
import OrderPaymentComplete from "@/components/order-payment-complete";
import { useOrder } from "@/hooks/useOrder";
import { useStripePayment } from "@/hooks/useStripePayment";

// Initialize Stripe outside component to prevent re-initialization
const stripePromise = loadStripe(
  process.env.NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY!
);

function PaymentCompleteContent() {
  const stripe = useStripe();
  const router = useRouter();
  const { createOrderWithPayment } = useOrder();
  const { retrievePaymentIntent } = useStripePayment();
  const [loading, setLoading] = useState(true);
  const [order, setOrder] = useState<OrderDto | null>(null);
  const [status, setStatus] = useState<PaymentStatus>("default");
  // const hasInitialized = useRef(false); // Track if the effect has already run

  useEffect(() => {
    const initializePayment = async () => {
      try {
        if (!stripe) {
          console.log("Stripe is not initialized");
          return;
        }

        const searchParams = new URLSearchParams(window.location.search);
        const encodedData = searchParams.get("data");
        const clientSecret = searchParams.get("payment_intent_client_secret");

        if (!encodedData || !clientSecret) {
          throw new Error("Missing required parameters in URL");
        }

        // Decode order data only once
        const decodedData = JSON.parse(atob(encodedData));
        setOrder(decodedData);

        const paymentIntent = await retrievePaymentIntent(stripe, clientSecret);
        if (!paymentIntent) return;

        if (paymentIntent.status === "succeeded") {
          try {
            await createOrderWithPayment(paymentIntent.id, decodedData);
            setStatus(paymentIntent.status as PaymentStatus);
          } catch (error) {
            console.error("Error creating order:", error);
            router.push("/errors");
          }
        } else {
          console.error("Payment was not successful:", paymentIntent.status);
          setStatus(paymentIntent.status as PaymentStatus);
        }
      } catch (error) {
        console.error("Error initializing payment:", error);
      } finally {
        setLoading(false);
      }
    };

    // Run the effect only once
    // if (!hasInitialized.current) {
    //   hasInitialized.current = true;
    initializePayment();
    // }
  }, [stripe, router]);

  if (loading) {
    return (
      <Backdrop
        sx={(theme) => ({ color: "#fff", zIndex: theme.zIndex.drawer + 1 })}
        open={loading}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
    );
  }

  return (
    <Container>
      <OrderPaymentComplete status={status} />
      <br />
      <Button
        variant="contained"
        color="primary"
        onClick={() =>
          window.location.replace(`/orders?productId=${order?.productId}`)
        }
      >
        New Order
      </Button>
    </Container>
  );
}

export default function CompletePage() {
  return (
    <Container>
      <Elements stripe={stripePromise}>
        <PaymentCompleteContent />
      </Elements>
    </Container>
  );
}
