import { QueryObserverResult, useMutation, useQuery } from "@tanstack/react-query";
import { queryClient } from "../configs/queryConfig";
import { RegisterStudentDto } from "../dtos/RegisterStudentDto";
import { RegisterTeacherDto } from "../dtos/RegisterTeacherDto";
import { StudentUpdateDto } from "../dtos/StudentUpdateDto";
import { TeacherUpdateDto } from "../dtos/TeacherUpdateDto";
import { UserDto } from "../dtos/UserDto";
import { findUserById, registerStudent, registerTeacher, updateStudent, updateTeacher } from "../services/user.service";

export const useFindUserById = (userId: string = ''): QueryObserverResult<UserDto> => {
    return useQuery<UserDto>({
        queryFn: async () => {
            const { data } = await findUserById(userId);
            return data;
        },
        queryKey: ['user', { userId }]
    });
};

export const useRegisterStudent = () => {
    return useMutation<void, Error, RegisterStudentDto>({
        mutationFn: async (data: RegisterStudentDto) => {
            await registerStudent(data);
        }
    });
};

export const useRegisterTeacher = () => {
    return useMutation<void, Error, RegisterTeacherDto>({
        mutationFn: async (data: RegisterTeacherDto) => {
            await registerTeacher(data);
        }
    });
};

export const useUpdateStudent = (userId: string) => {
    return useMutation<UserDto, Error, StudentUpdateDto>({
        mutationFn: (data: StudentUpdateDto) => updateStudent(userId, data),
        onSuccess: (data) => {
            queryClient.setQueryData(['user', { userId }], data);
        }
    });
};

export const useUpdateTeacher = (userId: string) => {
    return useMutation<UserDto, Error, TeacherUpdateDto>({
        mutationFn: (data: TeacherUpdateDto) => updateTeacher(userId, data),
        onSuccess: (data) => {
            queryClient.setQueryData(['user', { userId }], data);
            queryClient.invalidateQueries({ queryKey: ['teacher', userId] });
        }
    });
};