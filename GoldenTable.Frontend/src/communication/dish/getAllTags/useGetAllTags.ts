import { useQuery } from "@tanstack/react-query";
import { fetchAllTags } from "./fetchGetAllTags";

export default function useGetAllTags() {
    const query = useQuery({
        queryKey: ['dishes', 'tags'],
        queryFn: ({signal}) => fetchAllTags(signal),
        staleTime: 1000 * 60 * 5 // 5 minutes
    })

    return query;
}