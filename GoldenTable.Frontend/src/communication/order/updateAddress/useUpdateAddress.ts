import { useMutation } from "@tanstack/react-query";
import invalidateOrders from "../../common/invalidateOrders";
import fetchUpdateAddress from "./fetchUpdateAddress";

export default function useUpdateAddress() {
  return useMutation({
    mutationFn: fetchUpdateAddress,
    // Refresh on failure too: an error often means the order changed meanwhile
    onSettled: invalidateOrders,
  });
}
