export type Booking = {
    id: number;
    studentId: string;
    studentName: string;
    availableSlotId: number;
    slotStartTime: string;
    slotEndTime: string;
    teacherId: string;
    teacherName: string;
    status: BookingStatus;
    createdAt: string;
}

export interface CreateBookingRequest {
    availableSlotId: number;
}

export interface BookingAcceptanceRequest {
    status: BookingStatus;
}

export enum BookingStatus {
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2
}