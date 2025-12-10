import {
    Box,
    Typography,
    Card,
    CardContent,
    Chip,
    Button,
    Alert,
    CircularProgress,
    Divider,
    useTheme,
    Snackbar
} from '@mui/material';
import {
    AccessTime as TimeIcon,
    Person as PersonIcon,
    Check as CheckIcon,
    ArrowBack as ArrowBackIcon,
    Event as EventIcon
} from '@mui/icons-material';
import { useParams, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { useAvailableSlotDetails } from '../hooks/avaliableSlotQueries';
import { useDecideBookingAcceptance } from '../hooks/bookingQueries';
import { BookingStatus } from '../models/Booking';

const BookingDetails = () => {
    const { slotId } = useParams<{ slotId: string }>();
    const navigate = useNavigate();
    const theme = useTheme();

    const { data: slot, isLoading, error } = useAvailableSlotDetails(Number(slotId));
    const decideBookingAcceptanceMutation = useDecideBookingAcceptance();

    const [snackbar, setSnackbar] = useState<{
        open: boolean;
        message: string;
        severity: 'success' | 'error'
    }>({
        open: false,
        message: '',
        severity: 'success'
    });

    const formatDateTime = (dateTime: string) => {
        const date = new Date(dateTime);
        return {
            date: date.toLocaleDateString('hu-HU', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            }),
            time: date.toLocaleTimeString('hu-HU', {
                hour: '2-digit',
                minute: '2-digit'
            })
        };
    };

    const getStatusColor = (status: BookingStatus) => {
        switch (status) {
            case BookingStatus.Pending:
                return 'warning';
            case BookingStatus.Confirmed:
                return 'success';
            case BookingStatus.Cancelled:
                return 'error';
            default:
                return 'default';
        }
    };

    const getStatusLabel = (status: BookingStatus) => {
        switch (status) {
            case BookingStatus.Pending:
                return 'Függőben';
            case BookingStatus.Confirmed:
                return 'Elfogadva';
            case BookingStatus.Cancelled:
                return 'Lemondva';
            default:
                return 'Ismeretlen';
        }
    };

    const handleAcceptBooking = async () => {
        if (!slot || slot.bookings.length === 0) return;

        try {
            await decideBookingAcceptanceMutation.mutateAsync({
                bookingId: slot.bookings[0].id,
                data: { status: BookingStatus.Confirmed }
            });

            setSnackbar({
                open: true,
                message: 'Foglalás sikeresen elfogadva!',
                severity: 'success'
            });
        } catch (error) {
            setSnackbar({
                open: true,
                message: error instanceof Error ? error.message : 'Hiba történt a foglalás elfogadása során',
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

    if (error || !slot) {
        return (
            <Box sx={{ p: 2 }}>
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error?.message || 'Az óraidőpont nem található'}
                </Alert>
                <Button
                    startIcon={<ArrowBackIcon />}
                    onClick={() => navigate('/my-slots')}
                    variant="outlined"
                >
                    Vissza az óraidőpontokhoz
                </Button>
            </Box>
        );
    }

    const startDateTime = formatDateTime(slot.startTime);
    const endDateTime = formatDateTime(slot.endTime);

    return (
        <Box sx={{ p: { xs: 1, sm: 2, md: 3 }, maxWidth: 1200, mx: 'auto' }}>
            {/* Header */}
            <Box sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 2 }}>
                <Button
                    startIcon={<ArrowBackIcon />}
                    onClick={() => navigate('/my-slots')}
                    variant="outlined"
                    size="small"
                >
                    Vissza
                </Button>
                <Typography
                    variant="h4"
                    component="h1"
                    sx={{
                        color: theme.palette.text.primary,
                        fontWeight: 500,
                        flexGrow: 1
                    }}
                >
                    Óraidőpont részletei
                </Typography>
            </Box>

            <Box
                sx={{
                    display: 'grid',
                    gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' },
                    gap: 3
                }}
            >
                {/* Slot Details Card */}
                <Box>
                    <Card sx={{ height: 'fit-content' }}>
                        <CardContent>
                            <Typography
                                variant="h6"
                                gutterBottom
                                sx={{
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: 1,
                                    color: theme.palette.primary.main,
                                    mb: 3
                                }}
                            >
                                <EventIcon />
                                Időpont információk
                            </Typography>

                            <Box sx={{ mb: 2 }}>
                                <Typography variant="body2" color="text.secondary" gutterBottom>
                                    Dátum
                                </Typography>
                                <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                    {startDateTime.date}
                                </Typography>
                            </Box>

                            <Box sx={{ mb: 2 }}>
                                <Typography variant="body2" color="text.secondary" gutterBottom>
                                    Időtartam
                                </Typography>
                                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                    <TimeIcon fontSize="small" color="action" />
                                    <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                        {startDateTime.time} - {endDateTime.time}
                                    </Typography>
                                </Box>
                            </Box>
                        </CardContent>
                    </Card>
                </Box>

                {/* Booking Details Card */}
                <Box>
                    <Card sx={{ height: 'fit-content' }}>
                        <CardContent>
                            <Typography
                                variant="h6"
                                gutterBottom
                                sx={{
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: 1,
                                    color: theme.palette.primary.main,
                                    mb: 3
                                }}
                            >
                                <PersonIcon />
                                Jelentkező
                            </Typography>

                            {slot.bookings[0] ? (
                                <>
                                    <Box sx={{ mb: 2 }}>
                                        <Typography variant="body2" color="text.secondary" gutterBottom>
                                            Diák neve
                                        </Typography>
                                        <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                            {slot.bookings[0].studentName}
                                        </Typography>
                                    </Box>

                                    <Box sx={{ mb: 2 }}>
                                        <Typography variant="body2" color="text.secondary" gutterBottom>
                                            Jelentkezés ideje
                                        </Typography>
                                        <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                            {new Date(slot.bookings[0].createdAt).toLocaleDateString('hu-HU', {
                                                year: 'numeric',
                                                month: 'long',
                                                day: 'numeric',
                                                hour: '2-digit',
                                                minute: '2-digit'
                                            })}
                                        </Typography>
                                    </Box>

                                    <Box sx={{ mb: 2 }}>
                                        <Typography variant="body2" color="text.secondary" gutterBottom>
                                            Állapot
                                        </Typography>
                                        <Chip
                                            label={getStatusLabel(slot.bookings[0].status)}
                                            color={getStatusColor(slot.bookings[0].status)}
                                            size="small"
                                        />
                                    </Box>

                                    <Divider sx={{ my: 2 }} />

                                    {/* Accept Button */}
                                    {slot.bookings[0].status === BookingStatus.Pending && (
                                        <Button
                                            variant="contained"
                                            color="success"
                                            startIcon={<CheckIcon />}
                                            onClick={handleAcceptBooking}
                                            disabled={decideBookingAcceptanceMutation.isPending}
                                            fullWidth
                                            sx={{ textTransform: 'none' }}
                                        >
                                            {decideBookingAcceptanceMutation.isPending ? 'Elfogadás...' : 'Foglalás elfogadása'}
                                        </Button>
                                    )}
                                </>
                            ) : (
                                <Box
                                    sx={{
                                        textAlign: 'center',
                                        py: 4,
                                        color: theme.palette.text.secondary
                                    }}
                                >
                                    <PersonIcon sx={{ fontSize: 48, mb: 2, opacity: 0.5 }} />
                                    <Typography variant="h6" gutterBottom>
                                        Nincs jelentkező
                                    </Typography>
                                    <Typography variant="body2">
                                        Erre az időpontra még nem jelentkezett senki.
                                    </Typography>
                                </Box>
                            )}
                        </CardContent>
                    </Card>
                </Box>
            </Box>

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

export default BookingDetails;