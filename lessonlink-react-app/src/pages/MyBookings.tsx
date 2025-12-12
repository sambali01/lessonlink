import { BookmarkBorder as BookingsIcon, } from '@mui/icons-material';
import {
    Box,
    CircularProgress,
    Pagination,
    Paper,
    Stack,
    Tab,
    Tabs,
    Typography,
    useTheme
} from '@mui/material';
import { FunctionComponent, SyntheticEvent, useMemo, useState } from 'react';
import NegativeActionConfirmationModal from '../components/common/NegativeActionConfirmationModal';
import TabPanel from '../components/common/TabPanel';
import BookingCard from '../components/features/my-bookings/BookingCard';
import { useCancelBooking, useMyBookingsAsStudent } from '../hooks/bookingQueries';
import { BookingStatus } from '../models/Booking';
import { BOOKINGS_PAGE_SIZE } from '../utils/constants';
import { useNotification } from '../hooks/useNotification';
import { ApiError } from '../utils/ApiError';

const MyBookings: FunctionComponent = () => {
    const theme = useTheme();
    const { showSuccess, showError } = useNotification();
    const [tabValue, setTabValue] = useState(0);
    const [activePage, setActivePage] = useState(1);
    const [pastPage, setPastPage] = useState(1);
    const [bookingToCancel, setBookingToCancel] = useState<{ id: number; teacherName: string; time: string } | null>(null);

    const { data: bookings, isLoading, refetch } = useMyBookingsAsStudent();
    const cancelBookingMutation = useCancelBooking();

    const handleTabChange = (_event: SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
        // Reset to first page when switching tabs
        if (newValue === 0) {
            setActivePage(1);
        } else {
            setPastPage(1);
        }
    };

    const handleActivePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
        setActivePage(value);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const handlePastPageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
        setPastPage(value);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const handleCancelBooking = (bookingId: number) => {
        const booking = bookings?.find(b => b.id === bookingId);
        if (booking) {
            const startTime = new Date(booking.slotStartTime);
            const endTime = new Date(booking.slotEndTime);
            const timeString = `${startTime.toLocaleDateString('hu-HU')} ${startTime.toLocaleTimeString('hu-HU', { hour: '2-digit', minute: '2-digit' })} - ${endTime.toLocaleTimeString('hu-HU', { hour: '2-digit', minute: '2-digit' })}`;

            setBookingToCancel({
                id: bookingId,
                teacherName: booking.teacherName,
                time: timeString
            });
        }
    };

    const handleConfirmCancelBooking = async () => {
        if (bookingToCancel) {
            try {
                await cancelBookingMutation.mutateAsync(bookingToCancel.id);
                showSuccess('Foglalás sikeresen lemondva');
            } catch (error) {
                if (error instanceof ApiError) {
                    showError(error.errors);
                } else {
                    showError('Hiba történt a lemondás során');
                }
            } finally {
                setBookingToCancel(null);
                refetch();
            }
        }
    };

    const handleCancelCancelBooking = () => {
        setBookingToCancel(null);
    };

    // Filter and sort bookings by status
    const { activeBookings, pastBookings, paginatedActiveBookings, paginatedPastBookings, activeTotalPages, pastTotalPages } = useMemo(() => {
        const now = new Date();

        const active = (bookings || [])
            .filter(booking =>
                (booking.status === BookingStatus.Pending || booking.status === BookingStatus.Confirmed) &&
                new Date(booking.slotEndTime) >= now
            )
            .sort((a, b) => new Date(a.slotStartTime).getTime() - new Date(b.slotStartTime).getTime());

        const past = (bookings || [])
            .filter(booking =>
                booking.status === BookingStatus.Cancelled ||
                new Date(booking.slotEndTime) < now
            )
            .sort((a, b) => new Date(b.slotStartTime).getTime() - new Date(a.slotStartTime).getTime());

        const activeTotalPages = Math.ceil(active.length / BOOKINGS_PAGE_SIZE);
        const pastTotalPages = Math.ceil(past.length / BOOKINGS_PAGE_SIZE);

        const paginatedActive = active.slice(
            (activePage - 1) * BOOKINGS_PAGE_SIZE,
            activePage * BOOKINGS_PAGE_SIZE
        );

        const paginatedPast = past.slice(
            (pastPage - 1) * BOOKINGS_PAGE_SIZE,
            pastPage * BOOKINGS_PAGE_SIZE
        );

        return {
            activeBookings: active,
            pastBookings: past,
            paginatedActiveBookings: paginatedActive,
            paginatedPastBookings: paginatedPast,
            activeTotalPages,
            pastTotalPages
        };
    }, [bookings, activePage, pastPage]);

    return (
        <Box sx={{ p: { xs: 1, sm: 2 } }}>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 4 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
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
                        Saját foglalások
                    </Typography>
                </Box>
            </Box>

            {isLoading ? (
                <Box display="flex" justifyContent="center" alignItems="center" minHeight="300px">
                    <CircularProgress size={60} />
                </Box>
            ) : (
                <Paper sx={{
                    borderRadius: 2,
                    overflow: 'hidden',
                    boxShadow: theme.shadows[2]
                }}>
                    <Tabs
                        value={tabValue}
                        onChange={handleTabChange}
                        variant="fullWidth"
                        sx={{
                            borderBottom: 1,
                            borderColor: 'divider',
                            bgcolor: theme.palette.background.paper
                        }}
                    >
                        <Tab
                            label={`Aktív foglalások (${activeBookings.length})`}
                            sx={{ fontWeight: 'medium' }}
                        />
                        <Tab
                            label={`Korábbi foglalások (${pastBookings.length})`}
                            sx={{ fontWeight: 'medium' }}
                        />
                    </Tabs>

                    {/* Active Bookings Tab */}
                    <TabPanel value={tabValue} index={0}>
                        <Box sx={{ p: 3 }}>
                            {activeBookings.length === 0 ? (
                                <Box sx={{
                                    textAlign: 'center',
                                    py: 8,
                                    color: theme.palette.text.secondary
                                }}>
                                    <BookingsIcon sx={{ fontSize: 60, mb: 2, opacity: 0.5 }} />
                                    <Typography variant="h6" gutterBottom>
                                        Nincsenek aktív foglalások
                                    </Typography>
                                    <Typography variant="body2">
                                        Böngéssz az oktatók között és foglalj órát!
                                    </Typography>
                                </Box>
                            ) : (
                                <>
                                    <Stack spacing={3}>
                                        {paginatedActiveBookings.map((booking) => (
                                            <BookingCard
                                                key={booking.id}
                                                booking={booking}
                                                onCancel={handleCancelBooking}
                                                showCancellationButton={true}
                                                isLoading={cancelBookingMutation.isPending}
                                            />
                                        ))}
                                    </Stack>

                                    {activeTotalPages > 1 && (
                                        <Box display="flex" flexDirection="column" alignItems="center" mt={4}>
                                            <Typography
                                                variant="body2"
                                                sx={{
                                                    color: theme.palette.text.secondary,
                                                    mb: 2
                                                }}
                                            >
                                                {activeBookings.length} aktív foglalás összesen
                                            </Typography>
                                            <Pagination
                                                count={activeTotalPages}
                                                page={activePage}
                                                onChange={handleActivePageChange}
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
                        </Box>
                    </TabPanel>

                    {/* Past Bookings Tab */}
                    <TabPanel value={tabValue} index={1}>
                        <Box sx={{ p: 3 }}>
                            {pastBookings.length === 0 ? (
                                <Box sx={{
                                    textAlign: 'center',
                                    py: 8,
                                    color: theme.palette.text.secondary
                                }}>
                                    <BookingsIcon sx={{ fontSize: 60, mb: 2, opacity: 0.5 }} />
                                    <Typography variant="h6" gutterBottom>
                                        Nincsenek korábbi foglalások
                                    </Typography>
                                    <Typography variant="body2">
                                        Itt fognak megjelenni a lezárt vagy lemondott foglalások.
                                    </Typography>
                                </Box>
                            ) : (
                                <>
                                    <Stack spacing={3}>
                                        {paginatedPastBookings.map((booking) => (
                                            <BookingCard
                                                key={booking.id}
                                                booking={booking}
                                                showCancellationButton={false}
                                            />
                                        ))}
                                    </Stack>

                                    {pastTotalPages > 1 && (
                                        <Box display="flex" flexDirection="column" alignItems="center" mt={4}>
                                            <Typography
                                                variant="body2"
                                                sx={{
                                                    color: theme.palette.text.secondary,
                                                    mb: 2
                                                }}
                                            >
                                                {pastBookings.length} korábbi foglalás összesen
                                            </Typography>
                                            <Pagination
                                                count={pastTotalPages}
                                                page={pastPage}
                                                onChange={handlePastPageChange}
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
                        </Box>
                    </TabPanel>
                </Paper>
            )}

            <NegativeActionConfirmationModal
                open={!!bookingToCancel}
                onClose={handleCancelCancelBooking}
                onConfirm={handleConfirmCancelBooking}
                isLoading={cancelBookingMutation.isPending}
                title="Foglalás lemondása"
                content={bookingToCancel ? `Biztosan le szeretnéd mondani a foglalást ${bookingToCancel.teacherName} oktatóval ${bookingToCancel.time} időpontban?` : ''}
                confirmButtonText="Lemondás"
                confirmButtonLoadingText="Lemondás..."
            />
        </Box>
    );
};

export default MyBookings;