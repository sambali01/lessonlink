import { axiosInstance } from "../configs/axiosConfig";
import { PaginatedResponse } from "../models/PaginatedResponse";
import { Teacher, TeacherSearchRequest } from "../models/Teacher";
import { ApiError } from "../utils/ApiError";

const TEACHER_API = '/Teachers';

export const getFeaturedTeachers = async (): Promise<Teacher[]> => {
    try {
        const response = await axiosInstance.get(TEACHER_API + '/featuredteachers');
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const getTeacherById = async (userId: string): Promise<Teacher> => {
    try {
        const response = await axiosInstance.get(`${TEACHER_API}/${userId}`);
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const searchTeachers = async (filters: TeacherSearchRequest): Promise<PaginatedResponse<Teacher>> => {
    try {
        const response = await axiosInstance.get<PaginatedResponse<Teacher>>(
            `${TEACHER_API}/search`,
            {
                params: filters,

                // Custom params serializer to handle array parameters
                paramsSerializer: (params) => {
                    const searchParams = new URLSearchParams();

                    Object.entries(params).forEach(([key, value]) => {
                        if (Array.isArray(value)) {
                            value.forEach(item => searchParams.append(key, item));
                        } else if (value !== undefined && value !== null && value !== '') {
                            searchParams.append(key, String(value));
                        }
                    });

                    return searchParams.toString();
                }
            }
        );
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const getTeacherContact = async (teacherId: string): Promise<string> => {
    try {
        const response = await axiosInstance.get<string>(`${TEACHER_API}/${teacherId}/contact`);
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};