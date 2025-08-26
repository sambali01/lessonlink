import { Teacher } from "./Teacher";

export type AvailableSlot = {
    teacher: Teacher;
    startTime: Date;
    endTime: Date;
}