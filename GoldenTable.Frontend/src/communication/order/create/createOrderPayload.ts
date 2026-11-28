import type { address, orderItem } from "../../../types/order";

export type createOrderPayload = {
  items: orderItem[];
  address: address;
};
