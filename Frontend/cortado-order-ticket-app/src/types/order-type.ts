export interface OrderDto {
  productId: string;
  email: string;
  phoneNumber: string;
  firstName: string;
  lastName: string;
  orderDate: Date;
  orderItems: OrderItemDto[];
}

export interface OrderItemDto {
  id: string;
  name: string;
  unitPrice: number;
  quantity: number;
  // price: number;
}

export type PaymentStatus =
  | "requires_payment_method"
  // | "requires_confirmation"
  // | "requires_action"
  | "processing"
  | "succeeded"
  | "canceled"
  | "default";

// export interface OrderDetails {
//   orderId: string;
//   orderDate: Date;
//   totalPrice: number;
//   isPaid: boolean;
//   orderItems: OrderItem[];
// }

export interface OrderItemRequest {
  productItemId: string;
  quantity: number;
}

export interface OrderRequest {
  productId: string;
  email: string;
  phoneNumber: string;
  firstName: string;
  lastName: string;
  orderDate: Date;
  orderItems: OrderItemRequest[];
}

export interface OrderWithPaymentRequest {
  productId: string;
  email: string;
  phoneNumber: string;
  firstName: string;
  lastName: string;
  orderDate: Date;
  orderItems: OrderItemRequest[];
  paymentId: string;
}
