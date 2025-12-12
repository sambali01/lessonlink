import {
    Event as EventIcon
} from '@mui/icons-material';
import {
    Box,
    CircularProgress,
    Paper,
    Typography,
    useTheme,
} from '@mui/material';
import { FunctionComponent } from 'react';
import { AvailableSlot } from '../../../models/AvailableSlot';
import MonthSection from '../../common/MonthSection';
import ScheduleDayCard from './ScheduleDayCard';

interface TeacherScheduleViewProps {
    slots: AvailableSlot[];
    isLoading: boolean;
    onBookSlot?: (slot: AvailableSlot) => void;
}

const TeacherScheduleView: FunctionComponent<TeacherScheduleViewProps> = ({ slots, isLoading, onBookSlot }) => {
    const theme = useTheme();

    if (isLoading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="300px">
                <CircularProgress size={60} />
            </Box>
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
                    Nézz vissza később!
                </Typography>
            </Paper>
        );
    }

    return (
        <Box>
            {Object.entries(monthGroups).map(([month, days]) => (
                <MonthSection
                    key={month}
                    month={month}
                    days={days}
                    showIcon={true}
                    renderDayCard={(date, daySlots) => (
                        <ScheduleDayCard
                            key={date}
                            date={date}
                            slots={daySlots}
                            onBookSlot={onBookSlot}
                        />
                    )}
                />
            ))}
        </Box>
    );
};

export default TeacherScheduleView;