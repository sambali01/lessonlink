import {
    Card,
    CardContent,
    Typography,
    Box,
    Chip,
    useTheme
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { AvailableSlot } from '../../../models/AvailableSlot';
import { BookingStatus } from '../../../models/Booking';
import { FunctionComponent } from 'react';

interface MyDayCardProps {
    date: string;
    slots: AvailableSlot[];
}

const MyDayCard: FunctionComponent<MyDayCardProps> = ({ date, slots }) => {
    const theme = useTheme();
    const navigate = useNavigate();

    const formattedDate = new Date(date).toLocaleDateString('hu-HU', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });

    const formatTime = (dateTime: string) => {
        return new Date(dateTime).toLocaleTimeString('hu-HU', {
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const hasActiveBooking = (slot: AvailableSlot) => {
        return slot.bookings && slot.bookings.some(booking =>
            booking.status === BookingStatus.Pending || booking.status === BookingStatus.Confirmed
        );
    };

    const hasPendingBooking = (slot: AvailableSlot) => {
        return slot.bookings && slot.bookings.some(booking =>
            booking.status === BookingStatus.Pending
        );
    };

    const handleSlotClick = (slot: AvailableSlot) => {
        navigate(`/teacher/slots/${slot.id}/details`);
    };

    return (
        <>
            <Card
                sx={{
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column'
                }}
            >
                <CardContent sx={{ flexGrow: 1, p: 2 }}>
                    <Typography
                        variant="h6"
                        component="h3"
                        gutterBottom
                        sx={{
                            color: theme.palette.primary.main,
                            fontSize: '1.1rem',
                            fontWeight: 600
                        }}
                    >
                        {formattedDate}
                    </Typography>

                    <Box>
                        {slots
                            .sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime())
                            .map((slot) => {
                                const isBooked = hasActiveBooking(slot);
                                const isPending = hasPendingBooking(slot);
                                return (
                                    <Box
                                        key={slot.id}
                                        sx={{
                                            display: 'flex',
                                            alignItems: 'center',
                                            mb: 1,
                                            '&:last-child': { mb: 0 }
                                        }}
                                    >
                                        <Chip
                                            label={`${formatTime(slot.startTime)} - ${formatTime(slot.endTime)}`}
                                            onClick={() => handleSlotClick(slot)}
                                            sx={{
                                                flexGrow: 1,
                                                cursor: 'pointer',
                                                backgroundColor: isPending
                                                    ? theme.palette.warning.light
                                                    : isBooked
                                                        ? theme.palette.success.light
                                                        : theme.palette.primary.light,
                                                color: isPending
                                                    ? theme.palette.getContrastText(theme.palette.warning.light)
                                                    : isBooked
                                                        ? theme.palette.getContrastText(theme.palette.success.light)
                                                        : theme.palette.getContrastText(theme.palette.primary.light),
                                                '&:hover': {
                                                    backgroundColor: isPending
                                                        ? theme.palette.warning.main
                                                        : isBooked
                                                            ? theme.palette.success.main
                                                            : theme.palette.primary.main
                                                }
                                            }}
                                        />
                                    </Box>
                                );
                            })}
                    </Box>

                    <Typography
                        variant="body2"
                        sx={{
                            mt: 1,
                            color: theme.palette.text.secondary,
                            fontStyle: 'italic'
                        }}
                    >
                        {slots.length} óraidőpont
                    </Typography>
                </CardContent>
            </Card>
        </>
    );
};

export default MyDayCard;