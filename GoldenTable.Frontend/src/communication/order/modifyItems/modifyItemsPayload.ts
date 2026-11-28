import type { orderItem } from "../../../types/order";

export type modifyItemsPayload = {
  orderId: string;
  items: orderItem[];
};
