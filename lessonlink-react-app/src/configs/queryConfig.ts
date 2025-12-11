import { QueryClient } from '@tanstack/react-query';
import { ApiError } from '../utils/ApiError';

export const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            // Don't retry any requests by default to avoid loops
            retry: false,
            // Consider data fresh for 30 seconds
            staleTime: 30 * 1000,
            // Refetch on window focus
            refetchOnWindowFocus: true,
        },
        mutations: {
            // Don't retry mutations by default
            retry: false,
        },
    },
});

// Global error handler for queries and mutations
// Handles network errors (server unreachable) by redirecting to error page
// API errors (including 5xx) are shown as notifications by NotificationProvider
queryClient.getQueryCache().config.onError = (error) => {
    // Prevent infinite loop - don't redirect if already on error page
    if (window.location.pathname === '/server-error') return;

    // Check if this is a network error (marked by ApiError.isNetworkError flag)
    if (error instanceof ApiError && error.isNetworkError) {
        window.location.href = '/server-error';
    }
};

queryClient.getMutationCache().config.onError = (error) => {
    // Prevent infinite loop - don't redirect if already on error page
    if (window.location.pathname === '/server-error') return;

    // Check if this is a network error (marked by ApiError.isNetworkError flag)
    if (error instanceof ApiError && error.isNetworkError) {
        window.location.href = '/server-error';
    }
};