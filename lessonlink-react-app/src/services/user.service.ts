import { AxiosResponse } from "axios";
import { axiosInstance } from "../configs/axiosConfig";
import { UserDto } from "../dtos/UserDto";
import { Role } from "../models/Role";
import { ProfileUpdateDto } from "../dtos/ProfileUpdateDto";

const USER_API = "/Users";

export const register = async (firstName: string, surName: string, email: string, password: string, role: Role) => {
    return await axiosInstance.post(USER_API + "/register", {
        firstname: firstName,
        surname: surName,
        email: email,
        password: password,
        role: role
    });
};

export const findUserById = async (userId: string): Promise<AxiosResponse<UserDto>> => {
    return await axiosInstance.get<UserDto>(USER_API + "/" + userId);
};

export const updateProfile = async (userId: string, data: ProfileUpdateDto): Promise<UserDto> => {
    const formData = new FormData();

    if (data.nickName) formData.append('nickName', data.nickName);
    if (data.profilePicture) formData.append('profilePicture', data.profilePicture);
    if (data.acceptsOnline !== undefined) formData.append('acceptsOnline', data.acceptsOnline.toString());
    if (data.acceptsInPerson !== undefined) formData.append('acceptsInPerson', data.acceptsInPerson.toString());
    if (data.location) formData.append('location', data.location);
    if (data.hourlyRate !== undefined) formData.append('hourlyRate', data.hourlyRate.toString());
    if (data.subjects) {
        data.subjects.forEach((subject) => {
            formData.append('subjects', subject);
        });
    }

    const response = await axiosInstance.patch<UserDto>(`${USER_API}/${userId}`, formData, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
    return response.data;
};