import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AvailableSlot, CreateAvailableSlotRequest, UpdateAvailableSlotRequest } from '../models/AvailableSlot';
import { PaginatedResponse } from '../models/PaginatedResponse';
import {
    createAvailableSlot,
    deleteAvailableSlot,
    getAvailableSlotDetails,
    getCurrentSlotsByTeacherId,
    getMyCurrentSlots,
    getMyPastSlots,
    updateAvailableSlot
} from '../services/availableSlot.service';
import { useAuth } from './useAuth';

export const useMyCurrentSlots = (page: number, pageSize: number) => {
    const { currentUserAuth } = useAuth();

    return useQuery<PaginatedResponse<AvailableSlot>, Error>({
        queryKey: ['myCurrentSlots', currentUserAuth?.userId, page, pageSize],
        queryFn: () => getMyCurrentSlots(page, pageSize),
        enabled: !!currentUserAuth?.userId,
        staleTime: 1000 * 60 * 5,
        placeholderData: (previousData) => previousData,
    });
};

export const useMyPastSlots = (page: number, pageSize: number) => {
    const { currentUserAuth } = useAuth();

    return useQuery<PaginatedResponse<AvailableSlot>, Error>({
        queryKey: ['myPastSlots', currentUserAuth?.userId, page, pageSize],
        queryFn: () => getMyPastSlots(page, pageSize),
        enabled: !!currentUserAuth?.userId,
        staleTime: 1000 * 60 * 5,
        placeholderData: (previousData) => previousData,
    });
};

export const useAvailableSlotsByTeacherId = (teacherId: string, page: number, pageSize: number) => {
    return useQuery<PaginatedResponse<AvailableSlot>, Error>({
        queryKey: ['availableSlots', teacherId, page, pageSize],
        queryFn: () => getCurrentSlotsByTeacherId(teacherId, page, pageSize),
        enabled: !!teacherId,
        staleTime: 1000 * 60 * 5,
        placeholderData: (previousData) => previousData,
    });
};

export const useCreateAvailableSlot = () => {
    const queryClient = useQueryClient();

    return useMutation<AvailableSlot, Error, CreateAvailableSlotRequest>({
        mutationFn: createAvailableSlot,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myAvailableSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myCurrentSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myPastSlots'] });
        },
    });
};

export const useUpdateAvailableSlot = () => {
    const queryClient = useQueryClient();

    return useMutation<AvailableSlot, Error, { slotId: number; data: UpdateAvailableSlotRequest }>({
        mutationFn: ({ slotId, data }) => updateAvailableSlot(slotId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myAvailableSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myCurrentSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myPastSlots'] });
            queryClient.invalidateQueries({ queryKey: ['availableSlotDetails'] });
        },
    });
};

export const useDeleteAvailableSlot = () => {
    const queryClient = useQueryClient();

    return useMutation<void, Error, number>({
        mutationFn: deleteAvailableSlot,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myAvailableSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myCurrentSlots'] });
            queryClient.invalidateQueries({ queryKey: ['myPastSlots'] });
            queryClient.invalidateQueries({ queryKey: ['availableSlotDetails'] });
        },
    });
};

export const useAvailableSlotDetails = (slotId: number, enabled: boolean = true) => {
    const { currentUserAuth } = useAuth();

    return useQuery<AvailableSlot, Error>({
        queryKey: ['availableSlotDetails', currentUserAuth?.userId, slotId],
        queryFn: () => getAvailableSlotDetails(slotId),
        enabled: enabled && !!slotId && !!currentUserAuth?.userId
    });
};