import type { address } from "../../../types/order";

export type updateAddressPayload = {
  orderId: string;
  address: address;
};
