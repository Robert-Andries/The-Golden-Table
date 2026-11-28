import { useQuery } from "@tanstack/react-query";
import { fetchGetAllImages } from "./fetchGetAllImages";

export default function useGetAllImages() {
  const query = useQuery({
    queryKey: ["images"],
    queryFn: ({ signal }) => fetchGetAllImages(signal),
    staleTime: 1000 * 60 * 5, // 5 minutes
  });

  return query;
}
