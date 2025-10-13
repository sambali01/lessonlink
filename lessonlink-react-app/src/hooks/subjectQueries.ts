import { useQuery } from "@tanstack/react-query";
import { getSubjectNames } from "../services/subject.service";

export const useSubjects = () => {
    return useQuery<string[]>({
        queryKey: ['subjects'],
        queryFn: getSubjectNames
    });
};