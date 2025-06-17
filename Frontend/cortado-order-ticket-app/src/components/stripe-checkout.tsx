"use client";
import { useEffect, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Divider,
  Typography,
  TextField,
} from "@mui/material";
import Grid from "@mui/material/Grid2";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";

import { useStripePayment } from "@/hooks/useStripePayment";
import {
  Elements,
  PaymentElement,
  useElements,
  useStripe,
} from "@stripe/react-stripe-js";
import { loadStripe } from "@stripe/stripe-js";
import { OrderDto, OrderItemDto } from "@/types/order-type";
import { OrderService } from "@/services/order-service";
import { encryptTimestamp } from "@/utils/date-utils";

const stripePromise = loadStripe(
  process.env.NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY!
);

interface SimpleCheckoutProps {
  productId: string;
  orderItems: OrderItemDto[];
  subtotal: number;
  serviceFee: number;
  total: number;
  onBack: () => void;
}

export function CheckoutForm({
  productId,
  orderItems,
  subtotal,
  serviceFee,
  total,
  onBack,
}: SimpleCheckoutProps) {
  const stripe = useStripe();
  const elements = useElements();

  const [errorMsg, setErrorMsg] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!stripe || !elements) {
      console.error("Stripe.js hasn't loaded yet");
      return;
    }

    setErrorMsg("");
    setIsSubmitting(true);

    const formData = new FormData(e.currentTarget);
    const formValues = Object.fromEntries(formData.entries());

    const order: OrderDto = {
      productId: productId,
      email: formValues.email as string,
      phoneNumber: formValues.phone as string,
      firstName: formValues.firstName as string,
      lastName: formValues.lastName as string,
      orderDate: new Date(),
      orderItems: orderItems.filter((t) => t.quantity > 0),
    };

    try {
      const orderId = await OrderService.createOrder(order);
      if (!orderId) {
        setErrorMsg("Failed to create order. Please try again.");
        setIsSubmitting(false);
        return;
      }

      const returnUrl = encodeURIComponent(window.location.href);
      const timestamp = encryptTimestamp(Date.now());
      const { error } = await stripe.confirmPayment({
        elements,
        confirmParams: {
          return_url: `http://localhost:3000/orders/payments/complete?oid=${orderId}&bak=${returnUrl}&ts=${timestamp}`,
          payment_method_data: {
            billing_details: {
              email: formValues.email as string,
              name: `${formValues.firstName} ${formValues.lastName}`,
              phone: formValues.phone as string,
              address: {
                country: "NZ",
              },
            },
          },
        },
        // redirect: "if_required",
      });
      if (error) {
        console.error(error);
        setIsSubmitting(false);
      }
      // if (paymentIntent && paymentIntent.status === "succeeded") {
      //   const result = await stripe.retrievePaymentIntent(
      //     paymentIntent.client_secret!
      //   );
      //   console.log("retrievePaymentIntent result:", {
      //     hasError: !!result.error,
      //     hasPaymentIntent: !!result.paymentIntent,
      //     status: result.paymentIntent?.status,
      //   });
      //   // Redirect to success page or show success message
      //   window.location.href = `/orders/payments/complete?orderId=${orderId}&productId=${productId}`;
      // } else {
      //   setErrorMsg("Payment failed. Please try again.");
      //   setIsSubmitting(false);
      // }
    } catch (error) {
      setErrorMsg(
        "Something went wrong while placing your order. Please try again later."
      );
      setIsSubmitting(false);
    }
  };

  if (!stripe || !elements) {
    return <Typography>Loading...</Typography>;
  }

  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3 }}>
      <Button
        startIcon={<ArrowBackIcon />}
        onClick={onBack}
        sx={{ alignSelf: "flex-start", color: "text.secondary" }}
      >
        Back to Tickets
      </Button>

      <Card variant="outlined">
        <CardContent sx={{ p: 3 }}>
          <Box
            component="form"
            onSubmit={handleSubmit}
            sx={{ display: "flex", flexDirection: "column", gap: 3 }}
          >
            <Box>
              <Typography variant="h6" sx={{ mb: 2 }}>
                Order Summary
              </Typography>
              <Card variant="outlined" sx={{ bgcolor: "grey.50", mb: 2 }}>
                <CardContent>
                  <Box
                    sx={{
                      display: "flex",
                      flexDirection: "column",
                      gap: 1,
                    }}
                  >
                    {orderItems
                      .filter((t) => t.quantity > 0)
                      .map((ticket, index) => (
                        <Box
                          key={index}
                          sx={{
                            display: "flex",
                            justifyContent: "space-between",
                          }}
                        >
                          <Typography variant="body2">
                            {ticket.name} x {ticket.quantity}
                          </Typography>
                          <Typography variant="body2">
                            {ticket.unitPrice === 0
                              ? "Free"
                              : `$${(
                                  ticket.unitPrice * ticket.quantity
                                ).toFixed(2)}`}
                          </Typography>
                        </Box>
                      ))}
                  </Box>
                </CardContent>
              </Card>

              <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
                {/* {subtotal > 0 && (
                  <Box
                    sx={{
                      display: "flex",
                      justifyContent: "space-between",
                    }}
                  >
                    <Typography variant="body2" color="text.secondary">
                      Service Fee
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      ${serviceFee.toFixed(2)}
                    </Typography>
                  </Box>
                )} */}
                <Divider />
                <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                  <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                    Total
                  </Typography>
                  <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                    ${total.toFixed(2)}
                  </Typography>
                </Box>
              </Box>
            </Box>

            <Grid container spacing={3}>
              <Grid size={{ xs: 12 }}>
                <TextField
                  required
                  id="email"
                  name="email"
                  type="email"
                  placeholder="john@doe.com"
                  label="Email"
                  size="small"
                  fullWidth
                />
              </Grid>
              <Grid size={{ xs: 12 }}>
                <TextField
                  required
                  id="phone"
                  name="phone"
                  type="text"
                  placeholder="012 345 6789"
                  label="Phone number"
                  size="small"
                  fullWidth
                />
              </Grid>
              <Grid size={{ xs: 12, md: 6 }}>
                <TextField
                  required
                  id="firstName"
                  name="firstName"
                  type="text"
                  placeholder="John"
                  label="First name"
                  size="small"
                  fullWidth
                />
              </Grid>
              <Grid size={{ xs: 12, md: 6 }}>
                <TextField
                  required
                  id="lastName"
                  name="lastName"
                  type="text"
                  placeholder="Doe"
                  label="Last name"
                  size="small"
                  fullWidth
                />
              </Grid>
            </Grid>

            <PaymentElement
              options={{
                fields: {
                  billingDetails: {
                    address: {
                      country: "never",
                    },
                  },
                },
              }}
            />

            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={isSubmitting}
              disableElevation
              sx={{ mt: 1 }}
            >
              {isSubmitting
                ? "Processing..."
                : total > 0
                ? `Pay $${total.toFixed(2)}`
                : "Register for Free"}
            </Button>

            <Typography variant="caption" color="text.secondary" align="center">
              All sales are final. No refunds or exchanges.
            </Typography>

            {errorMsg && <Alert severity="error">{errorMsg}</Alert>}
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}

export function StripeCheckout(props: SimpleCheckoutProps) {
  const [clientSecret, setClientSecret] = useState("");
  const { createPaymentIntent } = useStripePayment();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let mounted = true;

    const initializePayment = async () => {
      try {
        setIsLoading(true);
        const amountInCents = Math.round(props.total * 100);
        const secret = await createPaymentIntent(amountInCents);
        if (mounted) {
          setClientSecret(secret);
        }
      } catch (error) {
        console.error("Error creating payment intent:", error);
      } finally {
        if (mounted) {
          setIsLoading(false);
        }
      }
    };

    initializePayment();

    return () => {
      mounted = false;
    };
  }, [props.total]); // Remove createPaymentIntent from dependencies

  if (isLoading || !clientSecret) {
    return <Typography>Loading payment form...</Typography>;
  }

  return (
    <Elements
      key={clientSecret}
      stripe={stripePromise}
      options={{
        clientSecret,
        appearance: { theme: "stripe" },
      }}
    >
      <CheckoutForm {...props} />
    </Elements>
  );
}
