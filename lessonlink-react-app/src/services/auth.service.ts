import { axiosInstance } from "../configs/axiosConfig";
import { ApiError } from "../utils/ApiError";

const AUTH_API = "/Auth";

export const login = async (email: string, password: string) => {
    try {
        const response = await axiosInstance
            .post(AUTH_API + "/login", {
                email: email,
                password: password,
            });
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const refresh = async () => {
    try {
        const response = await axiosInstance.post(AUTH_API + "/refresh");
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
}

export const logout = async () => {
    try {
        const response = await axiosInstance
            .post(AUTH_API + "/logout");
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
}