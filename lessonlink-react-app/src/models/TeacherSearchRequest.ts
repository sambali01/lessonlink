export interface TeacherSearchRequest {
    searchQuery?: string;
    subjects?: string[];
    minPrice?: number;
    maxPrice?: number;
    minRating?: number;
    acceptsOnline: boolean;
    acceptsInPerson: boolean;
    location?: string;
    page?: number;
    pageSize?: number;
}