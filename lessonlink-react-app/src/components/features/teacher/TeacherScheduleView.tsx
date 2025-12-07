import {
    CalendarToday as CalendarIcon,
    Event as EventIcon
} from '@mui/icons-material';
import {
    Alert,
    Box,
    CircularProgress,
    Paper,
    Typography,
    useTheme,
} from '@mui/material';
import { FunctionComponent } from 'react';
import { AvailableSlot } from '../../../models/AvailableSlot';
import ScheduleDayCard from './ScheduleDayCard';

interface TeacherScheduleViewProps {
    slots: AvailableSlot[];
    isLoading: boolean;
    error: Error | null;
    onBookSlot?: (slot: AvailableSlot) => void;
    showBookingButtons?: boolean;
}

const TeacherScheduleView: FunctionComponent<TeacherScheduleViewProps> = ({
    slots,
    isLoading,
    error,
    onBookSlot
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