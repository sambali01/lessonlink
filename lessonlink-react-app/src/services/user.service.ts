import { AxiosResponse } from "axios";
import { axiosInstance } from "../configs/axiosConfig";
import { RegisterStudentRequest, RegisterTeacherRequest, StudentUpdateRequest, TeacherUpdateRequest, User } from "../models/User";
import { ApiError } from "../utils/ApiError";

const USER_API = "/Users";

export const registerStudent = async (data: RegisterStudentRequest) => {
    try {
        return await axiosInstance.post(USER_API + "/register-student", data);
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const registerTeacher = async (data: RegisterTeacherRequest) => {
    try {
        return await axiosInstance.post(USER_API + "/register-teacher", data);
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const findUserById = async (userId: string): Promise<AxiosResponse<User>> => {
    try {
        return await axiosInstance.get<User>(USER_API + "/" + userId);
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const updateStudent = async (userId: string, data: StudentUpdateRequest) => {

    const formData = new FormData();

    if (data.nickName) formData.append('nickName', data.nickName);
    if (data.profilePicture) formData.append('profilePicture', data.profilePicture);

    try {
        const response = await axiosInstance.put<User>(`${USER_API}/update-student/${userId}`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const updateTeacher = async (userId: string, data: TeacherUpdateRequest): Promise<User> => {

    const formData = new FormData();

    if (data.nickName) formData.append('nickName', data.nickName);
    if (data.profilePicture) formData.append('profilePicture', data.profilePicture);
    if (data.acceptsOnline !== undefined) formData.append('acceptsOnline', data.acceptsOnline.toString());
    if (data.acceptsInPerson !== undefined) formData.append('acceptsInPerson', data.acceptsInPerson.toString());
    if (data.location) formData.append('location', data.location);
    if (data.hourlyRate !== undefined) formData.append('hourlyRate', data.hourlyRate.toString());
    if (data.description) formData.append('description', data.description);
    if (data.contact) formData.append('contact', data.contact);
    if (data.subjectNames) {
        data.subjectNames.forEach((subject) => {
            formData.append('subjectNames', subject);
        });
    }

    try {
        const response = await axiosInstance.put<User>(`${USER_API}/update-teacher/${userId}`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};