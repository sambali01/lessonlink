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