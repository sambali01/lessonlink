import { Box, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import MyDayCard from '../features/my-slots/MyDayCard';
import { AvailableSlot } from '../../models/AvailableSlot';
import { FunctionComponent, ReactElement } from 'react';
import { CalendarToday as CalendarIcon } from '@mui/icons-material';

interface MonthSectionProps {
    month: string;
    days: Record<string, AvailableSlot[]>;
    renderDayCard?: (date: string, slots: AvailableSlot[]) => ReactElement;
    showIcon?: boolean;
}

const MonthSection: FunctionComponent<MonthSectionProps> = ({ month, days, renderDayCard, showIcon = false }) => {
    const theme = useTheme();

    return (
        <Box sx={{ mb: 4 }}>
            <Typography
                variant="h5"
                sx={{
                    color: theme.palette.text.primary,
                    mb: showIcon ? 3 : 2,
                    pl: 1,
                    borderLeft: `4px solid ${theme.palette.primary.main}`,
                    display: showIcon ? 'flex' : 'block',
                    alignItems: 'center',
                    gap: 1,
                    fontWeight: showIcon ? 500 : 400
                }}
            >
                {showIcon && <CalendarIcon />}
                {month.charAt(0).toUpperCase() + month.slice(1)}
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
                        renderDayCard
                            ? renderDayCard(date, slots)
                            : <MyDayCard key={date} date={date} slots={slots} />
                    ))
                }
            </Box>
        </Box>
    );
};

export default MonthSection;