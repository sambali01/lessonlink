import React from 'react';
import {
    Box,
    Typography,
    useTheme,
    CircularProgress,
    Alert,
    Paper,
    Stack,
    Button
} from '@mui/material';
import {
    CalendarToday as CalendarIcon,
    AccessTime as TimeIcon,
    Event as EventIcon
} from '@mui/icons-material';
import { AvailableSlot } from '../../../models/AvailableSlot';

interface TeacherScheduleViewProps {
    slots: AvailableSlot[];
    isLoading: boolean;
    error: Error | null;
    onBookSlot?: (slot: AvailableSlot) => void;
    showBookingButtons?: boolean;
}

interface ScheduleDayCardProps {
    date: string;
    slots: AvailableSlot[];
    onBookSlot?: (slot: AvailableSlot) => void;
    showBookingButtons?: boolean;
}

const ScheduleDayCard: React.FC<ScheduleDayCardProps> = ({
    date,
    slots,
    onBookSlot,
    showBookingButtons = false
}) => {
    const theme = useTheme();

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

                        {showBookingButtons && onBookSlot && (
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

const TeacherScheduleView: React.FC<TeacherScheduleViewProps> = ({
    slots,
    isLoading,
    error,
    onBookSlot,
    showBookingButtons = false
}) => {
    const theme = useTheme();

    if (isLoading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Alert severity="error" sx={{ borderRadius: 2 }}>
                {error.message}
            </Alert>
        );
    }

    // Group slots by date
    const groupedSlots = (slots || []).reduce((acc, slot) => {
        const startDate = new Date(slot.startTime);
        const dateKey = startDate.toISOString().split('T')[0]; // YYYY-MM-DD

        if (!acc[dateKey]) {
            acc[dateKey] = [];
        }

        acc[dateKey].push(slot);
        return acc;
    }, {} as Record<string, AvailableSlot[]>);

    // Group by month for better organization
    const monthGroups = Object.entries(groupedSlots).reduce((acc, [date, daySlots]) => {
        const monthKey = new Date(date).toLocaleString('hu-HU', {
            month: 'long',
            year: 'numeric'
        });

        if (!acc[monthKey]) {
            acc[monthKey] = {};
        }

        acc[monthKey][date] = daySlots;
        return acc;
    }, {} as Record<string, Record<string, AvailableSlot[]>>);

    if (Object.keys(groupedSlots).length === 0) {
        return (
            <Paper sx={{ p: 6, textAlign: 'center', borderRadius: 2 }}>
                <EventIcon
                    sx={{
                        fontSize: 60,
                        color: theme.palette.text.secondary,
                        mb: 2
                    }}
                />
                <Typography variant="h6" color="text.secondary" gutterBottom>
                    Jelenleg nincsenek elérhető időpontok
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    Nézz vissza később vagy vedd fel a kapcsolatot a tanárral.
                </Typography>
            </Paper>
        );
    }

    return (
        <Box>
            {Object.entries(monthGroups).map(([month, days]) => (
                <Box key={month} sx={{ mb: 4 }}>
                    <Typography
                        variant="h4"
                        sx={{
                            color: theme.palette.text.primary,
                            mb: 3,
                            pl: 1,
                            borderLeft: `4px solid ${theme.palette.primary.main}`,
                            display: 'flex',
                            alignItems: 'center',
                            gap: 1
                        }}
                    >
                        <CalendarIcon />
                        {month}
                    </Typography>

                    <Box
                        sx={{
                            display: 'grid',
                            gridTemplateColumns: {
                                xs: '1fr',
                                sm: 'repeat(2, 1fr)',
                                lg: 'repeat(3, 1fr)',
                            },
                            gap: 3,
                        }}
                    >
                        {Object.entries(days)
                            .sort(([dateA], [dateB]) => new Date(dateA).getTime() - new Date(dateB).getTime())
                            .map(([date, daySlots]) => (
                                <ScheduleDayCard
                                    key={date}
                                    date={date}
                                    slots={daySlots}
                                    onBookSlot={onBookSlot}
                                    showBookingButtons={showBookingButtons}
                                />
                            ))
                        }
                    </Box>
                </Box>
            ))}
        </Box>
    );
};

export default TeacherScheduleView;