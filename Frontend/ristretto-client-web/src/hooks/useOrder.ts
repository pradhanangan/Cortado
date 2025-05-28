import {
  OrderDto,
  OrderRequest,
  OrderWithPaymentRequest,
  OrderItemRequest,
} from "@/types/orders-module";

export function useOrder() {
  const createOrder = async (orderDto: OrderDto) => {
    const orderItemsRequest: OrderItemRequest[] = orderDto.orderItems.map(
      (item) => ({
        productItemId: item.id,
        quantity: item.quantity,
      })
    );

    const orderRequest: OrderRequest = {
      productId: orderDto.productId,
      email: orderDto.email,
      phoneNumber: orderDto.phoneNumber,
      firstName: orderDto.firstName,
      lastName: orderDto.lastName,
      orderDate: orderDto.orderDate,
      orderItems: orderItemsRequest,
    };

    const response = await fetch("https://localhost:7159/api/orders", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(orderRequest),
    });

    if (!response.ok) {
      debugger;
      var err = await response.json();
      throw new Error("Failed to create order");
    }

    const orderId = await response.json();
    return orderId;
  };

  const createOrderWithPayment = async (
    paymentIntentId: string,
    orderDto: OrderDto
  ) => {
    const orderItemsRequest: OrderItemRequest[] = orderDto.orderItems.map(
      (item) => ({
        productItemId: item.id,
        quantity: item.quantity,
      })
    );

    const orderWithPaymentRequest: OrderWithPaymentRequest = {
      productId: orderDto.productId,
      email: orderDto.email,
      phoneNumber: orderDto.phoneNumber,
      firstName: orderDto.firstName,
      lastName: orderDto.lastName,
      orderDate: orderDto.orderDate,
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
        body: JSON.stringify(orderWithPaymentRequest),
      }
    );

    if (!response.ok) {
      throw new Error("Failed to create order");
    }

    const orderId = await response.json();
    return orderId;
  };

  const markAsPaid = async (orderId: string, paymentIntentId: string) => {
    const markAsPaidRequest = {
      paymentId: paymentIntentId,
    };

    const response = await fetch(
      `https://localhost:7159/api/orders/${orderId}/mark-as-paid`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(markAsPaidRequest),
      }
    );

    if (!response.ok) {
      throw new Error("Failed to create order");
    }
  };

  return { createOrder, createOrderWithPayment, markAsPaid };
}
