export type TeacherDto = {
    userId: string;
    firstName: string;
    surName: string;
    nickName: string;
    profilePicture?: string;
    acceptsOnline?: boolean;
    acceptsInPerson?: boolean;
    location?: string;
    hourlyRate?: number;
    description?: string;
    rating?: string;
    subjects: string[];
}