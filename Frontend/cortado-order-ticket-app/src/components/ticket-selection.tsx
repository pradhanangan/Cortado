"use client";

import { useState } from "react";

import {
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  Divider,
  TextField,
  Typography,
} from "@mui/material";

import IconButton from "@mui/material/IconButton";
import AddIcon from "@mui/icons-material/Add";
import RemoveIcon from "@mui/icons-material/Remove";

import { Product } from "@/types/products-module";
import { StripeCheckout } from "./stripe-checkout";
import { SimpleCheckout } from "./simple-checkout";
import { OrderItemDto } from "@/types/orders-module";

interface TicketSelectionProps {
  product: Product | null;
}

export default function TicketSelection({ product }: TicketSelectionProps) {
  const [quantities, setQuantities] = useState<Record<string, number>>({});
  const [showCheckout, setShowCheckout] = useState(false);

  if (product && product.productItems.length === 0) {
    return (
      <Box>
        <Typography variant="h6">Product has no items</Typography>
      </Box>
    );
  }

  const handleQuantityChange = (id: string, value: number) => {
    setQuantities((prev) => ({
      ...prev,
      [id]: value,
    }));
  };

  const calculateTotals = () => {
    let subtotal = 0;
    let totalTickets = 0;

    product?.productItems.forEach((option) => {
      const quantity = quantities[option.id] || 0;
      subtotal += option.unitPrice * quantity;
      totalTickets += quantity;
    });

    // const serviceFee = subtotal > 0 ? subtotal * 0.15 : 0;
    const serviceFee = 0;
    const total = subtotal + serviceFee;

    return { subtotal, serviceFee, total, totalTickets };
  };

  const { subtotal, serviceFee, total, totalTickets } = calculateTotals();

  const handleContinue = () => {
    if (totalTickets > 0) {
      setShowCheckout(true);
    }
  };

  if (showCheckout) {
    const orderItemsDto: OrderItemDto[] =
      product?.productItems.map((option) => ({
        id: option.id,
        name: option.name,
        quantity: quantities[option.id] || 0,
        unitPrice: option.unitPrice,
      })) || [];

    if (process.env.NEXT_PUBLIC_ENABLE_STRIPE_PAYMENT === "true" && total > 0) {
      return (
        <StripeCheckout
          productId={product?.id || ""}
          orderItems={orderItemsDto}
          subtotal={subtotal}
          serviceFee={serviceFee}
          total={total}
          onBack={() => setShowCheckout(false)}
        />
      );
    }
    return (
      <SimpleCheckout
        productId={product?.id || ""}
        orderItems={orderItemsDto}
        subtotal={subtotal}
        serviceFee={serviceFee}
        total={total}
        onBack={() => setShowCheckout(false)}
      />
    );
  }
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 3, mt: 3 }}>
      <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <Box>
          <Typography variant="h6">Get Your Tickets</Typography>
        </Box>

        {product?.productItems.map((item, index) => (
          <Card key={item.id} variant="outlined" sx={{ width: "100%" }}>
            <CardContent sx={{ p: 2, "&:last-child": { pb: 2 } }}>
              <Box
                sx={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "center",
                }}
              >
                <Box>
                  <Typography
                    variant="subtitle1"
                    sx={{ fontWeight: 600, color: "text.primary" }}
                  >
                    {item.name}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {item.description}
                  </Typography>
                  <Typography
                    variant="body1"
                    sx={{
                      mt: 0.5,
                      fontWeight: 500,
                      color: "text.secondary",
                    }}
                  >
                    {item.isFree ? "Free" : `$${item.unitPrice.toFixed(2)}`}
                  </Typography>
                </Box>
                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                  <IconButton
                    size="small"
                    onClick={() =>
                      handleQuantityChange(
                        item.id,
                        Math.max(0, (quantities[item.id] || 0) - 1)
                      )
                    }
                    disabled={(quantities[item.id] || 0) <= 0}
                    sx={{ border: 1, borderColor: "grey.300" }}
                  >
                    <RemoveIcon fontSize="small" />
                  </IconButton>
                  <TextField
                    size="small"
                    type="number"
                    inputProps={{
                      min: 0,
                      max: 10,
                      style: { textAlign: "center" },
                    }}
                    value={quantities[item.id] || 0}
                    onChange={(e) =>
                      handleQuantityChange(
                        item.id,
                        Number.parseInt(e.target.value) || 0
                      )
                    }
                    sx={{ width: 60 }}
                  />
                  <IconButton
                    size="small"
                    onClick={() =>
                      handleQuantityChange(
                        item.id,
                        Math.min(10, (quantities[item.id] || 0) + 1)
                      )
                    }
                    disabled={(quantities[item.id] || 0) >= 10}
                    sx={{ border: 1, borderColor: "grey.300" }}
                  >
                    <AddIcon fontSize="small" />
                  </IconButton>
                </Box>
              </Box>
            </CardContent>
          </Card>
        ))}

        {totalTickets > 0 && (
          <Card variant="outlined">
            <CardContent sx={{ pt: 3 }}>
              <Box sx={{ display: "flex", flexDirection: "column", gap: 1 }}>
                {product?.productItems.map((option, index) => {
                  const quantity = quantities[option.id] || 0;
                  if (quantity === 0) return null;

                  return (
                    <Box
                      key={option.id}
                      sx={{
                        display: "flex",
                        justifyContent: "space-between",
                      }}
                    >
                      <Typography variant="body2">
                        {option.name} x {quantity}
                      </Typography>
                      <Typography variant="body2">
                        {option.isFree
                          ? "Free"
                          : `$${(option.unitPrice * quantity).toFixed(2)}`}
                      </Typography>
                    </Box>
                  );
                })}

                {/* {subtotal > 0 && (
                  <>
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
                  </>
                )} */}
                <Divider sx={{ my: 1 }} />
                <Box
                  sx={{
                    display: "flex",
                    justifyContent: "space-between",
                  }}
                >
                  <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                    Total
                  </Typography>
                  <Typography variant="subtitle1" sx={{ fontWeight: 700 }}>
                    ${total.toFixed(2)}
                  </Typography>
                </Box>
              </Box>
            </CardContent>
            <CardActions sx={{ p: 2, pt: 0 }}>
              <Button
                variant="contained"
                fullWidth
                onClick={handleContinue}
                disableElevation
              >
                Continue to Checkout
              </Button>
            </CardActions>
          </Card>
        )}
      </Box>
    </Box>
  );
}
