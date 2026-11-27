import { useQuery } from "@tanstack/react-query";
import fetchGetOrdersByCustomer from "./fetchGetOrdersByCustomer";

export default function useGetOrdersByCustomer() {
  const query = useQuery({
    queryKey: ["my-orders"],
    queryFn: ({ signal }) => fetchGetOrdersByCustomer(signal),
  });

  return query;
}
