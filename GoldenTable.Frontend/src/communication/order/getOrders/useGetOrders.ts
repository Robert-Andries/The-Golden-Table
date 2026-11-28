import { useQuery } from "@tanstack/react-query";
import fetchGetOrders from "./fetchGetOrders";

export default function useGetOrders() {
  const query = useQuery({
    queryKey: ["orders"],
    queryFn: ({ signal }) => fetchGetOrders(signal),
  });

  return query;
}
