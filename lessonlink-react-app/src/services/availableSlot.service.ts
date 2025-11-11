import { axiosInstance } from '../configs/axiosConfig';
import { AvailableSlot } from '../models/AvailableSlot';
import { Booking } from '../models/Booking';
import { PaginatedResponse } from '../models/PaginatedResponse';

export interface AvailableSlotCreateDto {
    startTime: string; // ISO string
    endTime: string;   // ISO string
}

export interface AvailableSlotDetailsDto {
    id: number;
    teacherId: string;
    teacherName: string;
    startTime: string;
    endTime: string;
    booking?: Booking;
}

const AVAILABLESLOT_API = '/AvailableSlots';

export const getMyAvailableSlots = async (page: number = 1, pageSize: number = 10): Promise<PaginatedResponse<AvailableSlot>> => {
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

export const createAvailableSlot = async (data: AvailableSlotCreateDto): Promise<AvailableSlot> => {
    try {
        const response = await axiosInstance.post(`${AVAILABLESLOT_API}`, data);
        return response.data;
    } catch (error) {
        throw new Error('Error creating available slot: ' + error);
    }
};

export const deleteAvailableSlot = async (slotId: number): Promise<void> => {
    try {
        await axiosInstance.delete(`${AVAILABLESLOT_API}/${slotId}`);
    } catch (error) {
        throw new Error('Error deleting available slot: ' + error);
    }
};

export const getAvailableSlotDetails = async (slotId: number): Promise<AvailableSlotDetailsDto> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/${slotId}/details`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching available slot details: ' + error);
    }
};
