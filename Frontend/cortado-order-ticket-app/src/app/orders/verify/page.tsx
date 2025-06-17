"use client";
import { useEffect, useState } from "react";
import { Alert, Box, Typography } from "@mui/material";

import { OrderService } from "@/services/order-service";

type VerificationStatus = "loading" | "success" | "error";
type OrderStatus =
  | "Verified"
  | "Already verified"
  | "Token expired"
  | "Not found";

interface VerificationState {
  status: VerificationStatus;
  message: string;
}

const STATUS_MESSAGES: Record<OrderStatus, string> = {
  Verified: "Order verified successfully.",
  "Already verified": "Order has already been verified.",
  "Token expired": "Order verification link has expired.",
  "Not found": "Invalid Order verification link.",
};

export default function OrdersVerifyPage() {
  const [state, setState] = useState<VerificationState>({
    status: "loading",
    message: "",
  });

  useEffect(() => {
    const verifyOrder = async () => {
      const searchParams = new URLSearchParams(window.location.search);
      const token = searchParams.get("token");

      if (!token) {
        setState({
          status: "error",
          message: "No token provided in the URL.",
        });
        return;
      }

      try {
        const status = await OrderService.verifyOrder(token);
        setState({
          status: status === "Verified" ? "success" : "error",
          message:
            STATUS_MESSAGES[status as OrderStatus] || "Failed to verify Order",
        });
      } catch (error) {
        setState({
          status: "error",
          message: "Failed to verify order. Please try again.",
        });
      }
    };

    verifyOrder();
  }, []);

  if (state.status === "loading") {
    return (
      <Box marginX={3}>
        <Typography>Loading...</Typography>
      </Box>
    );
  }

  return (
    <Box marginX={3}>
      <Alert severity={state.status === "success" ? "success" : "error"}>
        {state.message}
      </Alert>
    </Box>
  );
}
