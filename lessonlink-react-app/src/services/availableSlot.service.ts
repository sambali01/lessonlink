import { axiosInstance } from '../configs/axiosConfig';
import { AvailableSlot } from '../models/AvailableSlot';

export interface AvailableSlotCreateDto {
    startTime: string; // ISO string
    endTime: string;   // ISO string
}

const AVAILABLESLOT_API = '/AvailableSlots';

export const getMyAvailableSlots = async (): Promise<AvailableSlot[]> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/my-slots`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching my available slots: ' + error);
    }
};

export const getAvailableSlotsByTeacherId = async (teacherId: string): Promise<AvailableSlot[]> => {
    try {
        const response = await axiosInstance.get(`${AVAILABLESLOT_API}/${teacherId}`);
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
