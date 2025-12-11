import { FunctionComponent, ReactNode, useCallback, useEffect, useState } from 'react';
import { Notification, NotificationContext, NotificationType } from '../../contexts/NotificationContext';
import NotificationSnackbar from '../common/NotificationSnackbar';
import { queryClient } from '../../configs/queryConfig';
import { ApiError } from '../../utils/ApiError';
import { useNavigate } from 'react-router-dom';

interface NotificationProviderProps {
    children: ReactNode;
}

export const NotificationProvider: FunctionComponent<NotificationProviderProps> = ({ children }) => {
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const navigate = useNavigate();

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

                        if (error instanceof ApiError) {
                            // Server errors (5xx) - navigate to error page
                            if (error.statusCode >= 500) {
                                navigate('/server-error');
                            }
                            // Client errors (4xx) - show notification
                            else if (error.statusCode >= 400) {
                                showError(error.errors);
                            }
                        } else {
                            // Network or unknown errors
                            showError('An unexpected error occurred. Please try again.');
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

                    if (error instanceof ApiError) {
                        // Server errors (5xx) - navigate to error page
                        if (error.statusCode >= 500) {
                            navigate('/server-error');
                        }
                        // Client errors (4xx) - show notification
                        else if (error.statusCode >= 400) {
                            showError(error.errors);
                        }
                    } else {
                        // Network or unknown errors
                        showError('An unexpected error occurred. Please try again.');
                    }
                }
            }
        });

        return () => {
            mutationUnsubscribe();
            queryUnsubscribe();
        };
    }, [navigate, showError]);

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
