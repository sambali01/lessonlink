import { axiosInstance } from "../configs/axiosConfig";
import { TeacherDto } from "../dtos/TeacherDto";
import { PaginatedResponse } from "../models/PaginatedResponse";
import { TeacherSearchFilters } from "../models/TeacherSearchFilters";

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

export const searchTeachers = async (filters: TeacherSearchFilters): Promise<PaginatedResponse<TeacherDto>> => {
    try {
        const response = await axiosInstance.get<PaginatedResponse<TeacherDto>>(
            `${TEACHER_API}/search`,
            { params: filters }
        );
        return response.data;
    } catch (error) {
        console.log("Error teacher!");
        throw new Error('Error searching teachers: ' + error);
    }
};