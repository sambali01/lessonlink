import { axiosInstance } from "../configs/axiosConfig";
import { Subject } from "../models/Subject";

const SUBJECT_API = '/Subjects';

export const getSubjectNames = async (): Promise<string[]> => {
    try {
        const response = await axiosInstance.get(SUBJECT_API);
        const subjects = response.data;
        return subjects.map((s: Subject) => s.name);
    } catch (error) {
        console.log("Error subject!");
        throw new Error('Error fetching subjects: ' + error);
    }
};