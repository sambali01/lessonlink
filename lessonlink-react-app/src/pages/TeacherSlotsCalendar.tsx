import {
    Box,
    Typography,
    useTheme,
    CircularProgress,
    Alert,
    Button,
    Snackbar
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { useEffect, useState } from 'react';
import { AvailableSlot } from '../models/AvailableSlot';
import MonthSection from '../components/calendar/MonthSection';
import { useMyAvailableSlots, useCreateAvailableSlot, useDeleteAvailableSlot } from '../hooks/avaliableSlotQueries';
import CreateSlotModal from '../components/calendar/CreateSlotModal';
import { AvailableSlotCreateDto } from '../services/availableSlot.service';

const TeacherSlotsCalendar = () => {
    const theme = useTheme();
    const { data: slots, isLoading, error } = useMyAvailableSlots();
    const createSlotMutation = useCreateAvailableSlot();
    const deleteSlotMutation = useDeleteAvailableSlot();

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [snackbar, setSnackbar] = useState<{ open: boolean; message: string; severity: 'success' | 'error' }>({
        open: false,
        message: '',
        severity: 'success'
    });

    useEffect(() => {
        if (deleteSlotMutation.error) {
            setSnackbar({
                open: true,
                message: deleteSlotMutation.error.message,
                severity: 'error'
            });
        }
    }, [deleteSlotMutation.error]);

    const handleCreateSlot = async (data: AvailableSlotCreateDto) => {
        try {
            await createSlotMutation.mutateAsync(data);
            setIsModalOpen(false);
            setSnackbar({
                open: true,
                message: 'Óraidőpont sikeresen létrehozva!',
                severity: 'success'
            });
        } catch (error) {
            setSnackbar({
                open: true,
                message: error instanceof Error ? error.message : 'Hiba történt a létrehozás során',
                severity: 'error'
            });
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbar(prev => ({ ...prev, open: false }));
    };

    if (isLoading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Alert severity="error" sx={{ m: 2 }}>
                {error.message}
            </Alert>
        );
    }

    // Group slots by month and then by day
    const groupedSlots = (slots || []).reduce((acc, slot) => {
        const startDate = new Date(slot.startTime);
        const monthKey = startDate.toLocaleString('hu-HU', { month: 'long', year: 'numeric' });
        const dateKey = startDate.toISOString().split('T')[0]; // YYYY-MM-DD

        if (!acc[monthKey]) {
            acc[monthKey] = {};
        }

        if (!acc[monthKey][dateKey]) {
            acc[monthKey][dateKey] = [];
        }

        acc[monthKey][dateKey].push(slot);
        return acc;
    }, {} as Record<string, Record<string, AvailableSlot[]>>);

    return (
        <Box sx={{ p: { xs: 1, sm: 2 } }}>
            {/* Fejléc és gomb */}
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography
                    variant="h4"
                    component="h1"
                    sx={{
                        color: theme.palette.text.primary,
                    }}
                >
                    Óraidőpontjaim
                </Typography>

                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={() => setIsModalOpen(true)}
                    sx={{
                        textTransform: 'none'
                    }}
                >
                    Új időpont
                </Button>
            </Box>

            {/* Tartalom */}
            {Object.keys(groupedSlots).length === 0 ? (
                <Box textAlign="center" mt={4}>
                    <Typography
                        variant="h6"
                        sx={{
                            color: theme.palette.text.secondary,
                            mb: 2
                        }}
                    >
                        Nincsenek óraidőpontok.
                    </Typography>
                    <Button
                        variant="outlined"
                        onClick={() => setIsModalOpen(true)}
                    >
                        Hozd létre az elsőt!
                    </Button>
                </Box>
            ) : (
                Object.entries(groupedSlots).map(([month, days]) => (
                    <MonthSection key={month} month={month} days={days} />
                ))
            )}

            {/* Modal */}
            <CreateSlotModal
                open={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onCreate={handleCreateSlot}
                isLoading={createSlotMutation.isPending}
                error={createSlotMutation.error?.message}
            />

            {/* Snackbar */}
            <Snackbar
                open={snackbar.open}
                autoHideDuration={6000}
                onClose={handleCloseSnackbar}
            >
                <Alert
                    onClose={handleCloseSnackbar}
                    severity={snackbar.severity}
                    variant="filled"
                >
                    {snackbar.message}
                </Alert>
            </Snackbar>
        </Box>
    );
};

export default TeacherSlotsCalendar;