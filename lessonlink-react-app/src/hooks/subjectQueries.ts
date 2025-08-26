import { useQuery } from "@tanstack/react-query";
import { getSubjects } from "../services/subject.service";

export const useSubjects = () => {
    return useQuery<string[]>({
        queryKey: ['subjects'],
        queryFn: getSubjects,
        staleTime: Infinity
    });
};