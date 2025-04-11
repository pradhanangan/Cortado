import { OrderDto } from "@/types/orders-module";

const useOrderUtils = () => {
  const validateOrderRequest = (order: OrderDto) => {
    const orderDate = new Date(order.orderDate);
    const now = new Date();
    const diffInMinutes = (now.getTime() - orderDate.getTime()) / 1000 / 60;

    if (diffInMinutes > 15 || order.isPaid === true) {
      return false;
    }

    return true;
  };

  const mapOrderDtoToOrderRequest = (orderDto: OrderDto) => {
    return {
      productId: orderDto.productId,
      email: orderDto.email,
      phoneNumber: orderDto.phoneNumber,
      firstName: orderDto.firstName,
      lastName: orderDto.lastName,
      isPaid: orderDto.isPaid,
      orderDate: orderDto.orderDate,
      orderItems: orderDto.orderItems.map((item) => ({
        productItemId: item.id,
        quantity: item.quantity,
      })),
      paymentId: "",
    };
  };
  return { validateOrderRequest, mapOrderDtoToOrderRequest };
};

export default useOrderUtils;
