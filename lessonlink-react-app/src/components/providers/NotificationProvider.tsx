import { FunctionComponent, ReactNode, useCallback, useEffect, useState } from 'react';
import { Notification, NotificationContext, NotificationType } from '../../contexts/NotificationContext';
import NotificationSnackbar from '../common/NotificationSnackbar';
import { queryClient } from '../../configs/queryConfig';
import { ApiError } from '../../utils/ApiError';
import axios from 'axios';

interface NotificationProviderProps {
    children: ReactNode;
}

export const NotificationProvider: FunctionComponent<NotificationProviderProps> = ({ children }) => {
    const [notifications, setNotifications] = useState<Notification[]>([]);

    const showNotification = useCallback((message: string, type: NotificationType) => {
        const id = `${Date.now()}-${Math.random()}`;
        const notification: Notification = { id, message, type };

        setNotifications(prev => [...prev, notification]);

        // Auto-remove after 6 seconds
        setTimeout(() => {
            setNotifications(prev => prev.filter(n => n.id !== id));
        }, 6000);
    }, []);

    const showSuccess = useCallback((message: string) => {
        showNotification(message, 'success');
    }, [showNotification]);

    const showError = useCallback((message: string | string[]) => {
        if (Array.isArray(message)) {
            // If there are multiple errors, show them combined
            const combinedMessage = message.join('. ');
            showNotification(combinedMessage, 'error');
        } else {
            showNotification(message, 'error');
        }
    }, [showNotification]);

    // Global error handler for React Query mutations and queries
    useEffect(() => {
        // Track which errors we've already handled to avoid duplicates
        const handledErrors = new Set<string>();

        const mutationUnsubscribe = queryClient.getMutationCache().subscribe((event) => {
            if (event.type === 'updated') {
                const mutation = event.mutation;

                // Check if mutation failed
                if (mutation.state.status === 'error') {
                    const error = mutation.state.error;
                    const errorKey = `mutation-${mutation.mutationId}`;

                    // Only handle each error once
                    if (!handledErrors.has(errorKey)) {
                        handledErrors.add(errorKey);

                        // Skip network errors - they are handled by queryConfig
                        if (error instanceof ApiError && error.isNetworkError) {
                            return;
                        }

                        if (error instanceof ApiError) {
                            // Show notifications for all API errors (4xx and 5xx)
                            if (error.statusCode >= 400) {
                                showError(error.errors);
                            }
                        } else if (!(axios.isAxiosError(error) && !error.response)) {
                            // Show notification for non-network errors
                            showError('Váratlan hiba történt. Próbálkozz újra.');
                        }
                    }
                }
            }
        });

        const queryUnsubscribe = queryClient.getQueryCache().subscribe((event) => {
            if (event.type === 'updated' && event.query.state.status === 'error') {
                const error = event.query.state.error;
                const errorKey = `query-${event.query.queryHash}`;

                // Only handle each error once
                if (!handledErrors.has(errorKey)) {
                    handledErrors.add(errorKey);

                    // Skip network errors - they are handled by queryConfig
                    if (error instanceof ApiError && error.isNetworkError) {
                        return;
                    }

                    if (error instanceof ApiError) {
                        // Show notifications for all API errors (4xx and 5xx)
                        if (error.statusCode >= 400) {
                            showError(error.errors);
                        }
                    } else if (!(axios.isAxiosError(error) && !error.response)) {
                        // Show notification for non-network errors
                        showError('Váratlan hiba történt. Próbálkozz újra.');
                    }
                }
            }
        });

        return () => {
            mutationUnsubscribe();
            queryUnsubscribe();
        };
    }, [showError]);

    const handleClose = useCallback((id: string) => {
        setNotifications(prev => prev.filter(n => n.id !== id));
    }, []);

    return (
        <NotificationContext.Provider value={{ showNotification, showSuccess, showError }}>
            {children}
            {notifications.map(notification => (
                <NotificationSnackbar
                    key={notification.id}
                    notification={notification}
                    onClose={() => handleClose(notification.id)}
                />
            ))}
        </NotificationContext.Provider>
    );
};
