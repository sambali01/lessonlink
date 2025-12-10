export type RegisterTeacherDto = {
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
