import { useQuery } from "@tanstack/react-query";
import { getSubjects } from "../services/subject.service";
import { Subject } from "../models/Subject";

export const useSubjects = () => {
    return useQuery<Subject[]>({
        queryKey: ['subjects'],
        queryFn: getSubjects
    });
};