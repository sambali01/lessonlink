import { axiosInstance } from '../configs/axiosConfig';
import { Booking, BookingAcceptanceRequest, CreateBookingRequest } from '../models/Booking';





const BOOKING_API = '/Bookings';

export const createBooking = async (data: CreateBookingRequest): Promise<Booking> => {
    try {
        const response = await axiosInstance.post(`${BOOKING_API}`, data);
        return response.data;
    } catch (error) {
        throw new Error('Error creating booking: ' + error);
    }
};

export const getMyBookingsAsStudent = async (): Promise<Booking[]> => {
    try {
        const response = await axiosInstance.get(`${BOOKING_API}/my-bookings/student`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching my bookings as student: ' + error);
    }
};

export const getMyBookingsAsTeacher = async (): Promise<Booking[]> => {
    try {
        const response = await axiosInstance.get(`${BOOKING_API}/my-bookings/teacher`);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching my bookings as teacher: ' + error);
    }
};

export const decideBookingAcceptance = async (bookingId: number, data: BookingAcceptanceRequest): Promise<Booking> => {
    try {
        const response = await axiosInstance.put(`${BOOKING_API}/${bookingId}/status`, data);
        return response.data;
    } catch (error) {
        throw new Error('Error updating booking status: ' + error);
    }
};

export const cancelBooking = async (bookingId: number): Promise<void> => {
    try {
        await axiosInstance.delete(`${BOOKING_API}/${bookingId}`);
    } catch (error) {
        throw new Error('Error cancelling booking: ' + error);
    }
};