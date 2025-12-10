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

export type RegisterStudentRequest = {
    firstName: string;
    surName: string;
    email: string;
    password: string;
};

export type RegisterTeacherRequest = {
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

export type StudentUpdateRequest = {
    nickName?: string;
    profilePicture?: File;
};

export type TeacherUpdateRequest = {
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