import { useMutation } from "@tanstack/react-query";
import invalidateOrders from "../../common/invalidateOrders";
import fetchRequestCancel from "./fetchRequestCancel";

export default function useRequestCancel() {
  return useMutation({
    mutationFn: fetchRequestCancel,
    // Refresh on failure too: an error often means the order changed meanwhile
    onSettled: invalidateOrders,
  });
}
