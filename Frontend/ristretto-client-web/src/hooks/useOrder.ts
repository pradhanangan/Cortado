import { useState } from "react";
import {
  OrderDto,
  OrderRequest,
  OrderItemRequest,
} from "@/types/orders-module";

export function useOrder() {
  const createOrder = async (paymentIntentId: string, orderData: OrderDto) => {
    const orderItemsRequest: OrderItemRequest[] = orderData.orderItems.map(
      (item) => ({
        productItemId: item.id,
        quantity: item.quantity,
      })
    );

    const orderRequest: OrderRequest = {
      productId: orderData.productId,
      email: orderData.email,
      phoneNumber: orderData.phoneNumber,
      firstName: orderData.firstName,
      lastName: orderData.lastName,
      isPaid: true,
      orderDate: orderData.orderDate,
      orderItems: orderItemsRequest,
      paymentId: paymentIntentId,
    };

    const response = await fetch(
      "https://localhost:7159/api/orders/with-payment",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(orderRequest),
      }
    );

    if (!response.ok) {
      throw new Error("Failed to create order");
    }

    const orderId = await response.json();
    return orderId;
  };

  return { createOrder };
}
