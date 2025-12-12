import { createContext } from 'react';

export type NotificationType = 'success' | 'error' | 'info' | 'warning';

export interface Notification {
    id: string;
    message: string;
    type: NotificationType;
}

export interface NotificationContextType {
    showNotification: (message: string, type: NotificationType) => void;
    showSuccess: (message: string) => void;
    showError: (message: string | string[]) => void;
}

export const NotificationContext = createContext<NotificationContextType | undefined>(undefined);
