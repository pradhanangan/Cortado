"use client";
import {
  Backdrop,
  Box,
  Button,
  CircularProgress,
  Container,
} from "@mui/material";

import OrderPaymentComplete from "@/components/order-payment-complete";
import OrderError from "@/components/order-error";

import { usePaymentComplete } from "@/hooks/usePaymentComplete";

function PaymentCompleteContent() {
  const { loading, errorMsg, status, returnUrl } = usePaymentComplete();

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

  if (errorMsg) {
    return <OrderError message={errorMsg} />;
  }

  return (
    <Container>
      <OrderPaymentComplete status={status} />
      <Box sx={{ display: "flex", justifyContent: "center", mt: 2 }}>
        <Button
          variant="contained"
          color="primary"
          onClick={() => window.location.replace(decodeURIComponent(returnUrl))}
        >
          New Order
        </Button>
      </Box>
    </Container>
  );
}

export default function CompletePage() {
  return (
    <Container>
      <PaymentCompleteContent />
    </Container>
  );
}
