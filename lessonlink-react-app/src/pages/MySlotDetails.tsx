import {
    ArrowBack as ArrowBackIcon,
    Check as CheckIcon,
    Close as CloseIcon,
    Delete as DeleteIcon,
    Edit as EditIcon,
    Event as EventIcon,
    Person as PersonIcon,
    AccessTime as TimeIcon
} from '@mui/icons-material';
import {
    Alert,
    Box,
    Button,
    Card,
    CardContent,
    Chip,
    CircularProgress,
    Divider,
    Paper,
    Typography,
    useTheme
} from '@mui/material';
import { FunctionComponent, useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import NegativeActionConfirmationModal from '../components/common/NegativeActionConfirmationModal';
import SlotFormModal from '../components/features/my-slots/SlotFormModal';
import { useAvailableSlotDetails, useDeleteAvailableSlot, useUpdateAvailableSlot } from '../hooks/avaliableSlotQueries';
import { useDecideBookingAcceptance } from '../hooks/bookingQueries';
import { useNotification } from '../hooks/useNotification';
import { CreateAvailableSlotRequest } from '../models/AvailableSlot';
import { BookingStatus } from '../models/Booking';
import { ApiError } from '../utils/ApiError';

const MySlotDetails: FunctionComponent = () => {
    const { slotId } = useParams<{ slotId: string }>();
    const navigate = useNavigate();
    const theme = useTheme();
    const { showSuccess, showError } = useNotification();

    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [editDialogOpen, setEditDialogOpen] = useState(false);
    const [isDeleting, setIsDeleting] = useState(false);

    const { data: slot, isLoading } = useAvailableSlotDetails(Number(slotId), !isDeleting);
    const decideBookingAcceptanceMutation = useDecideBookingAcceptance();
    const deleteSlotMutation = useDeleteAvailableSlot();
    const updateSlotMutation = useUpdateAvailableSlot();

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
            }),
            fullDateTime: date.toLocaleString('hu-HU', {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
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
                return 'Jóváhagyva';
            case BookingStatus.Cancelled:
                return 'Lemondva';
            default:
                return 'Ismeretlen';
        }
    };

    const activeBooking = slot?.bookings.find(
        b => b.status === BookingStatus.Pending || b.status === BookingStatus.Confirmed
    );

    const cancelledBookings = slot?.bookings
        .filter(b => b.status === BookingStatus.Cancelled)
        .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());

    const hasActiveBooking = !!activeBooking;

    // Check if the slot is in the past
    const isPastSlot = slot ? new Date(slot.endTime) < new Date() : false;

    const handleAcceptBooking = async () => {
        if (!activeBooking) return;

        try {
            await decideBookingAcceptanceMutation.mutateAsync({
                bookingId: activeBooking.id,
                data: { status: BookingStatus.Confirmed }
            });

            showSuccess('Foglalás sikeresen jóváhagyva!');
        } catch (error) {
            if (error instanceof ApiError) {
                showError(error.errors);
            } else {
                showError('Hiba történt a foglalás jóváhagyása során');
            }
        }
    };

    const handleRejectBooking = async () => {
        if (!activeBooking) return;

        try {
            await decideBookingAcceptanceMutation.mutateAsync({
                bookingId: activeBooking.id,
                data: { status: BookingStatus.Cancelled }
            });

            showSuccess('Foglalás sikeresen lemondva!');
        } catch (error) {
            if (error instanceof ApiError) {
                showError(error.errors);
            } else {
                showError('Hiba történt a foglalás lemondása során');
            }
        }
    };

    const handleOpenEditDialog = () => {
        setEditDialogOpen(true);
    };

    const handleUpdateSlot = async (data: CreateAvailableSlotRequest) => {
        if (!slot) return;

        try {
            await updateSlotMutation.mutateAsync({
                slotId: slot.id,
                data
            });

            setEditDialogOpen(false);
            showSuccess('Időpont sikeresen frissítve!');
        } catch (error) {
            if (error instanceof ApiError) {
                showError(error.errors);
            } else {
                showError('Hiba történt a frissítés során');
            }
        }
    };

    const handleDeleteSlot = async () => {
        if (!slot) return;

        try {
            setIsDeleting(true);
            await deleteSlotMutation.mutateAsync(slot.id);
            showSuccess('Időpont sikeresen törölve!');
            navigate('/my-slots', { replace: true });
        } catch (error) {
            setIsDeleting(false);
            if (error instanceof ApiError) {
                showError(error.errors);
            } else {
                showError('Hiba történt a törlés során');
            }
            setDeleteDialogOpen(false);
        }
    };

    // Navigate to not-found if slot doesn't exist
    useEffect(() => {
        if (!slot && !isLoading) {
            navigate('/not-found');
        }
    }, [slot, isLoading, navigate]);

    if (isLoading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
                <CircularProgress size={60} />
            </Box>
        );
    }

    if (!slot) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
                <CircularProgress size={60} />
            </Box>
        );
    }

    const startDateTime = formatDateTime(slot.startTime);
    const endDateTime = formatDateTime(slot.endTime);

    return (
        <Box sx={{
            minHeight: '100vh',
            bgcolor: theme.palette.mode === 'dark' ? theme.palette.background.default : '#f5f5f5',
            py: 4
        }}>
            <Box sx={{
                maxWidth: '1400px',
                mx: 'auto',
                px: { xs: 2, sm: 3, md: 4 }
            }}>
                {/* Header */}
                <Box sx={{ mb: 4, display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 2 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                        <Button
                            startIcon={<ArrowBackIcon />}
                            onClick={() => navigate('/my-slots')}
                            variant="outlined"
                        >
                            Vissza
                        </Button>
                        <Typography
                            variant="h4"
                            component="h1"
                            sx={{
                                color: theme.palette.text.primary,
                                fontWeight: 500
                            }}
                        >
                            Óraidőpont részletei
                        </Typography>
                    </Box>

                    {/* Action Buttons */}
                    {!hasActiveBooking && !isPastSlot && (
                        <Box sx={{ display: 'flex', gap: 1 }}>
                            <Button
                                startIcon={<EditIcon />}
                                onClick={handleOpenEditDialog}
                                variant="outlined"
                                color="primary"
                            >
                                Szerkesztés
                            </Button>
                            <Button
                                startIcon={<DeleteIcon />}
                                onClick={() => setDeleteDialogOpen(true)}
                                variant="outlined"
                                color="error"
                            >
                                Törlés
                            </Button>
                        </Box>
                    )}
                </Box>

                <Box
                    sx={{
                        display: 'grid',
                        gridTemplateColumns: { xs: '1fr', lg: '1fr 1.5fr' },
                        gap: 3
                    }}
                >
                    {/* Slot Details Card */}
                    <Box>
                        <Card>
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

                                {hasActiveBooking && (
                                    <Alert severity="info" sx={{ mt: 3 }}>
                                        Ez az időpont nem törölhető és nem szerkeszthető, mert aktív foglalás van rajta.
                                    </Alert>
                                )}
                            </CardContent>
                        </Card>
                    </Box>

                    {/* Bookings Section */}
                    <Box>
                        {/* Active Booking Card */}
                        {activeBooking ? (
                            <Card sx={{ mb: 3 }}>
                                <CardContent>
                                    <Typography
                                        variant="h6"
                                        gutterBottom
                                        sx={{
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: 1,
                                            color: theme.palette.success.main,
                                            mb: 3
                                        }}
                                    >
                                        <PersonIcon />
                                        Aktív foglalás
                                    </Typography>

                                    <Box sx={{ mb: 2 }}>
                                        <Typography variant="body2" color="text.secondary" gutterBottom>
                                            Diák neve
                                        </Typography>
                                        <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                            {activeBooking.studentName}
                                        </Typography>
                                    </Box>

                                    <Box sx={{ mb: 2 }}>
                                        <Typography variant="body2" color="text.secondary" gutterBottom>
                                            Jelentkezés ideje
                                        </Typography>
                                        <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                            {formatDateTime(activeBooking.createdAt).fullDateTime}
                                        </Typography>
                                    </Box>

                                    <Box sx={{ mb: 2 }}>
                                        <Typography variant="body2" color="text.secondary" gutterBottom>
                                            Állapot
                                        </Typography>
                                        <Chip
                                            label={getStatusLabel(activeBooking.status)}
                                            color={getStatusColor(activeBooking.status)}
                                            size="small"
                                        />
                                    </Box>

                                    <Divider sx={{ my: 2 }} />

                                    {/* Action Buttons for Active Booking */}
                                    {activeBooking.status === BookingStatus.Pending && !isPastSlot && (
                                        <Box sx={{ display: 'flex', gap: 1 }}>
                                            <Button
                                                variant="contained"
                                                color="success"
                                                startIcon={<CheckIcon />}
                                                onClick={handleAcceptBooking}
                                                disabled={decideBookingAcceptanceMutation.isPending}
                                                fullWidth
                                                sx={{ textTransform: 'none' }}
                                            >
                                                Jóváhagyás
                                            </Button>
                                            <Button
                                                variant="outlined"
                                                color="error"
                                                startIcon={<CloseIcon />}
                                                onClick={handleRejectBooking}
                                                disabled={decideBookingAcceptanceMutation.isPending}
                                                fullWidth
                                                sx={{ textTransform: 'none' }}
                                            >
                                                Lemondás
                                            </Button>
                                        </Box>
                                    )}
                                </CardContent>
                            </Card>
                        ) : (
                            <Paper sx={{ p: 4, textAlign: 'center', mb: 3 }}>
                                <PersonIcon sx={{ fontSize: 60, color: theme.palette.text.secondary, mb: 2, opacity: 0.5 }} />
                                <Typography variant="h6" color="text.secondary" gutterBottom>
                                    Nincs aktív foglalás
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    Erre az időpontra jelenleg nincs érvényes foglalás.
                                </Typography>
                            </Paper>
                        )}

                        {/* Cancelled Bookings */}
                        {cancelledBookings && cancelledBookings.length > 0 && (
                            <Card>
                                <CardContent>
                                    <Typography
                                        variant="h6"
                                        gutterBottom
                                        sx={{
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: 1,
                                            color: theme.palette.text.primary,
                                            mb: 3
                                        }}
                                    >
                                        Korábbi foglalások
                                    </Typography>

                                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
                                        {cancelledBookings.map((booking) => (
                                            <Paper
                                                key={booking.id}
                                                sx={{
                                                    p: 2,
                                                    bgcolor: theme.palette.mode === 'dark'
                                                        ? 'rgba(255, 255, 255, 0.05)'
                                                        : 'rgba(0, 0, 0, 0.02)',
                                                    border: '1px solid',
                                                    borderColor: 'divider'
                                                }}
                                            >
                                                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
                                                    <Typography variant="body1" sx={{ fontWeight: 500 }}>
                                                        {booking.studentName}
                                                    </Typography>
                                                    <Chip
                                                        label="Lemondva"
                                                        color="error"
                                                        size="small"
                                                    />
                                                </Box>
                                                <Typography variant="body2" color="text.secondary">
                                                    Jelentkezés: {formatDateTime(booking.createdAt).fullDateTime}
                                                </Typography>
                                            </Paper>
                                        ))}
                                    </Box>
                                </CardContent>
                            </Card>
                        )}
                    </Box>
                </Box>
            </Box>

            {/* Edit Dialog */}
            <SlotFormModal
                open={editDialogOpen}
                onClose={() => setEditDialogOpen(false)}
                onSubmit={handleUpdateSlot}
                isLoading={updateSlotMutation.isPending}
                mode="edit"
                initialStartTime={slot?.startTime}
                initialEndTime={slot?.endTime}
            />

            {/* Delete Confirmation Modal */}
            <NegativeActionConfirmationModal
                open={deleteDialogOpen}
                onClose={() => setDeleteDialogOpen(false)}
                onConfirm={handleDeleteSlot}
                isLoading={deleteSlotMutation.isPending}
                title="Időpont törlése"
                content="Biztosan törölni szeretnéd ezt az időpontot? Ez a művelet nem vonható vissza."
                confirmButtonText="Törlés"
                confirmButtonLoadingText="Törlés..."
            />

        </Box>
    );
};

export default MySlotDetails;
