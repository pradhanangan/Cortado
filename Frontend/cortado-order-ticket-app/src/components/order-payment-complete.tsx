import { Box, Container, Stack, Typography, useTheme } from "@mui/material";
import { CheckCircle, Cancel, Info } from "@mui/icons-material";
import { PaymentStatus } from "@/types/orders-module";
import React from "react";

const STATUS_CONTENT_MAP = (theme: any) => ({
  requires_payment_method: {
    text: "Payment failed",
    subtext:
      "We are sorry, there was an error processing your payment. Please try again.",
    iconColor: "#DF1B41",
    icon: (color: string) => <Cancel sx={{ color, fontSize: 40, mr: 2 }} />,
  },
  processing: {
    text: "Processing your order ...",
    subtext:
      "Hold tight, your order is being processed. We will email you when your order succeeds.",
    iconColor: "#6D6E78",
    icon: (color: string) => <Info sx={{ color, fontSize: 40, mr: 2 }} />,
  },
  succeeded: {
    text: "Payment successful",
    subtext:
      "Your order has been placed. We will send you an email with your order details.",
    iconColor: "#30B130",
    icon: (color: string) => (
      <CheckCircle sx={{ color, fontSize: 40, mr: 2 }} />
    ),
  },
  canceled: {
    text: "Your order was canceled",
    subtext:
      "Your order has been canceled. We will send you an email with the details.",
    iconColor: "#30B130",
    icon: (color: string) => (
      <CheckCircle sx={{ color, fontSize: 40, mr: 2 }} />
    ),
  },
  default: {
    text: "",
    subtext: "",
    iconColor: "",
    icon: () => null,
  },
});

interface OrderPaymentCompleteProps {
  status: PaymentStatus;
}

export default function OrderPaymentComplete({
  status,
}: OrderPaymentCompleteProps) {
  const theme = useTheme();

  const statusContent = STATUS_CONTENT_MAP(theme)[status];

  return (
    <React.Fragment>
      {status !== "default" && (
        <Container
          sx={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            pt: { xs: 14, sm: 20 },
            pb: { xs: 8, sm: 12 },
          }}
        >
          <Stack
            spacing={2}
            useFlexGap
            sx={{ alignItems: "center", width: { xs: "100%", sm: "70%" } }}
          >
            <Typography
              variant="h1"
              sx={{
                display: "flex",
                flexDirection: { xs: "column", sm: "row" },
                alignItems: "center",
                fontSize: "clamp(1rem, 2.5vw, 2rem)",
              }}
            >
              {statusContent.icon(statusContent.iconColor)}
              {statusContent.text}
            </Typography>
            <Typography
              sx={{
                textAlign: "center",
                color: "text.secondary",
                width: { sm: "100%", md: "80%" },
              }}
            >
              {statusContent.subtext}
            </Typography>
          </Stack>
        </Container>
      )}
    </React.Fragment>
  );
}
