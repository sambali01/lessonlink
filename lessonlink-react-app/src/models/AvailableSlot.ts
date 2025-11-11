import { Booking } from './Booking';

export type AvailableSlot = {
    id: number;
    teacherId: string;
    startTime: string;
    endTime: string;
    bookings?: Booking[];
}