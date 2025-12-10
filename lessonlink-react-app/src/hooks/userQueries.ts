import { QueryObserverResult, useMutation, useQuery } from "@tanstack/react-query";
import { queryClient } from "../configs/queryConfig";
import { findUserById, registerStudent, registerTeacher, updateStudent, updateTeacher } from "../services/user.service";
import { RegisterStudentRequest, RegisterTeacherRequest, StudentUpdateRequest, TeacherUpdateRequest, User } from "../models/User";

export const useFindUserById = (userId: string = ''): QueryObserverResult<User> => {
    return useQuery<User>({
        queryFn: async () => {
            const { data } = await findUserById(userId);
            return data;
        },
        queryKey: ['user', { userId }]
    });
};

export const useRegisterStudent = () => {
    return useMutation<void, Error, RegisterStudentRequest>({
        mutationFn: async (data: RegisterStudentRequest) => {
            await registerStudent(data);
        }
    });
};

export const useRegisterTeacher = () => {
    return useMutation<void, Error, RegisterTeacherRequest>({
        mutationFn: async (data: RegisterTeacherRequest) => {
            await registerTeacher(data);
        }
    });
};

export const useUpdateStudent = (userId: string) => {
    return useMutation<User, Error, StudentUpdateRequest>({
        mutationFn: (data: StudentUpdateRequest) => updateStudent(userId, data),
        onSuccess: (data) => {
            queryClient.setQueryData(['user', { userId }], data);
        }
    });
};

export const useUpdateTeacher = (userId: string) => {
    return useMutation<User, Error, TeacherUpdateRequest>({
        mutationFn: (data: TeacherUpdateRequest) => updateTeacher(userId, data),
        onSuccess: (data) => {
            queryClient.setQueryData(['user', { userId }], data);
            queryClient.invalidateQueries({ queryKey: ['teacher', userId] });
        }
    });
};