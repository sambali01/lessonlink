import { axiosInstance } from '../configs/axiosConfig';
import { AvailableSlot, CreateAvailableSlotRequest, UpdateAvailableSlotRequest } from '../models/AvailableSlot';
import { PaginatedResponse } from '../models/PaginatedResponse';
import { ApiError } from '../utils/ApiError';

const AVAILABLESLOT_API = '/AvailableSlots';

export const getAvailableSlotDetails = async (slotId: number): Promise<AvailableSlot> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/${slotId}/details`);
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const getMyCurrentSlots = async (page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<AvailableSlot>> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/my-slots/current?page=${page}&pageSize=${pageSize}`);
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const getMyPastSlots = async (page: number, pageSize: number): Promise<PaginatedResponse<AvailableSlot>> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/my-slots/past?page=${page}&pageSize=${pageSize}`);
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const getCurrentSlotsByTeacherId = async (teacherId: string, page: number, pageSize: number): Promise<PaginatedResponse<AvailableSlot>> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/teacher/${teacherId}?page=${page}&pageSize=${pageSize}`);
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const createAvailableSlot = async (data: CreateAvailableSlotRequest): Promise<AvailableSlot> => {
    try {
        const response = await axiosInstance.post(`${AVAILABLESLOT_API}`, data);
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const updateAvailableSlot = async (slotId: number, data: UpdateAvailableSlotRequest): Promise<AvailableSlot> => {
    try {
        const response = await axiosInstance.put(`${AVAILABLESLOT_API}/${slotId}`, data);
        return response.data;
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};

export const deleteAvailableSlot = async (slotId: number): Promise<void> => {
    try {
        await axiosInstance.delete(`${AVAILABLESLOT_API}/${slotId}`);
    } catch (error) {
        throw ApiError.fromAxiosError(error);
    }
};
