"use client";
import { useEffect, useState } from "react";
import { useSearchParams } from "next/navigation";
import { Box, Container } from "@mui/material";

import ProductDetail from "@/components/product-detail";
import TicketSelection from "@/components/ticket-selection";
import OrderError from "@/components/order-error";
import { ProductService } from "@/services/product-service";
import { Product } from "@/types/products-module";
import { PRODUCT_ERRORS } from "@/constants/product-constant";

const ERROR_MESSAGES = {
  TOKEN_MISSING:
    "Something's missing in the link. It may have been broken or copied incorrectly.",
  TOKEN_EXPIRED: "Looks like this link has expired or is no longer valid.",
  DEFAULT: "Oops! Something went wrong. Please try again.",
};

export default function OrdersPage() {
  const searchParams = useSearchParams();
  const [product, setProduct] = useState<Product | null>(null);
  const [errorMsg, setErrorMsg] = useState("");

  const token = searchParams.get("et") || "";

  useEffect(() => {
    if (!token) {
      setErrorMsg(ERROR_MESSAGES.TOKEN_MISSING);
      return;
    }

    const fetchProduct = async () => {
      try {
        const data = await ProductService.getProductByToken(token);
        setProduct(data);
        setErrorMsg("");
      } catch (error) {
        console.error(error);
        setErrorMsg(
          error instanceof Error &&
            error.message === PRODUCT_ERRORS.TOKEN_EXPIRED
            ? ERROR_MESSAGES.TOKEN_EXPIRED
            : ERROR_MESSAGES.DEFAULT
        );
      }
    };
    fetchProduct();
  }, [token]);

  return (
    <Box>
      {product && (
        <Container>
          <ProductDetail product={product} />
          <TicketSelection product={product} />
        </Container>
      )}
      {errorMsg && <OrderError message={errorMsg} />}
    </Box>
  );
}
