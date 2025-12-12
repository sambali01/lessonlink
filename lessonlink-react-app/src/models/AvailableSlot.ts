import { Booking } from './Booking';

export type AvailableSlot = {
    id: number;
    teacherId: string;
    startTime: string;
    endTime: string;
    bookings: Booking[];
}

export interface CreateAvailableSlotRequest {
    startTime: string;
    endTime: string;
}

export interface UpdateAvailableSlotRequest {
    startTime: string;
    endTime: string;
}