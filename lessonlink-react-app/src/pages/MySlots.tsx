import { Add as AddIcon } from '@mui/icons-material';
import {
    Box,
    Button,
    CircularProgress,
    Pagination,
    Tab,
    Tabs,
    Typography,
    useTheme
} from '@mui/material';
import { FunctionComponent, useEffect, useState } from 'react';
import MonthSection from '../components/common/MonthSection';
import SlotFormModal from '../components/features/my-slots/SlotFormModal';
import { useCreateAvailableSlot, useDeleteAvailableSlot, useMyCurrentSlots, useMyPastSlots } from '../hooks/avaliableSlotQueries';
import { useNotification } from '../hooks/useNotification';
import { AvailableSlot, CreateAvailableSlotRequest } from '../models/AvailableSlot';
import { ApiError } from '../utils/ApiError';
import { MY_SLOTS_PAGE_SIZE } from '../utils/constants';
import { useLocation } from 'react-router-dom';

const MySlots: FunctionComponent = () => {
    const theme = useTheme();
    const { showSuccess, showError } = useNotification();
    const location = useLocation();
    const [tabValue, setTabValue] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);
    const [pastPage, setPastPage] = useState(1);

    const { data: currentSlotsResponse, isLoading: currentLoading } = useMyCurrentSlots(currentPage, MY_SLOTS_PAGE_SIZE);
    const { data: pastSlotsResponse, isLoading: pastLoading } = useMyPastSlots(pastPage, MY_SLOTS_PAGE_SIZE);
    const createSlotMutation = useCreateAvailableSlot();
    const deleteSlotMutation = useDeleteAvailableSlot();

    const [isModalOpen, setIsModalOpen] = useState(false);

    // Open modal if navigated from Dashboard
    useEffect(() => {
        if (location.state?.openModal) {
            setIsModalOpen(true);
        }
    }, [location]);

    useEffect(() => {
        if (deleteSlotMutation.error) {
            if (deleteSlotMutation.error instanceof ApiError) {
                showError(deleteSlotMutation.error.errors);
            } else {
                showError(deleteSlotMutation.error.message);
            }
        }
    }, [deleteSlotMutation.error, showError]);

    const handleCreateSlot = async (data: CreateAvailableSlotRequest) => {
        try {
            await createSlotMutation.mutateAsync(data);
            setIsModalOpen(false);
            showSuccess('Óraidőpont sikeresen létrehozva!');
        } catch (error) {
            if (error instanceof ApiError) {
                showError(error.errors);
            } else {
                showError('Hiba történt a létrehozás során');
            }
        }
    };

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleCurrentPageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
        setCurrentPage(value);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const handlePastPageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
        setPastPage(value);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const isLoading = tabValue === 0 ? currentLoading : pastLoading;
    const slotsResponse = tabValue === 0 ? currentSlotsResponse : pastSlotsResponse;
    const slots = slotsResponse?.items || [];
    const totalPages = slotsResponse?.totalPages || 0;
    const activePage = tabValue === 0 ? currentPage : pastPage;
    const handlePageChange = tabValue === 0 ? handleCurrentPageChange : handlePastPageChange;

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

            {/* Tabs */}
            <Tabs
                value={tabValue}
                onChange={handleTabChange}
                sx={{
                    mb: 3,
                    borderBottom: 1,
                    borderColor: 'divider'
                }}
            >
                <Tab label="Aktuális" />
                <Tab label="Korábbi" />
            </Tabs>

            {/* Loading State */}
            {isLoading && (
                <Box display="flex" justifyContent="center" alignItems="center" minHeight="300px">
                    <CircularProgress size={60} />
                </Box>
            )}

            {/* Content */}
            {!isLoading && (
                <>
                    {Object.keys(groupedSlots).length === 0 ? (
                        <Box textAlign="center" mt={4}>
                            <Typography
                                variant="h6"
                                sx={{
                                    color: theme.palette.text.secondary,
                                    mb: 2
                                }}
                            >
                                {tabValue === 0
                                    ? (slotsResponse?.totalCount === 0
                                        ? 'Még nincsenek aktuális óraidőpontjaid.'
                                        : 'Nincsenek óraidőpontok ezen az oldalon.')
                                    : 'Nincsenek korábbi óraidőpontok.'
                                }
                            </Typography>
                            {tabValue === 0 && slotsResponse?.totalCount === 0 && (
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
                                page={activePage}
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
                </>
            )}

            {/* Modal */}
            <SlotFormModal
                open={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onSubmit={handleCreateSlot}
                isLoading={createSlotMutation.isPending}
                mode="create"
            />

        </Box>
    );
};

export default MySlots;