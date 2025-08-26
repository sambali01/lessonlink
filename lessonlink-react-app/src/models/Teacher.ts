import { Subject } from "./Subject";
import { User } from "./User";

export type Teacher = {
    user: User;
    acceptOnline?: boolean;
    acceptInPerson?: boolean;
    location?: string;
    hourlyRate?: number;
    description?: string;
    rating?: string;
    subjects: Subject[];
}