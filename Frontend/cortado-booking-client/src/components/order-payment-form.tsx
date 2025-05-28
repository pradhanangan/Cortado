import { useState } from "react";
import {
  PaymentElement,
  useElements,
  useStripe,
} from "@stripe/react-stripe-js";
import {
  Alert,
  Backdrop,
  Box,
  Button,
  CircularProgress,
  Stack,
  Typography,
} from "@mui/material";
import { OrderDto } from "@/types/orders-module";
import OrderSummary from "./order-summary";

interface PaymentFormProps {
  order: OrderDto;
}

export default function PaymentForm({ order }: PaymentFormProps) {
  const stripe = useStripe();
  const elements = useElements();
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);

  const handlePayment = async (e: React.FormEvent) => {
    e.preventDefault();
    debugger;
    if (!stripe || !elements) return;
    setLoading(true);
    const params = new URLSearchParams({
      data: btoa(JSON.stringify(order)), // Base64 encode the data
    });
    const { error } = await stripe.confirmPayment({
      elements,
      confirmParams: {
        return_url: `http://localhost:3000/payments/complete?${params}`,
        payment_method_data: {
          billing_details: {
            address: {
              country: "NZ", // Set default country since we're hiding the field
            },
          },
        },
      },

      // redirect: "if_required",
    });

    if (error) {
      console.error(error);
      setErrors([error.message ?? "An unknown error occurred"]);
    }
    setLoading(false);
  };

  return (
    <>
      <Stack
        direction={{ xs: "column", md: "row" }}
        spacing={2}
        alignItems="flex-start"
        justifyContent="center"
      >
        <Box
          component="form"
          onSubmit={handlePayment}
          sx={{
            maxWidth: 400,
            mx: "auto",
            p: 3,
            border: "1px solid #ccc",
            borderRadius: 2,
          }}
        >
          <Typography variant="h6" gutterBottom>
            Payment Details
          </Typography>

          <Box sx={{ mb: 2 }}>
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
          </Box>
          <Button
            type="submit"
            variant="contained"
            color="primary"
            disabled={!stripe || loading}
            fullWidth
          >
            {/* {loading ? <CircularProgress size={24} /> : "Pay"} */}
            Pay
          </Button>

          {errors.length !== 0 && (
            <Box marginTop={3}>
              {errors.map((error, index) => (
                <Alert key={index} severity="error">
                  {" "}
                  {error}{" "}
                </Alert>
              ))}
            </Box>
          )}
        </Box>
        <Stack>
          <OrderSummary order={order} />
        </Stack>
      </Stack>

      <Backdrop
        sx={(theme) => ({ color: "#fff", zIndex: theme.zIndex.drawer + 1 })}
        open={loading}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
    </>
  );
}
