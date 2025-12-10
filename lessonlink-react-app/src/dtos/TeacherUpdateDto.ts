export type TeacherUpdateDto = {
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
