import {
    CalendarToday as CalendarIcon,
    Cancel as CancelIcon,
    CheckCircle as ConfirmIcon,
    ContactMail as ContactIcon,
    Schedule as PendingIcon,
    Person as PersonIcon,
    AccessTime as TimeIcon
} from '@mui/icons-material';
import {
    Box,
    Button,
    Card,
    CardContent,
    Chip,
    CircularProgress,
    Divider,
    Link,
    Stack,
    Typography,
    useTheme
} from '@mui/material';
import { FunctionComponent } from 'react';
import { useTeacherContact } from '../../../hooks/teacherQueries';
import { Booking, BookingStatus } from '../../../models/Booking';

interface BookingCardProps {
    booking: Booking;
    onCancel?: (bookingId: number) => void;
    showCancellationButton?: boolean;
    isLoading?: boolean;
}

const BookingCard: FunctionComponent<BookingCardProps> = ({
    booking,
    onCancel,
    showCancellationButton: showCancelButton = true,
    isLoading = false
}) => {
    const theme = useTheme();

    // Only fetch contact if booking is confirmed
    const shouldFetchContact = booking.status === BookingStatus.Confirmed;
    const { data: teacherContact, isLoading: isLoadingContact } = useTeacherContact(
        booking.teacherId,
        { enabled: shouldFetchContact }
    );

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

    const getStatusConfig = (status: BookingStatus) => {
        switch (status) {
            case BookingStatus.Pending:
                return {
                    color: 'warning' as const,
                    icon: <PendingIcon />,
                    label: 'Függőben',
                    bgColor: theme.palette.warning.light
                };
            case BookingStatus.Confirmed:
                return {
                    color: 'success' as const,
                    icon: <ConfirmIcon />,
                    label: 'Jóváhagyva',
                    bgColor: theme.palette.success.light
                };
            case BookingStatus.Cancelled:
                return {
                    color: 'error' as const,
                    icon: <CancelIcon />,
                    label: 'Lemondva',
                    bgColor: theme.palette.error.light
                };
            default:
                return {
                    color: 'warning' as const,
                    icon: <PendingIcon />,
                    label: 'Függőben',
                    bgColor: theme.palette.warning.light
                };
        }
    };

    const startTime = formatDateTime(booking.slotStartTime);
    const endTime = formatDateTime(booking.slotEndTime);
    const statusConfig = getStatusConfig(booking.status);

    // Check if booking can be cancelled
    const canCancel = () => {
        if (booking.status !== BookingStatus.Pending && booking.status !== BookingStatus.Confirmed) {
            return false;
        }

        const startDate = new Date(booking.slotStartTime);
        const now = new Date();
        const hoursUntilStart = (startDate.getTime() - now.getTime()) / (1000 * 60 * 60);

        return hoursUntilStart >= 24;
    };

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
                    {showCancelButton && canCancel() && onCancel && (
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

                    {shouldFetchContact && (
                        <>
                            <Divider sx={{ my: 1 }} />
                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <ContactIcon color="primary" />
                                {isLoadingContact ? (
                                    <CircularProgress size={20} />
                                ) : teacherContact ? (
                                    <Box>
                                        <Typography variant="caption" color="text.secondary" display="block">
                                            Kapcsolat:
                                        </Typography>
                                        <Link
                                            href={`mailto:${teacherContact}`}
                                            sx={{
                                                textDecoration: 'none',
                                                color: theme.palette.primary.main,
                                                '&:hover': {
                                                    textDecoration: 'underline'
                                                }
                                            }}
                                        >
                                            {teacherContact}
                                        </Link>
                                    </Box>
                                ) : null}
                            </Box>
                        </>
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