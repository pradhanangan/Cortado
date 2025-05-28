export interface Product {
  id: string;
  code: string;
  name: string;
  description: string;
  productItems: ProductItem[];
}

export interface ProductItem {
  id: string;
  name: string;
  description: string;
  variants: string;
  unitPrice: number;
  isFree: boolean;
}
