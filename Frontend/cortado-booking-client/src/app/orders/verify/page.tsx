"use client";
import { Alert, Box, Typography } from "@mui/material";
import { useEffect, useState } from "react";

export default function OrdersVerifyPage() {
  const [token, setToken] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  useEffect(() => {
    const searchParams = new URLSearchParams(window.location.search);
    const token = searchParams.get("token") || "";
    if (!token) {
      setMessage("No token provided in the URL.");
      setError(true);
      setLoading(false);
    } else {
      setToken(token);
    }
  }, []);

  useEffect(() => {
    const verifyOrder = async () => {
      if (token) {
        try {
          const response = await fetch(
            `https://localhost:7159/api/orders/verify-order?token=${token}`,
            {
              method: "GET",
            }
          );
          debugger;
          if (response.ok) {
            const data = await response.json();
            if (data.status === "Verified") {
              setError(false);
              setMessage("Booking verified successfully.");
            } else if (data.status === "Already verified") {
              setError(true);
              setMessage("Booking has already been verified.");
            } else if (data.status === "Token expired") {
              setError(true);
              setMessage("Booking verification link has expired.");
            } else if (data.status === "Not found") {
              setError(true);
              setMessage("Invalid booking verification link.");
            } else {
              setError(true);
              throw new Error("Failed to verify booking");
            }
          } else {
            setError(true);
            const errorData = await response.json();
            throw new Error("Failed to verify order");
          }
        } catch (error) {
          setError(true);
          setMessage("Failed to verify order. Please try again.");
        } finally {
          setLoading(false);
        }
      }
    };
    verifyOrder();
    debugger;
  }, [token]);

  if (loading) {
    return (
      <Box marginX={3}>
        <Typography>Loading...</Typography>
      </Box>
    );
  }

  return (
    <Box marginX={3}>
      {/* <Typography>{message}</Typography> */}
      {error && <Alert severity="error">{message}</Alert>}
      {!error && <Alert severity="success">{message}</Alert>}
    </Box>
  );
}
