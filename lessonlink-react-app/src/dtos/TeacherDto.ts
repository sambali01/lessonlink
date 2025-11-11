export type TeacherDto = {
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
    rating?: string;
    subjects: string[];
}