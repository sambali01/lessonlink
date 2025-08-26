import { Booking } from "./Booking";

export type User = {
    firstName: string;
    surName: string;
    nickName?: string;
    profilePicture?: string;
    bookings: Booking[];
}