import React, { useState } from 'react';
import {
    Box,
    Button,
    Container,
    Typography,
    useTheme,
    CircularProgress,
    Alert,
    Paper,
    Tab,
    Tabs,
    Stack,
    Snackbar
} from '@mui/material';
import {
    BookmarkBorder as BookingsIcon,
    Refresh as RefreshIcon
} from '@mui/icons-material';
import { useMyBookingsAsStudent, useCancelBooking } from '../hooks/bookingQueries';
import BookingCard from '../components/features/booking/BookingCard';
import TabPanel from '../components/common/TabPanel';
import { BookingStatus } from '../models/Booking';

const MyBookings: React.FC = () => {
    const theme = useTheme();
    const [tabValue, setTabValue] = useState(0);
    const [snackbar, setSnackbar] = useState<{
        open: boolean;
        message: string;
        severity: 'success' | 'error'
    }>({
        open: false,
        message: '',
        severity: 'success'
    });

    const { data: bookings, isLoading, error, refetch } = useMyBookingsAsStudent();
    const cancelBookingMutation = useCancelBooking();

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleCancelBooking = async (bookingId: number) => {
        try {
            await cancelBookingMutation.mutateAsync(bookingId);
            setSnackbar({
                open: true,
                message: 'Foglalás sikeresen lemondva',
                severity: 'success'
            });
        } catch (error) {
            setSnackbar({
                open: true,
                message: error instanceof Error ? error.message : 'Hiba történt a lemondás során',
                severity: 'error'
            });
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbar(prev => ({ ...prev, open: false }));
    };

    const handleRefresh = () => {
        refetch();
    };

    // Filter bookings by status
    const activeBookings = bookings?.filter(booking =>
        booking.status === BookingStatus.Pending || booking.status === BookingStatus.Confirmed
    ) || [];

    const pastBookings = bookings?.filter(booking =>
        booking.status === BookingStatus.Cancelled || new Date(booking.slotEndTime) < new Date()
    ) || [];

    if (error) {
        return (
            <Container sx={{ textAlign: 'center', py: 8 }}>
                <Alert severity="error" sx={{ mb: 3 }}>
                    Hiba történt a foglalások betöltése közben: {error.message}
                </Alert>
                <Button variant="contained" onClick={handleRefresh}>
                    Újrapróbálkozás
                </Button>
            </Container>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ py: 4 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 4 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                    <BookingsIcon
                        sx={{
                            fontSize: 40,
                            color: theme.palette.primary.main
                        }}
                    />
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

                <Button
                    variant="outlined"
                    startIcon={<RefreshIcon />}
                    onClick={handleRefresh}
                    disabled={isLoading}
                >
                    Frissítés
                </Button>
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
                            bgcolor: theme.palette.mode === 'dark'
                                ? theme.palette.background.default
                                : '#fff'
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
                                <Stack spacing={3}>
                                    {activeBookings.map((booking) => (
                                        <BookingCard
                                            key={booking.id}
                                            booking={booking}
                                            onCancel={handleCancelBooking}
                                            showCancelButton={true}
                                            isLoading={cancelBookingMutation.isPending}
                                        />
                                    ))}
                                </Stack>
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
                                <Stack spacing={3}>
                                    {pastBookings.map((booking) => (
                                        <BookingCard
                                            key={booking.id}
                                            booking={booking}
                                            showCancelButton={false}
                                        />
                                    ))}
                                </Stack>
                            )}
                        </Box>
                    </TabPanel>
                </Paper>
            )}

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
        </Container>
    );
};

export default MyBookings;