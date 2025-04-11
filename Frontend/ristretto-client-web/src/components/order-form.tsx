"use client";
import ReCAPTCHA from "react-google-recaptcha";
import {
  Alert,
  Backdrop,
  Box,
  Button,
  Card,
  CardActions,
  CardContent,
  CardHeader,
  Divider,
  Stack,
  TextField,
  Typography,
} from "@mui/material";

import Grid from "@mui/material/Grid2";
import CircularProgress from "@mui/material/CircularProgress";
import { useEffect, useRef, useState } from "react";

import { Product } from "@/types/products-module";

import { useRouter } from "next/navigation";
import { OrderDto, OrderRequest } from "@/types/orders-module";
import useOrderUtils from "@/utils/order-utils";

interface OrderFormProps {
  product: Product | null;
}

export default function OrderForm({ product }: OrderFormProps) {
  const [openBackdrop, setOpenBackdrop] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<string[]>([]);
  const [success, setSuccess] = useState(false);
  const [totalTickets, setTotalTickets] = useState(0);
  const { mapOrderDtoToOrderRequest } = useOrderUtils();

  const getGridMdValue = (length: number) => {
    if (length === 1 || length === 2) return 6;
    return 4;
  };

  const formRef = useRef<HTMLFormElement>(null);
  const productItemRefs = useRef<(HTMLInputElement | null)[]>([]);
  const recaptchaRef = useRef<ReCAPTCHA | null>(null);
  const router = useRouter();

  if (product && product.productItems.length === 0) {
    return (
      <Box>
        <Typography variant="h6">Product has no items</Typography>
      </Box>
    );
  }

  const handleProductItemChange = () => {
    const total = productItemRefs.current.reduce((acc, ref, index) => {
      const value = parseInt(ref?.value || "0", 10);
      const price = product?.productItems[index].unitPrice || 0;
      return acc + value * price;
    }, 0);
    setTotalTickets(total);
  };

  const createOrderWithoutPayment = async (order: OrderDto) => {
    try {
      const orderRequest = mapOrderDtoToOrderRequest(order);
      const response = await fetch("https://localhost:7159/api/orders", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(orderRequest),
      });

      if (response.ok) {
        debugger;
        const orderId = await response.json();

        if (formRef.current) {
          formRef.current.reset(); // Clear all form values
        }
        productItemRefs.current = [];
        // recaptchaRef.current.reset();
        setSuccess(true);
        router.push(`/payments?orderId=${orderId}&&amount=${totalTickets}`);
      }
      if (!response.ok) {
        throw new Error("Failed to submit booking");
      }
    } catch (error) {
      console.error("Error:", error);
      setErrors((prev) => [
        ...prev,
        "An unexpected error occurred. Please try again later.",
      ]);
    } finally {
      setIsSubmitting(false);
      setOpenBackdrop(false);
    }
  };

  const goToPaymentPage = (order: OrderDto) => {
    const params = new URLSearchParams({
      data: btoa(JSON.stringify(order)), // Base64 encode the data
    });

    router.push(`/payments?${params.toString()}`);
    setIsSubmitting(false);
    setOpenBackdrop(false);
  };

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setIsSubmitting(true);
    setOpenBackdrop(true);

    setSuccess(false);
    setErrors([]);

    const formData = new FormData(event.currentTarget);
    const formValues = Object.fromEntries(formData.entries());

    const orderItems = product?.productItems
      .map((item) => {
        const value = parseInt(formValues[item.id]?.toString() ?? "0") || 0;
        return {
          id: item.id,
          name: item.name,
          unitPrice: item.unitPrice,
          quantity: value,
        };
      })
      .filter((item) => item.quantity > 0);

    if (!orderItems || orderItems.length === 0) {
      setErrors((prev) => [
        ...prev,
        "Please enter the number of tickets for at least one item.",
      ]);

      if (productItemRefs.current.length > 0) {
        const firstInvalidProductItem = productItemRefs.current.find(
          (ref) => ref && (ref.value === "0" || ref.value === "")
        );
        if (firstInvalidProductItem) {
          firstInvalidProductItem.focus();
        }
      }
      setIsSubmitting(false);
      setOpenBackdrop(false);
      return;
    }

    // const captchaValue = recaptchaRef.current.getValue();
    // if (!captchaValue) {
    //   setErrors(["Please complete the reCAPTCHA to proceed."]);
    //   setIsSubmitting(false);
    //   setOpenBackdrop(false);
    //   return;
    // }

    debugger;
    const order: OrderDto = {
      productId: formValues.productId as string,
      email: formValues.email as string,
      phoneNumber: formValues.phone as string,
      firstName: formValues.firstName as string,
      lastName: formValues.lastName as string,
      orderDate: new Date(),
      orderItems: orderItems,
      totalPrice: totalTickets,
      isPaid: false,
    };

    if (process.env.NEXT_PUBLIC_ENABLE_STRIPE_PAYMENT === "true") {
      console.log(
        "TRUE - ENABLE_STRIPE_PAYMENT:",
        process.env.NEXT_PUBLIC_ENABLE_STRIPE_PAYMENT
      );
    } else {
      console.log(
        "FALSE - ENABLE_STRIPE_PAYMENT:",
        process.env.NEXT_PUBLIC_ENABLE_STRIPE_PAYMENT
      );
      goToPaymentPage(order);
      return;
    }
  }

  return (
    <>
      <form onSubmit={handleSubmit} ref={formRef}>
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 9 }}>
            <Grid container spacing={3}>
              <Box width={"100%"}>
                <Typography variant="h6">Personal Details</Typography>
              </Box>
              <input
                type="hidden"
                id="productId"
                name="productId"
                value={product?.id}
              />
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
                  id="lastName"
                  name="lastName"
                  type="text"
                  placeholder="Doe"
                  label="Last name"
                  size="small"
                  fullWidth
                />
              </Grid>
              <Box width={"100%"}>
                <Typography variant="h6">Number of Tickets *</Typography>
              </Box>
              {product?.productItems.map((item, index) => (
                <Grid
                  size={{
                    xs: 12,
                    md: getGridMdValue(product.productItems.length),
                  }}
                  key={index}
                >
                  <TextField
                    id={item.id}
                    name={item.id}
                    type="number"
                    placeholder="0"
                    label={item.variants}
                    size="small"
                    fullWidth
                    inputRef={(el) => (productItemRefs.current[index] = el)}
                    onChange={handleProductItemChange}
                  />
                </Grid>
              ))}

              {/* <ReCAPTCHA
                ref={recaptchaRef}
                sitekey={process.env.NEXT_PUBLIC_SITE_KEY}
              /> */}
            </Grid>
          </Grid>

          <Grid size={{ xs: 12, sm: 3 }}>
            <Card sx={{ minWidth: 275 }}>
              <CardHeader
                title={<Typography variant="h6">Order Summary</Typography>}
              />
              <Divider variant="middle" />
              <CardContent>
                {product?.productItems.map((item, index) => (
                  <Stack
                    direction="row"
                    justifyContent="space-between"
                    spacing={1}
                    key={index}
                  >
                    <Typography sx={{ color: "text.secondary" }}>
                      {item.variants}
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
                        {productItemRefs.current[index]?.value || 0}
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
                      {(
                        parseInt(
                          productItemRefs.current[index]?.value || "0",
                          10
                        ) * item.unitPrice
                      ).toFixed(2) || 0}
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
                    ${totalTickets.toFixed(2)}
                  </Typography>
                </Stack>
              </CardActions>
            </Card>
          </Grid>
          <Grid size={{ xs: 12 }}>
            <Button
              type="submit"
              variant="contained"
              color="primary"
              disabled={isSubmitting}
            >
              Continue to Pay
            </Button>
          </Grid>
        </Grid>
      </form>

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
      {/* {success && (
        <Box marginTop={3}>
          <Alert severity="success">
            Thank you. We'll send you ticket once we process the payment. If you
            have any question, let us know.
          </Alert>
        </Box>
      )} */}
      <Backdrop
        sx={(theme) => ({ color: "#fff", zIndex: theme.zIndex.drawer + 1 })}
        open={openBackdrop}
      >
        <CircularProgress color="inherit" />
      </Backdrop>
    </>
  );
}
