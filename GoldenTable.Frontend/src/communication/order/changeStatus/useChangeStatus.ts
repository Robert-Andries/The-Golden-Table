import { useMutation } from "@tanstack/react-query";
import type { OrderStatus } from "../../../types/orderStatus";
import invalidateOrders from "../../common/invalidateOrders";
import fetchChangeStatus from "./fetchChangeStatus";

type variables = {
  orderId: string;
  status: OrderStatus;
};

export default function useChangeStatus() {
  return useMutation({
    mutationFn: ({ orderId, status }: variables) => fetchChangeStatus(orderId, status),
    // Refresh on failure too: an error often means the order changed meanwhile
    onSettled: invalidateOrders,
  });
}
