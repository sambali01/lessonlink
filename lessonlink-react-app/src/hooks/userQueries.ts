import { QueryObserverResult, useMutation, useQuery } from "@tanstack/react-query";
import { queryClient } from "../configs/queryConfig";
import { ProfileUpdateDto } from "../dtos/ProfileUpdateDto";
import { UserDto } from "../dtos/UserDto";
import { findUserById, updateProfile } from "../services/user.service";

export const useFindUserById = (userId: string = ''): QueryObserverResult<UserDto> => {
    return useQuery<UserDto>({
        queryFn: async () => {
            const { data } = await findUserById(userId);
            return data;
        },
        queryKey: ['user', { userId }]
    });
};

export const useUpdateProfile = (userId: string) => {
    return useMutation<UserDto, Error, ProfileUpdateDto>({
        mutationFn: (data: ProfileUpdateDto) => updateProfile(userId, data),
        onSuccess: (data) => {
            queryClient.setQueryData(['user', { userId }], data);
            queryClient.invalidateQueries({ queryKey: ['teacher', userId] });
        }
    });
};