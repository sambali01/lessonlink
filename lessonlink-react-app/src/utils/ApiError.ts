export class ApiError extends Error {
    public statusCode: number;
    public errors: string[];
    public isNetworkError: boolean;

    constructor(message: string, statusCode: number = 500, errors: string[] = [], isNetworkError: boolean = false) {
        super(message);
        this.name = 'ApiError';
        this.statusCode = statusCode;
        this.errors = errors.length > 0 ? errors : [message];
        this.isNetworkError = isNetworkError;
    }

    public static fromAxiosError(error: unknown): ApiError {
        if (axios.isAxiosError(error)) {
            // Network error - no response from server OR proxy 500 with empty data
            // (Vite proxy responds with 500 when backend is unreachable)
            if (!error.response ||
                (error.response.status === 500 && (!error.response.data || error.response.data === ''))) {
                return new ApiError(
                    'Network error',
                    0,
                    ['Nem sikerült kapcsolódni a szerverhez'],
                    true  // Mark as network error
                );
            }

            const statusCode = error.response.status;
            const errors = error.response.data;

            // Backend returns errors as array of strings
            if (Array.isArray(errors)) {
                return new ApiError(
                    errors.join('. '),
                    statusCode,
                    errors
                );
            }

            // Fallback for other error formats
            const message = error.response.data?.message
                || error.response.statusText
                || error.message
                || 'An unexpected error occurred';

            return new ApiError(message, statusCode, [message]);
        }

        // Non-axios error
        if (error instanceof Error) {
            return new ApiError(error.message, 500, [error.message]);
        }

        // Unknown error type
        return new ApiError('An unexpected error occurred', 500, ['An unexpected error occurred']);
    }
}

import axios from 'axios';
