import { useQuery } from "@tanstack/react-query";
import fetchGetMyOrderById from "./fetchGetMyOrderById";

export default function useGetMyOrderById(id: string | undefined) {
  const query = useQuery({
    queryKey: ["my-orders", id],
    queryFn: ({ signal }) => fetchGetMyOrderById(id!, signal),
    enabled: !!id,
  });

  return query;
}
