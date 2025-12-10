import { axiosInstance } from '../configs/axiosConfig';
import { AvailableSlot, CreateAvailableSlotRequest } from '../models/AvailableSlot';
import { PaginatedResponse } from '../models/PaginatedResponse';

const AVAILABLESLOT_API = '/AvailableSlots';

export const getMySlots = async (page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<AvailableSlot>> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/my-slots?page=${page}&pageSize=${pageSize}`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching my available slots: ' + error);
    }
};

export const getAvailableSlotsByTeacherId = async (teacherId: string): Promise<AvailableSlot[]> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/teacher/${teacherId}`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching available slots by teacher id: ' + error);
    }
};

export const createAvailableSlot = async (data: CreateAvailableSlotRequest): Promise<AvailableSlot> => {
    return await axiosInstance.post(`${AVAILABLESLOT_API}`, data)
        .then((response) => { return response.data; })
        .catch((error) => { throw new Error(error.response); });
};

export const deleteAvailableSlot = async (slotId: number): Promise<void> => {
    try {
        await axiosInstance.delete(`${AVAILABLESLOT_API}/${slotId}`);
    } catch (error) {
        throw new Error('Error deleting available slot: ' + error);
    }
};

export const getAvailableSlotDetails = async (slotId: number): Promise<AvailableSlot> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/${slotId}/details`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching available slot details: ' + error);
    }
};
