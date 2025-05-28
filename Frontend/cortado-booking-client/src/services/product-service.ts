import { API_CONFIG } from "@/config/api-config";
import { PRODUCT_ERRORS } from "@/constants/product-constant";
import { ApiProblemDetails, ApiErrorCode } from "@/types/api";
import { Product } from "@/types/products-module";

export class ProductService {
  public static async getProductByToken(token: string): Promise<Product> {
    const response = await fetch(
      `${API_CONFIG.BASE_URL}/products/token?token=${token}`,
      {
        method: "GET",
      }
    );

    if (!response.ok) {
      const errorData: ApiProblemDetails = await response.json();
      if (
        errorData.errors?.some(
          (error) => error.code === ApiErrorCode.TokenExpired
        )
      ) {
        throw new Error(PRODUCT_ERRORS.TOKEN_EXPIRED);
      } else {
        throw new Error(PRODUCT_ERRORS.FETCH_FAILED);
      }
    }

    const succesData = await response.json();
    if (succesData.productItems.length === 0) {
      throw new Error(PRODUCT_ERRORS.NO_PRODUCT_ITEM);
    }

    return succesData;
  }
}
