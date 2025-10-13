import { Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { AvailableSlot } from '../../models/AvailableSlot';
import DayCard from './DayCard';

interface MonthSectionProps {
    month: string;
    days: Record<string, AvailableSlot[]>;
}

const MonthSection = ({ month, days }: MonthSectionProps) => {
    const theme = useTheme();

    return (
        <Box sx={{ mb: 4 }}>
            <Typography
                variant="h5"
                sx={{
                    color: theme.palette.text.primary,
                    mb: 2,
                    pl: 1,
                    borderLeft: `4px solid ${theme.palette.primary.main}`
                }}
            >
                {month}
            </Typography>

            <Box
                sx={{
                    display: 'grid',
                    gridTemplateColumns: {
                        xs: '1fr',
                        sm: 'repeat(2, 1fr)',
                        md: 'repeat(3, 1fr)',
                        lg: 'repeat(4, 1fr)',
                    },
                    gap: 2,
                }}
            >
                {Object.entries(days)
                    .sort(([dateA], [dateB]) => new Date(dateA).getTime() - new Date(dateB).getTime())
                    .map(([date, slots]) => (
                        <DayCard key={date} date={date} slots={slots} />
                    ))
                }
            </Box>
        </Box>
    );
};

export default MonthSection;