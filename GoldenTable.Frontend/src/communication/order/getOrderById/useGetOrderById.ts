import { useQuery } from "@tanstack/react-query";
import fetchGetOrderById from "./fetchGetOrderById";

export default function useGetOrderById(id: string | undefined) {
  const query = useQuery({
    queryKey: ["orders", id],
    queryFn: ({ signal }) => fetchGetOrderById(id!, signal),
    enabled: !!id,
  });

  return query;
}
