export type ProfileUpdateDto = {
    nickName?: string;
    profilePicture?: File;
    acceptsOnline?: boolean;
    acceptsInPerson?: boolean;
    location?: string;
    hourlyRate?: number;
};