import { Subject } from "./Subject";

export type Teacher = {
    userId: string;
    firstName: string;
    surName: string;
    nickName: string;
    imageUrl?: string;
    acceptsOnline?: boolean;
    acceptsInPerson?: boolean;
    location?: string;
    hourlyRate?: number;
    description?: string;
    contact?: string;
    subjects: Subject[];
}

export interface TeacherSearchRequest {
    searchText?: string;
    subjects?: string[];
    minPrice?: number;
    maxPrice?: number;
    acceptsOnline?: boolean;
    acceptsInPerson?: boolean;
    location?: string;
    page?: number;
    pageSize?: number;
}