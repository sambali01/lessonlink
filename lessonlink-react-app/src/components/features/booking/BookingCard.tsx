import React from 'react';
import {
    Card,
    CardContent,
    Typography,
    Box,
    Chip,
    Button,
    useTheme,
    Stack,
    Divider
} from '@mui/material';
import {
    CalendarToday as CalendarIcon,
    AccessTime as TimeIcon,
    Person as PersonIcon,
    Cancel as CancelIcon,
    CheckCircle as ConfirmIcon,
    Schedule as PendingIcon
} from '@mui/icons-material';
import { Booking, BookingStatus } from '../../../models/Booking';

interface BookingCardProps {
    booking: Booking;
    onCancel?: (bookingId: number) => void;
    showCancelButton?: boolean;
    isLoading?: boolean;
}

const BookingCard: React.FC<BookingCardProps> = ({
    booking,
    onCancel,
    showCancelButton = true,
    isLoading = false
}) => {
    const theme = useTheme();

    const formatDateTime = (dateTimeString: string) => {
        const date = new Date(dateTimeString);
        return {
            date: date.toLocaleDateString('hu-HU', {
                year: 'numeric',
                month: 'long',
                day: 'numeric',
                weekday: 'long'
            }),
            time: date.toLocaleTimeString('hu-HU', {
                hour: '2-digit',
                minute: '2-digit'
            })
        };
    };

    const getStatusConfig = (status: number) => {
        switch (status) {
            case 0: // BookingStatus.Pending
                return {
                    color: 'warning' as const,
                    icon: <PendingIcon />,
                    label: 'Függőben',
                    bgColor: theme.palette.warning.light
                };
            case 1: // BookingStatus.Confirmed
                return {
                    color: 'success' as const,
                    icon: <ConfirmIcon />,
                    label: 'Megerősítve',
                    bgColor: theme.palette.success.light
                };
            case 2: // BookingStatus.Cancelled
                return {
                    color: 'error' as const,
                    icon: <CancelIcon />,
                    label: 'Törölve',
                    bgColor: theme.palette.error.light
                };
            default:
                return {
                    color: 'default' as const,
                    icon: <PendingIcon />,
                    label: 'Ismeretlen',
                    bgColor: theme.palette.grey[300]
                };
        }
    };

    const startTime = formatDateTime(booking.slotStartTime);
    const endTime = formatDateTime(booking.slotEndTime);
    const statusConfig = getStatusConfig(booking.status);

    const canCancel = booking.status === BookingStatus.Pending || booking.status === BookingStatus.Confirmed;

    return (
        <Card sx={{
            borderRadius: 2,
            boxShadow: theme.shadows[2],
            transition: 'all 0.2s ease',
            '&:hover': {
                boxShadow: theme.shadows[4],
                transform: 'translateY(-2px)'
            },
            borderLeft: `4px solid ${statusConfig.bgColor}`
        }}>
            <CardContent sx={{ p: 3 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                    <Box>
                        <Typography variant="h6" sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                            <PersonIcon color="primary" />
                            {booking.teacherName}
                        </Typography>
                        <Chip
                            icon={statusConfig.icon}
                            label={statusConfig.label}
                            color={statusConfig.color}
                            variant="outlined"
                            size="small"
                        />
                    </Box>
                    {showCancelButton && canCancel && onCancel && (
                        <Button
                            variant="outlined"
                            color="error"
                            size="small"
                            startIcon={<CancelIcon />}
                            onClick={() => onCancel(booking.id)}
                            disabled={isLoading}
                            sx={{ minWidth: 'auto' }}
                        >
                            Lemondás
                        </Button>
                    )}
                </Box>

                <Divider sx={{ my: 2 }} />

                <Stack spacing={2}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <CalendarIcon color="secondary" />
                        <Typography variant="body1" sx={{ fontWeight: 'medium' }}>
                            {startTime.date}
                        </Typography>
                    </Box>

                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <TimeIcon color="secondary" />
                        <Typography variant="body1">
                            {startTime.time} - {endTime.time}
                        </Typography>
                    </Box>

                    {booking.notes && (
                        <Box sx={{
                            p: 2,
                            bgcolor: theme.palette.mode === 'dark'
                                ? theme.palette.grey[800]
                                : theme.palette.grey[50],
                            borderRadius: 1,
                            border: `1px solid ${theme.palette.divider}`
                        }}>
                            <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
                                "{booking.notes}"
                            </Typography>
                        </Box>
                    )}

                    <Box sx={{ pt: 1 }}>
                        <Typography variant="caption" color="text.secondary">
                            Foglalás ideje: {new Date(booking.createdAt).toLocaleDateString('hu-HU')}
                        </Typography>
                    </Box>
                </Stack>
            </CardContent>
        </Card>
    );
};

export default BookingCard;