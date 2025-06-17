export interface Product {
  id: string;
  code: string;
  name: string;
  description: string;
  startDate: string;
  startTime: string;
  endDate: string;
  endTime: string;
  address: string;
  imageUrl: string;
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
