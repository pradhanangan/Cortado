import { API_CONFIG } from "@/config/api-config";
import { ORDER_ERRORS } from "@/constants/order-constant";
import { ApiProblemDetails } from "@/types/api";
import {
  OrderDto,
  OrderRequest,
  OrderItemRequest,
} from "@/types/orders-module";

export class OrderService {
  public static async createOrder(orderDto: OrderDto): Promise<string> {
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

    const response = await fetch(`${API_CONFIG.BASE_URL}/orders`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(orderRequest),
    });

    if (!response.ok) {
      const errorData: ApiProblemDetails = await response.json();
      console.error("Order creation failed: ", errorData);
      throw new Error(ORDER_ERRORS.CREATE_ORDER_FAILED);
    }

    const succesData = await response.json();
    return succesData.orderId;
  }

  public static async createOrderWithPayment(
    paymentIntentId: string,
    orderDto: OrderDto
  ): Promise<string> {
    const orderItemsRequest: OrderItemRequest[] = orderDto.orderItems.map(
      (item) => ({
        productItemId: item.id,
        quantity: item.quantity,
      })
    );

    const orderWithPaymentRequest = {
      productId: orderDto.productId,
      email: orderDto.email,
      phoneNumber: orderDto.phoneNumber,
      firstName: orderDto.firstName,
      lastName: orderDto.lastName,
      orderDate: orderDto.orderDate,
      orderItems: orderItemsRequest,
      paymentIntentId: paymentIntentId,
    };

    const response = await fetch(`${API_CONFIG.BASE_URL}/orders/with-payment`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(orderWithPaymentRequest),
    });

    if (!response.ok) {
      const errorData: ApiProblemDetails = await response.json();
      console.error("Order with payment creation failed: ", errorData);
      throw new Error(ORDER_ERRORS.CREATE_ORDER_WITH_PAYMENT_FAILED);
    }

    const succesData = await response.json();
    return succesData.orderId;
  }

  public static async markOrderAsPaid(
    orderId: string,
    paymentIntentId: string
  ): Promise<void> {
    const response = await fetch(
      `${API_CONFIG.BASE_URL}/orders/${orderId}/mark-as-paid`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ paymentIntentId }),
      }
    );

    if (!response.ok) {
      const errorData: ApiProblemDetails = await response.json();
      console.error("Mark order as paid failed: ", errorData);
      throw new Error(ORDER_ERRORS.MARK_ORDER_AS_PAID_FAILED);
    }
  }
}
