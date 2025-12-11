import { Alert, Snackbar } from '@mui/material';
import { FunctionComponent } from 'react';
import { Notification } from '../../contexts/NotificationContext';

interface NotificationSnackbarProps {
    notification: Notification;
    onClose: () => void;
}

const NotificationSnackbar: FunctionComponent<NotificationSnackbarProps> = ({
    notification,
    onClose
}) => {
    return (
        <Snackbar
            open={true}
            autoHideDuration={6000}
            onClose={onClose}
            anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
        >
            <Alert
                onClose={onClose}
                severity={notification.type}
                variant="filled"
                sx={{ width: '100%' }}
            >
                {notification.message}
            </Alert>
        </Snackbar>
    );
};

export default NotificationSnackbar;
