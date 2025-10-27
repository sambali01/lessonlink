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
    notes?: string;
    createdAt: string;
}

export enum BookingStatus {
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2
}