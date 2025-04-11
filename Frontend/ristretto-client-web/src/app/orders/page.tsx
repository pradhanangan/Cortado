"use client";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Box, Container } from "@mui/material";
import { Product } from "@/types/products-module";
import OrderForm from "@/components/order-form";
export default function OrdersPage() {
  const [productId, setProductId] = useState("");
  const [product, setProduct] = useState<Product | null>(null);
  const router = useRouter();

  useEffect(() => {
    const searchParams = new URLSearchParams(window.location.search);
    const productId = searchParams.get("productId") || "";
    if (!productId) {
      router.push("/errors");
    } else {
      setProductId(productId);
    }
  }, [router]);

  useEffect(() => {
    const fetchProduct = async () => {
      if (!productId) return;
      try {
        const response = await fetch(
          `https://localhost:7159/api/products/${productId}`,
          {
            method: "GET",
          }
        );
        if (response.ok) {
          const data = await response.json();
          if (data.productItems.length === 0) {
            throw new Error("Product has no items");
          }
          setProduct(data);
        } else {
          throw new Error("Failed to fetch products");
        }
      } catch (error) {
        console.error(error);
        router.push("/errors");
      }
    };
    fetchProduct();
  }, [productId]);

  if (!product) {
    return;
  }
  return (
    <Box>
      <Container>
        <OrderForm product={product} />
      </Container>
    </Box>
  );
}
