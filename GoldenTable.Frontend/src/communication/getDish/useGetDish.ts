import { useQuery } from "@tanstack/react-query";
import { fetchGetDish } from "./fetchGetDish";

export default function useGetDish(id: string | undefined) {
    const query = useQuery({
        queryKey: ['dish', id],
        queryFn: ({signal}) => fetchGetDish(id!, signal),
        staleTime: 1000 * 60 * 5, // 5 minutes
        enabled: !!id   // !!id means if id is undefined or empty
    })

    return query;
}