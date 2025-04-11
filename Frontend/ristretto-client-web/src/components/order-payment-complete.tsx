import { Box, Typography, useTheme } from "@mui/material";
import { CheckCircle, Cancel, Info } from "@mui/icons-material";
import { PaymentStatus } from "@/types/orders-module";
import React from "react";

const STATUS_CONTENT_MAP = (theme: any) => ({
  requires_payment_method: {
    text: "Payment failed",
    subtext:
      "We are sorry, there was an error processing your payment. Please try again.",
    iconColor: "#DF1B41",
    icon: (color: string) => <Cancel sx={{ color }} />,
  },
  processing: {
    text: "Processing your order ...",
    subtext:
      "Hold tight, your order is being processed. We will email you when your order succeeds.",
    iconColor: "#6D6E78",
    icon: (color: string) => <Info sx={{ color }} />,
  },
  succeeded: {
    text: "Payment successful",
    subtext:
      "Your order has been placed. We will send you an email with your order details.",
    iconColor: "#30B130",
    icon: (color: string) => <CheckCircle sx={{ color }} />,
  },
  canceled: {
    text: "Your order was canceled",
    subtext:
      "Your order has been canceled. We will send you an email with the details.",
    iconColor: "#30B130",
    icon: (color: string) => <CheckCircle sx={{ color }} />,
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
        <Box>
          {statusContent.icon(statusContent.iconColor)}
          <Typography variant="h6">{statusContent.text}</Typography>
          <Typography variant="body2">{statusContent.subtext}</Typography>
        </Box>
      )}
    </React.Fragment>
  );
}
