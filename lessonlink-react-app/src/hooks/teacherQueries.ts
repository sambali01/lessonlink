import { keepPreviousData, useQuery } from '@tanstack/react-query';
import { TeacherDto } from '../dtos/TeacherDto';
import { getFeaturedTeachers, getTeacherById, searchTeachers } from '../services/teacher.service';
import { TeacherSearchFilters } from '../models/TeacherSearchFilters';
import { PaginatedResponse } from '../models/PaginatedResponse';

export const useFeaturedTeachers = () => {
    return useQuery({
        queryKey: ['featuredTeachers'],
        queryFn: getFeaturedTeachers,
        staleTime: 1000 * 60 * 5    // 5 minutes cache
    });
};

export const useTeacherDetails = (userId: string, options?: { enabled?: boolean }) => {
    return useQuery<TeacherDto>({
        queryKey: ['teacher', userId],
        queryFn: () => getTeacherById(userId),
        enabled: !!userId && (options?.enabled ?? true)
    });
};

export const useSearchTeachers = (filters: TeacherSearchFilters) => {
    return useQuery<PaginatedResponse<TeacherDto>, Error>({
        queryKey: ['teachers', filters],
        queryFn: () => searchTeachers(filters),
        placeholderData: keepPreviousData,
    });
};