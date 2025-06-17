import { useEffect, useState } from "react";
import { OrderService } from "@/services/order-service";
import { PaymentStatus } from "@/types/orders-module";
import { decryptTimestamp } from "@/utils/date-utils";

interface PaymentCompleteParams {
  orderId: string | null;
  paymentIntentId: string | null;
  clientSecret: string | null;
  redirectStatus: string | null;
  returnUrl: string | null;
  encryptedTs: string | null;
}

interface PaymentCompleteState {
  loading: boolean;
  errorMsg: string;
  status: PaymentStatus;
  returnUrl: string;
}

const PAYMENT_LINK_EXPIRY = 20 * 60 * 1000; // 20 minutes in milliseconds

export function usePaymentComplete() {
  const [state, setState] = useState<PaymentCompleteState>({
    loading: true,
    errorMsg: "",
    status: "default",
    returnUrl: "",
  });

  useEffect(() => {
    const getUrlParams = (): PaymentCompleteParams => {
      const searchParams = new URLSearchParams(window.location.search);
      const params = {
        orderId: searchParams.get("oid"),
        paymentIntentId: searchParams.get("payment_intent"),
        clientSecret: searchParams.get("payment_intent_client_secret"),
        redirectStatus: searchParams.get("redirect_status") as PaymentStatus,
        returnUrl: searchParams.get("bak"),
        encryptedTs: searchParams.get("ts"),
      };

      // Validate all params exist
      if (Object.values(params).some((value) => !value)) {
        throw new Error("Missing required parameters in URL");
      }

      return params as PaymentCompleteParams;
    };

    const handlePaymentComplete = async () => {
      try {
        // Get URL parameters
        const params = getUrlParams();

        // Validate timestamp
        const timestamp = decryptTimestamp(params.encryptedTs!);
        if (Date.now() - timestamp > PAYMENT_LINK_EXPIRY) {
          throw new Error("Payment link has expired");
        }

        if (params.redirectStatus === "succeeded") {
          try {
            await OrderService.markOrderAsPaid(
              params.orderId!,
              params.paymentIntentId!
            );
          } catch (error) {
            // Log error but do not set error message to user
            console.error("Error marking order as paid:", error);
            // ToDo: Send to error tracking service / notify administrator
            // captureException(error);
          }
        }
        setState({
          loading: false,
          errorMsg: "",
          status: params.redirectStatus as PaymentStatus,
          returnUrl: params.returnUrl!,
        });
      } catch (error) {
        setState((prev) => ({
          ...prev,
          loading: false,
          errorMsg:
            error instanceof Error
              ? error.message
              : "An unexpected error occurred",
        }));
      }
    };

    handlePaymentComplete();
  }, []);

  return state;
}
