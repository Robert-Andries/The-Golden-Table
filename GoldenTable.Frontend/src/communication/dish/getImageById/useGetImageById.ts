import { useQuery } from "@tanstack/react-query";
import fetchGetImageById from "./fetchGetImageById";

export default function useGetImageById(id: string) {
  const query = useQuery({
    queryKey: ['images', id],
    queryFn: () => fetchGetImageById(id),
    staleTime: 1000 * 60 * 5 // 5 minutes
  });

  return query;
}