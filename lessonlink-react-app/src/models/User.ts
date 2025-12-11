export type User = {
    firstName: string;
    surName: string;
    nickName?: string;
    imageUrl?: string;
}

export type UserAuth = {
    userId: string;
    roles: string[];
};

export interface RegisterStudentRequest {
    firstName: string;
    surName: string;
    email: string;
    password: string;
};

export interface RegisterTeacherRequest {
    firstName: string;
    surName: string;
    email: string;
    password: string;
    acceptsOnline: boolean;
    acceptsInPerson: boolean;
    location: string | null;
    hourlyRate: number;
    contact: string;
    subjectNames: string[];
};

export interface StudentUpdateRequest {
    nickName?: string;
    profilePicture?: File;
};

export interface TeacherUpdateRequest {
    nickName?: string;
    profilePicture?: File;
    acceptsOnline?: boolean;
    acceptsInPerson?: boolean;
    location?: string;
    hourlyRate?: number;
    description?: string;
    contact?: string;
    subjectNames?: string[];
};

export enum Role {
    Student = "Student",
    Teacher = "Teacher"
}