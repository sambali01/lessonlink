export interface TeacherSearchFilters {
    searchQuery?: string;
    subjects?: string[];
    minPrice?: number;
    maxPrice?: number;
    minRating?: number;
    acceptsOnline?: boolean;
    acceptsInPerson?: boolean;
    page?: number;
    pageSize?: number;
    [key: string]: unknown;
}