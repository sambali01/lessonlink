import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Booking, BookingAcceptanceRequest, CreateBookingRequest } from '../models/Booking';
import {
    createBooking,
    getMyBookingsAsStudent,
    getMyBookingsAsTeacher,
    decideBookingAcceptance,
    cancelBooking
} from '../services/booking.service';
import { useAuth } from './useAuth';

export const useCreateBooking = () => {
    const queryClient = useQueryClient();

    return useMutation<Booking, Error, CreateBookingRequest>({
        mutationFn: createBooking,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myBookings'] });
            queryClient.invalidateQueries({ queryKey: ['availableSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myCurrentSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myPastSlots'] });
        },
    });
};

export const useMyBookingsAsStudent = () => {
    const { currentUserAuth } = useAuth();

    return useQuery<Booking[], Error>({
        queryKey: ['myBookings', currentUserAuth?.userId, 'student'],
        queryFn: getMyBookingsAsStudent,
        enabled: !!currentUserAuth?.userId,
    });
};

export const useMyBookingsAsTeacher = () => {
    const { currentUserAuth } = useAuth();

    return useQuery<Booking[], Error>({
        queryKey: ['myBookings', currentUserAuth?.userId, 'teacher'],
        queryFn: getMyBookingsAsTeacher,
        enabled: !!currentUserAuth?.userId,
    });
};

export const useDecideBookingAcceptance = () => {
    const queryClient = useQueryClient();

    return useMutation<Booking, Error, { bookingId: number; data: BookingAcceptanceRequest }>({
        mutationFn: ({ bookingId, data }) => decideBookingAcceptance(bookingId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myBookings'] });
            queryClient.invalidateQueries({ queryKey: ['availableSlotDetails'] });
            queryClient.invalidateQueries({ queryKey: ['myCurrentSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myPastSlots'] });
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
            queryClient.invalidateQueries({ queryKey: ['myCurrentSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myPastSlots'] });
        },
    });
};