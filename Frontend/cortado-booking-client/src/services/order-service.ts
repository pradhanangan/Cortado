import { API_CONFIG } from "@/config/api-config";
import { ORDER_ERRORS } from "@/constants/order-constant";
import { ApiProblemDetails } from "@/types/api-type";
import { OrderDto, OrderRequest, OrderItemRequest } from "@/types/order-type";

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
      throw new Error(ORDER_ERRORS.CREATE_ORDER_FAILED, {});
    }

    const orderId: string = await response.json();
    return orderId;
  }

  public static async createOrderWithEmail(
    orderDto: OrderDto
  ): Promise<string> {
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

    const response = await fetch(`${API_CONFIG.BASE_URL}/orders/with-email`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(orderRequest),
    });

    if (!response.ok) {
      const errorData: ApiProblemDetails = await response.json();
      console.error("Order creation failed: ", errorData);
      throw new Error(ORDER_ERRORS.CREATE_ORDER_FAILED, {});
    }

    const orderId: string = await response.json();
    return orderId;
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

    const orderId: string = await response.json();
    return orderId;
  }

  public static async markOrderAsPaid(
    orderId: string,
    paymentIntentId: string
  ): Promise<void> {
    debugger;
    const response = await fetch(
      `${API_CONFIG.BASE_URL}/orders/${orderId}/mark-as-paid`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ paymentId: paymentIntentId }),
      }
    );

    if (!response.ok) {
      const errorData: ApiProblemDetails = await response.json();
      console.error("Mark order as paid failed: ", errorData);
      throw new Error(ORDER_ERRORS.MARK_ORDER_AS_PAID_FAILED);
    }
  }

  public static async verifyOrder(token: string): Promise<string> {
    const response = await fetch(
      `${API_CONFIG.BASE_URL}/orders/verify-order?token=${token}`,
      {
        method: "GET",
      }
    );
    debugger;
    if (!response.ok) {
      const errorData: ApiProblemDetails = await response.json();
      console.error("Order verification failed: ", errorData);
      throw new Error(ORDER_ERRORS.VERIFY_ORDER_FAILED);
    }

    const data = await response.json();
    return data.status; // Assuming the API returns a status message
  }
}
