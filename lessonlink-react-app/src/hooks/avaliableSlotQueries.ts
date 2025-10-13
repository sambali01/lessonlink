import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { AvailableSlot } from '../models/AvailableSlot';
import { getMyAvailableSlots, getAvailableSlotsByTeacherId, createAvailableSlot, AvailableSlotCreateDto, deleteAvailableSlot } from '../services/availableSlot.service';

export const useMyAvailableSlots = () => {
    return useQuery<AvailableSlot[], Error>({
        queryKey: ['myAvailableSlots'],
        queryFn: getMyAvailableSlots,
        staleTime: 1000 * 60 * 5, // 5 minutes cache
    });
};

export const useAvailableSlotsByTeacherId = (teacherId: string) => {
    return useQuery<AvailableSlot[], Error>({
        queryKey: ['availableSlots', teacherId],
        queryFn: () => getAvailableSlotsByTeacherId(teacherId),
        enabled: !!teacherId,
        staleTime: 1000 * 60 * 5, // 5 minutes cache
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