"use client";
import { useState } from "react";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Divider,
  TextField,
  Typography,
} from "@mui/material";
import Grid from "@mui/material/Grid2";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import { amber } from "@mui/material/colors";

import { OrderService } from "@/services/order-service";
import { OrderDto, OrderItemDto } from "@/types/orders-module";

interface SimpleCheckoutProps {
  productId: string;
  orderItems: OrderItemDto[];
  subtotal: number;
  serviceFee: number;
  total: number;
  onBack: () => void;
}

export function SimpleCheckout({
  productId,
  orderItems,
  subtotal,
  serviceFee,
  total,
  onBack,
}: SimpleCheckoutProps) {
  const [errorMsg, setErrorMsg] = useState<string>("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isComplete, setIsComplete] = useState(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    setErrorMsg("");
    setIsSubmitting(true);
    setIsComplete(false);

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
    } catch (error) {
      setErrorMsg(
        "Something went wrong while placing your order. Please try again later."
      );
      setIsSubmitting(false);
      setIsComplete(false);
      return;
    }

    setErrorMsg("");
    setIsSubmitting(false);
    setIsComplete(true);
  };

  if (isComplete) {
    return (
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          textAlign: "center",
          py: 3,
          gap: 2,
        }}
      >
        <Box sx={{ bgcolor: amber[100], p: 1.5, borderRadius: "50%" }}>
          <CheckCircleIcon sx={{ fontSize: 40, color: amber[800] }} />
        </Box>
        <Typography
          variant="h4"
          sx={{ fontWeight: 700, color: "text.primary" }}
        >
          Order Complete!
        </Typography>
        <Typography color="text.secondary">
          Thank you. We'll send you ticket once we process the payment. If you
          have any question, let us know.
        </Typography>
        <Card
          variant="outlined"
          sx={{
            mt: 2,
            width: "100%",
            maxWidth: "md",
            bgcolor: "grey.50",
            borderColor: "grey.200",
          }}
        >
          <CardContent sx={{ p: 3 }}>
            <Box sx={{ display: "flex", flexDirection: "column", gap: 1.5 }}>
              <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                <Typography sx={{ fontWeight: 500 }}>Order Number:</Typography>
                <Typography>
                  RST-{Math.floor(Math.random() * 1000000)}
                </Typography>
              </Box>
              {orderItems
                .filter((t) => t.quantity > 0)
                .map((ticket, index) => (
                  <Box
                    key={index}
                    sx={{ display: "flex", justifyContent: "space-between" }}
                  >
                    <Typography sx={{ fontWeight: 500 }}>
                      {ticket.name}:
                    </Typography>
                    <Typography>{ticket.quantity}</Typography>
                  </Box>
                ))}
              <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                <Typography sx={{ fontWeight: 500 }}>Total Paid:</Typography>
                <Typography>${total.toFixed(2)}</Typography>
              </Box>
            </Box>
          </CardContent>
        </Card>
        <Button
          variant="contained"
          sx={{ mt: 2 }}
          onClick={() => window.location.reload()}
          disableElevation
        >
          Return to Order
        </Button>
      </Box>
    );
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
                {subtotal > 0 && (
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
                )}
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

            <Button
              type="submit"
              variant="contained"
              fullWidth
              disabled={isSubmitting}
              disableElevation
              sx={{ mt: 1 }}
            >
              {isSubmitting ? "Processing..." : "Submit Order"}
            </Button>

            {errorMsg && <Alert severity="error">{errorMsg}</Alert>}
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
