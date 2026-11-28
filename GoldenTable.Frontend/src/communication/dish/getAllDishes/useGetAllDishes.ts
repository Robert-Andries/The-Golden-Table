import { useQuery } from "@tanstack/react-query";
import { fetchGetAllDishes } from "./fetchGetAllDishes";

export default function useGetAllDishes() {
    const query = useQuery({
        queryKey: ['dishes'],
        queryFn: ({signal}) => fetchGetAllDishes(signal),
        staleTime: 1000 * 60 * 5 // 5 minutes
    })

    return query;
}