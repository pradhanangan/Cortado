import {
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Divider,
  Stack,
  Typography,
} from "@mui/material";

import { Order, OrderRequest } from "@/types/orders-module";

interface OrderSummaryProps {
  order: Order;
}

export default function OrderSummary({ order }: OrderSummaryProps) {
  return (
    <Card
      sx={{ minWidth: 342, mx: "auto", borderRadius: 0 }}
      elevation={0}
      variant="outlined"
    >
      <CardHeader title={<Typography variant="h6">Order Summary</Typography>} />
      {/* <Divider variant="middle" /> */}
      <CardContent>
        {order?.orderItems.map((item, index) => (
          <Stack
            direction="row"
            justifyContent="space-between"
            spacing={1}
            key={index}
          >
            <Typography sx={{ color: "text.secondary" }}>
              {item.name}
            </Typography>
            <Stack
              direction={"row"}
              sx={{ flex: 1, justifyContent: "flex-end" }}
            >
              <Typography
                sx={{
                  color: "text.secondary",
                  width: "24px",
                  textAlign: "right",
                  // border: "1px solid #ccc",
                }}
              >
                {item.quantity || 0}
              </Typography>
              <Typography sx={{ color: "text.secondary", mx: 0.5 }}>
                @
              </Typography>
              <Typography
                sx={{
                  color: "text.secondary",
                  minWidth: "50px",
                  textAlign: "right",
                }}
              >
                ${item.unitPrice.toFixed(2) || 0}
              </Typography>
            </Stack>
            <Typography
              sx={{
                color: "text.secondary",
                minWidth: "70px",
                textAlign: "right",
              }}
            >
              $
              {(parseInt(item.quantity || "0", 10) * item.unitPrice).toFixed(
                2
              ) || 0}
            </Typography>
          </Stack>
        ))}
      </CardContent>
      <Divider variant="middle" />
      <CardActions>
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="center"
          width={"100%"}
        >
          <Typography variant="h6" component="div">
            Total
          </Typography>
          <Typography variant="h6" component="div">
            ${order?.totalPrice.toFixed(2)}
          </Typography>
        </Stack>
      </CardActions>
    </Card>
  );
}
