import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { PaginatedResponse } from '../models/PaginatedResponse';
import { Teacher, TeacherSearchRequest } from '../models/Teacher';
import { getFeaturedTeachers, getTeacherById, searchTeachers } from '../services/teacher.service';

export const useFeaturedTeachers = () => {
    return useQuery({
        queryKey: ['featuredTeachers'],
        queryFn: getFeaturedTeachers,
        staleTime: 1000 * 60 * 5    // 5 minutes cache
    });
};

export const useTeacherDetails = (userId: string, options?: { enabled?: boolean }) => {
    return useQuery<Teacher>({
        queryKey: ['teacher', userId],
        queryFn: () => getTeacherById(userId),
        enabled: !!userId && (options?.enabled ?? true)
    });
};

export const useSearchTeachers = (filters: TeacherSearchRequest) => {
    return useQuery<PaginatedResponse<Teacher>, Error>({
        queryKey: ['teachers', filters],
        queryFn: () => searchTeachers(filters),
        placeholderData: keepPreviousData,
    });
};