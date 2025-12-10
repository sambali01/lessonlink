import { axiosInstance } from "../configs/axiosConfig";
import { Subject } from "../models/Subject";

const SUBJECT_API = '/Subjects';

export const getSubjects = async (): Promise<Subject[]> => {
    try {
        const response = await axiosInstance.get(SUBJECT_API);
        return response.data;
    } catch (error) {
        throw new Error('Error fetching subjects: ' + error);
    }
};