import { axiosInstance } from "../configs/axiosConfig";
import { TeacherDto } from "../dtos/TeacherDto";
import { PaginatedResponse } from "../models/PaginatedResponse";
import { TeacherSearchRequest } from "../models/TeacherSearchRequest";

const TEACHER_API = '/Teachers';

export const getFeaturedTeachers = async (): Promise<TeacherDto[]> => {
    try {
        const response = await axiosInstance.get(TEACHER_API + '/featuredteachers');
        return response.data;
    } catch (error) {
        throw new Error('Error fetching featured teachers: ' + error);
    }
};

export const getTeacherById = async (userId: string): Promise<TeacherDto> => {
    try {
        const response = await axiosInstance.get(`${TEACHER_API}/${userId}`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching teacher details: ' + error);
    }
};

export const searchTeachers = async (filters: TeacherSearchRequest): Promise<PaginatedResponse<TeacherDto>> => {
    try {
        const response = await axiosInstance.get<PaginatedResponse<TeacherDto>>(
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
        throw new Error('Error searching teachers: ' + error);
    }
};