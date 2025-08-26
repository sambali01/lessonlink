import { AvailableSlot } from "./AvailableSlot";
import { User } from "./User";

export type Booking = {
    student: User;
    availableSlot: AvailableSlot;
    status: BookingStatus;
    createdAt: Date;
}

const enum BookingStatus {
    Pending,
    Confirmed,
    Cancelled
}