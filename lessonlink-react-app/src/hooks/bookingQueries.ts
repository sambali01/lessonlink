import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Booking } from '../models/Booking';
import {
    createBooking,
    getMyBookingsAsStudent,
    getMyBookingsAsTeacher,
    updateBookingStatus,
    cancelBooking,
    BookingCreateDto,
    BookingUpdateStatusDto
} from '../services/booking.service';

export const useCreateBooking = () => {
    const queryClient = useQueryClient();

    return useMutation<Booking, Error, BookingCreateDto>({
        mutationFn: createBooking,
        onSuccess: () => {
            // Invalidate both student and teacher bookings
            queryClient.invalidateQueries({ queryKey: ['myBookings'] });
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

export const useUpdateBookingStatus = () => {
    const queryClient = useQueryClient();

    return useMutation<Booking, Error, { bookingId: number; data: BookingUpdateStatusDto }>({
        mutationFn: ({ bookingId, data }) => updateBookingStatus(bookingId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myBookings'] });
        },
    });
};

export const useCancelBooking = () => {
    const queryClient = useQueryClient();

    return useMutation<void, Error, number>({
        mutationFn: cancelBooking,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myBookings'] });
            queryClient.invalidateQueries({ queryKey: ['availableSlots'] });
        },
    });
};