import { useMutation } from "@tanstack/react-query";
import invalidateOrders from "../../common/invalidateOrders";
import fetchModifyItems from "./fetchModifyItems";

export default function useModifyItems() {
  return useMutation({
    mutationFn: fetchModifyItems,
    // Refresh on failure too: an error often means the order changed meanwhile
    onSettled: invalidateOrders,
  });
}
