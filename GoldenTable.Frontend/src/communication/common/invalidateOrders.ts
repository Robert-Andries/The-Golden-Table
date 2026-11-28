import queryClient from "./queryClient";

/** Refreshes every cached order list and order, for both staff and customer views. */
export default async function invalidateOrders(): Promise<void> {
  await Promise.all([
    queryClient.invalidateQueries({ queryKey: ["orders"] }),
    queryClient.invalidateQueries({ queryKey: ["my-orders"] }),
  ]);
}
