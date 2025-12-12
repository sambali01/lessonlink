import {
    CalendarToday as CalendarIcon,
    AccessTime as TimeIcon
} from '@mui/icons-material';
import {
    Box,
    Button,
    Paper,
    Stack,
    Typography,
    useTheme
} from '@mui/material';
import { FunctionComponent } from 'react';
import { useAuth } from '../../../hooks/useAuth';
import { AvailableSlot } from '../../../models/AvailableSlot';

interface ScheduleDayCardProps {
    date: string;
    slots: AvailableSlot[];
    onBookSlot?: (slot: AvailableSlot) => void;
    showBookingButtons?: boolean;
}

const ScheduleDayCard: FunctionComponent<ScheduleDayCardProps> = ({ date, slots, onBookSlot }) => {
    const theme = useTheme();

    const { currentUserAuth } = useAuth();

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

    return (
        <Paper sx={{
            p: 3,
            borderRadius: 2,
            border: `1px solid ${theme.palette.divider}`,
            transition: 'all 0.2s ease',
            '&:hover': {
                boxShadow: theme.shadows[4],
                transform: 'translateY(-2px)'
            }
        }}>
            <Typography
                variant="h6"
                sx={{
                    color: theme.palette.primary.main,
                    mb: 2,
                    display: 'flex',
                    alignItems: 'center',
                    gap: 1,
                    fontWeight: 'bold'
                }}
            >
                <CalendarIcon />
                {formattedDate}
            </Typography>

            {/* Slot List */}
            <Stack spacing={1.5}>
                {slots.map((slot) => (
                    <Box
                        key={slot.id}
                        sx={{
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'space-between',
                            p: 2,
                            borderRadius: 1,
                            backgroundColor: theme.palette.mode === 'dark'
                                ? theme.palette.grey[800]
                                : theme.palette.grey[50],
                            border: `1px solid ${theme.palette.divider}`
                        }}
                    >
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <TimeIcon
                                sx={{
                                    color: theme.palette.secondary.main,
                                    fontSize: '1.2rem'
                                }}
                            />
                            <Typography variant="body1" sx={{ fontWeight: 'medium' }}>
                                {formatTime(slot.startTime)} - {formatTime(slot.endTime)}
                            </Typography>
                        </Box>

                        {currentUserAuth && slot.teacherId !== currentUserAuth.userId && onBookSlot && (
                            <Button
                                variant="contained"
                                size="small"
                                onClick={() => onBookSlot(slot)}
                                sx={{
                                    backgroundColor: theme.palette.secondary.main,
                                    '&:hover': {
                                        backgroundColor: theme.palette.secondary.dark
                                    }
                                }}
                            >
                                Foglalás
                            </Button>
                        )}
                    </Box>
                ))}
            </Stack>
        </Paper>
    );
};

export default ScheduleDayCard;