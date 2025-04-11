"use client";

import { useState, useEffect } from "react";
import { Elements } from "@stripe/react-stripe-js";
import { loadStripe } from "@stripe/stripe-js";
import { Backdrop, CircularProgress, Container } from "@mui/material";
import { useRouter } from "next/navigation";
import useOrderUtils from "@/utils/order-utils";
import { OrderDto } from "@/types/orders-module";
import PaymentForm from "@/components/order-payment-form";
import { useStripePayment } from "@/hooks/useStripePayment";
const stripePromise = loadStripe(
  process.env.NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY!
);

export default function PaymentPage() {
  const { validateOrderRequest } = useOrderUtils();
  const [orderDto, setOrderDto] = useState<OrderDto | null>(null);
  const [clientSecret, setClientSecret] = useState("");
  const [loading, setLoading] = useState(false);
  const { createPaymentIntent } = useStripePayment();
  const router = useRouter();

  useEffect(() => {
    const initializePayment = async () => {
      try {
        const searchParams = new URLSearchParams(window.location.search);
        const encodedData = searchParams.get("data");
        if (!encodedData) {
          throw new Error("No order data found");
        }
        const decodedData = JSON.parse(atob(encodedData));

        setOrderDto(decodedData);

        if (!validateOrderRequest(decodedData)) {
          throw new Error("Invalid order data");
        }

        const amountInCents = decodedData.totalPrice * 100;
        const secret = await createPaymentIntent(amountInCents);
        setClientSecret(secret);
      } catch (error) {
        console.error("Error parsing order data:", error);
        router.push("/errors");
      } finally {
        setLoading(false);
      }
    };
    initializePayment();
  }, [router]);

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
    <>
      {clientSecret && (
        <Elements stripe={stripePromise} options={{ clientSecret }}>
          <Container>{orderDto && <PaymentForm order={orderDto} />}</Container>
        </Elements>
      )}
    </>
  );
}
