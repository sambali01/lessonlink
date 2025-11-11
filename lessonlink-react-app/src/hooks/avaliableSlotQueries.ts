import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AvailableSlot } from '../models/AvailableSlot';
import { PaginatedResponse } from '../models/PaginatedResponse';
import {
    getMyAvailableSlots,
    getAvailableSlotsByTeacherId,
    createAvailableSlot,
    AvailableSlotCreateDto,
    deleteAvailableSlot,
    getAvailableSlotDetails,
    AvailableSlotDetailsDto
} from '../services/availableSlot.service';

export const useMyAvailableSlots = (page: number = 1, pageSize: number = 10) => {
    return useQuery<PaginatedResponse<AvailableSlot>, Error>({
        queryKey: ['myAvailableSlots', page, pageSize],
        queryFn: () => getMyAvailableSlots(page, pageSize),
        staleTime: 1000 * 60 * 5, // 5 minutes cache
        placeholderData: (previousData) => previousData,
    });
};

export const useAvailableSlotsByTeacherId = (teacherId: string) => {
    return useQuery<AvailableSlot[], Error>({
        queryKey: ['availableSlots', teacherId],
        queryFn: () => getAvailableSlotsByTeacherId(teacherId),
        enabled: !!teacherId
    });
};

export const useCreateAvailableSlot = () => {
    const queryClient = useQueryClient();

    return useMutation<AvailableSlot, Error, AvailableSlotCreateDto>({
        mutationFn: createAvailableSlot,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myAvailableSlots'] });
        },
    });
};

export const useDeleteAvailableSlot = () => {
    const queryClient = useQueryClient();

    return useMutation<void, Error, number>({
        mutationFn: deleteAvailableSlot,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['myAvailableSlots'] });
        },
    });
};

export const useAvailableSlotDetails = (slotId: number) => {
    return useQuery<AvailableSlotDetailsDto, Error>({
        queryKey: ['availableSlotDetails', slotId],
        queryFn: () => getAvailableSlotDetails(slotId),
        enabled: !!slotId
    });
};