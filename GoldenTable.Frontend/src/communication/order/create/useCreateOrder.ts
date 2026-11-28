import { useMutation } from "@tanstack/react-query";
import invalidateOrders from "../../common/invalidateOrders";
import fetchCreateOrder from "./fetchCreateOrder";

export default function useCreateOrder() {
  return useMutation({
    mutationFn: fetchCreateOrder,
    // Refresh on failure too: an error often means the order changed meanwhile
    onSettled: invalidateOrders,
  });
}
