import type { OrderStatus } from "./orderStatus";

export type orderItem = {
  dishId: string;
  sizeName: string | null;
  quantity: number;
};

export type address = {
  county: string;
  city: string;
  street: string;
  streetNumber: string;
  zipCode: string;
};

export type order = {
  id: string;
  customerId: string;
  status: OrderStatus;
  cancelRequested: boolean;
  totalPriceAmount: number;
  totalPriceCurrency: string;
  items: orderItem[];
  address: address;
  createdOnUtc: string;
  modifiedOnUtc: string;
};
