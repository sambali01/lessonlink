import { QueryClient } from '@tanstack/react-query';

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