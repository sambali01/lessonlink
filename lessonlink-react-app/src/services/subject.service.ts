import { axiosInstance } from "../configs/axiosConfig";

const SUBJECT_API = '/Subjects';

export const getSubjects = async (): Promise<string[]> => {
    try {
        const response = await axiosInstance.get<string[]>(SUBJECT_API);
        console.log(response.data);
        return response.data;
    } catch (error) {
        console.log("Error subject!");
        throw new Error('Error fetching subjects: ' + error);
    }
};