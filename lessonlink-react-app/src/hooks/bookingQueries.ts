import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Booking, BookingAcceptanceRequest, CreateBookingRequest } from '../models/Booking';
import {
    createBooking,
    getMyBookingsAsStudent,
    getMyBookingsAsTeacher,
    decideBookingAcceptance,
    cancelBooking
} from '../services/booking.service';

export const useCreateBooking = () => {
    const queryClient = useQueryClient();

    return useMutation<Booking, Error, CreateBookingRequest>({
        mutationFn: createBooking,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myBookings', 'student'] });
            queryClient.invalidateQueries({ queryKey: ['myBookings', 'teacher'] });
            queryClient.invalidateQueries({ queryKey: ['availableSlots'] });
        },
    });
};

export const useMyBookingsAsStudent = () => {
    return useQuery<Booking[], Error>({
        queryKey: ['myBookings', 'student'],
        queryFn: getMyBookingsAsStudent,
    });
};

export const useMyBookingsAsTeacher = () => {
    return useQuery<Booking[], Error>({
        queryKey: ['myBookings', 'teacher'],
        queryFn: getMyBookingsAsTeacher,
    });
};

export const useDecideBookingAcceptance = () => {
    const queryClient = useQueryClient();

    return useMutation<Booking, Error, { bookingId: number; data: BookingAcceptanceRequest }>({
        mutationFn: ({ bookingId, data }) => decideBookingAcceptance(bookingId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myBookings'] });
            queryClient.invalidateQueries({ queryKey: ['availableSlotDetails'] });
        },
    });
};

export const useCancelBooking = () => {
    const queryClient = useQueryClient();

    return useMutation<void, Error, number>({
        mutationFn: cancelBooking,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myBookings', 'student'] });
            queryClient.invalidateQueries({ queryKey: ['myBookings', 'teacher'] });
            queryClient.invalidateQueries({ queryKey: ['availableSlots'] });
        },
    });
};