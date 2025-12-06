import {
    Box,
    Typography,
    useTheme,
    CircularProgress,
    Alert,
    Button,
    Snackbar,
    Pagination
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { useEffect, useState } from 'react';
import { AvailableSlot } from '../models/AvailableSlot';
import { useMyAvailableSlots, useCreateAvailableSlot, useDeleteAvailableSlot } from '../hooks/avaliableSlotQueries';
import { AvailableSlotCreateDto } from '../services/availableSlot.service';
import MonthSection from '../components/features/calendar/MonthSection';
import CreateSlotModal from '../components/features/calendar/CreateSlotModal';
import { PAGE_SIZE } from '../constants/searchDefaults';

const TeacherSlotsCalendar = () => {
    const theme = useTheme();
    const [currentPage, setCurrentPage] = useState(1);
    const { data: slotsResponse, isLoading, error } = useMyAvailableSlots(currentPage, PAGE_SIZE);
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

    const slots = slotsResponse?.items || [];
    const totalPages = slotsResponse?.totalPages || 0;

    const handlePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
        setCurrentPage(value);
    };

    // Group slots by month and then by day
    const groupedSlots = slots.reduce((acc, slot) => {
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
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography
                    variant="h2"
                    component="h1"
                    sx={{
                        mb: 4,
                        color: theme.palette.text.primary,
                        fontWeight: 400,
                        letterSpacing: '-1px'
                    }}
                >
                    Óraidőpontjaim
                </Typography>

                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={() => setIsModalOpen(true)}
                    sx={{
                        position: 'fixed',
                        top: 80,
                        right: 24,
                        zIndex: theme.zIndex.appBar,
                        textTransform: 'none'
                    }}
                >
                    Új időpont
                </Button>
            </Box>

            {Object.keys(groupedSlots).length === 0 ? (
                <Box textAlign="center" mt={4}>
                    <Typography
                        variant="h6"
                        sx={{
                            color: theme.palette.text.secondary,
                            mb: 2
                        }}
                    >
                        {slotsResponse?.totalCount === 0
                            ? 'Még nincsenek óraidőpontok.'
                            : 'Nincsenek óraidőpontok ezen az oldalon.'}
                    </Typography>
                    {slotsResponse?.totalCount === 0 && (
                        <Button
                            variant="outlined"
                            onClick={() => setIsModalOpen(true)}
                        >
                            Hozd létre az elsőt!
                        </Button>
                    )}
                </Box>
            ) : (
                <>
                    {Object.entries(groupedSlots).map(([month, days]) => (
                        <MonthSection key={month} month={month} days={days} />
                    ))}
                </>
            )}

            {/* Pagination */}
            {totalPages > 1 && (
                <Box display="flex" flexDirection="column" alignItems="center" mt={4}>
                    <Typography
                        variant="body2"
                        sx={{
                            color: theme.palette.text.secondary,
                            mb: 2
                        }}
                    >
                        {slotsResponse?.totalCount} óraidőpont összesen
                    </Typography>
                    <Pagination
                        count={totalPages}
                        page={currentPage}
                        onChange={handlePageChange}
                        color="primary"
                        size="large"
                        sx={{
                            '& .MuiPaginationItem-root': {
                                fontSize: '1rem',
                            },
                        }}
                    />
                </Box>
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