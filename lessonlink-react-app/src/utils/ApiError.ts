export class ApiError extends Error {
    public statusCode: number;
    public errors: string[];

    constructor(message: string, statusCode: number = 500, errors: string[] = []) {
        super(message);
        this.name = 'ApiError';
        this.statusCode = statusCode;
        this.errors = errors.length > 0 ? errors : [message];
    }

    public static fromAxiosError(error: unknown): ApiError {
        if (axios.isAxiosError(error)) {
            const statusCode = error.response?.status || 500;
            const errors = error.response?.data;

            // Backend returns errors as array of strings
            if (Array.isArray(errors)) {
                return new ApiError(
                    errors.join('. '),
                    statusCode,
                    errors
                );
            }

            // Fallback for other error formats
            const message = error.response?.data?.message
                || error.response?.statusText
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
