import { TeacherSearchRequest } from '../models/TeacherSearchRequest';

export const DEFAULT_TEACHER_SEARCH_FILTERS: TeacherSearchRequest = {
    acceptsOnline: true,
    acceptsInPerson: true,
    page: 1,
    pageSize: 12
};

export const PAGE_SIZE = 12;