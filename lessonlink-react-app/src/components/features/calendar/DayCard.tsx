import {
    Card,
    CardContent,
    Typography,
    Box,
    Chip,
    IconButton,
    Tooltip,
    useTheme
} from '@mui/material';
import { Delete as DeleteIcon, Visibility as VisibilityIcon } from '@mui/icons-material';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AvailableSlot } from '../../../models/AvailableSlot';
import { useDeleteAvailableSlot } from '../../../hooks/avaliableSlotQueries';
import { BookingStatus } from '../../../models/Booking';
import DeleteConfirmationModal from '../../common/DeleteConfirmationModal';

interface DayCardProps {
    date: string;
    slots: AvailableSlot[];
}

const DayCard = ({ date, slots }: DayCardProps) => {
    const theme = useTheme();
    const navigate = useNavigate();
    const deleteSlotMutation = useDeleteAvailableSlot();
    const [deletingSlotId, setDeletingSlotId] = useState<number | null>(null);
    const [slotToDelete, setSlotToDelete] = useState<{ id: number; time: string } | null>(null);

    const formattedDate = new Date(date).toLocaleDateString('hu-HU', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });

    const formatTime = (dateTime: string) => {
        return new Date(dateTime).toLocaleTimeString('hu-HU', {
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    const hasActiveBooking = (slot: AvailableSlot) => {
        return slot.bookings && slot.bookings.some(booking =>
            booking.status === BookingStatus.Pending || booking.status === BookingStatus.Confirmed
        );
    };

    const handleSlotClick = (slot: AvailableSlot) => {
        navigate(`/teacher/slots/${slot.id}/details`);
    };

    const handleDeleteClick = (slot: AvailableSlot) => {
        const slotTime = `${formatTime(slot.startTime)} - ${formatTime(slot.endTime)}`;
        setSlotToDelete({ id: slot.id, time: slotTime });
    };

    const handleConfirmDelete = async () => {
        if (slotToDelete) {
            await handleDeleteSlot(slotToDelete.id);
            setSlotToDelete(null);
        }
    };

    const handleCancelDelete = () => {
        setSlotToDelete(null);
    };

    const handleDeleteSlot = async (slotId: number) => {
        setDeletingSlotId(slotId);
        try {
            await deleteSlotMutation.mutateAsync(slotId);
        } catch (error) {
            console.error('Hiba a törlés során:', error);
        } finally {
            setDeletingSlotId(null);
        }
    };

    return (
        <>
            <Card
                sx={{
                    height: '100%',
                    display: 'flex',
                    flexDirection: 'column',
                    transition: 'transform 0.2s, box-shadow 0.2s',
                    '&:hover': {
                        transform: 'translateY(-4px)',
                        boxShadow: theme.shadows[4]
                    }
                }}
            >
                <CardContent sx={{ flexGrow: 1, p: 2 }}>
                    <Typography
                        variant="h6"
                        component="h3"
                        gutterBottom
                        sx={{
                            color: theme.palette.primary.main,
                            fontSize: '1.1rem',
                            fontWeight: 600
                        }}
                    >
                        {formattedDate}
                    </Typography>

                    <Box>
                        {slots
                            .sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime())
                            .map((slot) => {
                                const isBooked = hasActiveBooking(slot);
                                return (
                                    <Box
                                        key={slot.id}
                                        sx={{
                                            display: 'flex',
                                            alignItems: 'center',
                                            mb: 1,
                                            '&:last-child': { mb: 0 }
                                        }}
                                    >
                                        <Chip
                                            label={`${formatTime(slot.startTime)} - ${formatTime(slot.endTime)}`}
                                            onClick={() => handleSlotClick(slot)}
                                            sx={{
                                                flexGrow: 1,
                                                cursor: 'pointer',
                                                backgroundColor: isBooked
                                                    ? theme.palette.success.light
                                                    : theme.palette.primary.light,
                                                color: isBooked
                                                    ? theme.palette.getContrastText(theme.palette.success.light)
                                                    : theme.palette.getContrastText(theme.palette.primary.light),
                                                '&:hover': {
                                                    backgroundColor: isBooked
                                                        ? theme.palette.success.main
                                                        : theme.palette.primary.main,
                                                    transform: 'scale(1.02)'
                                                },
                                                transition: 'all 0.2s ease-in-out'
                                            }}
                                        />

                                        {isBooked && (
                                            <Tooltip title="Időpont megtekintése">
                                                <IconButton
                                                    size="small"
                                                    onClick={() => handleSlotClick(slot)}
                                                    sx={{
                                                        ml: 1,
                                                        color: theme.palette.info.main,
                                                        '&:hover': {
                                                            backgroundColor: theme.palette.info.light,
                                                            color: theme.palette.info.contrastText
                                                        }
                                                    }}
                                                >
                                                    <VisibilityIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                        )}

                                        <Tooltip title="Időpont törlése">
                                            <IconButton
                                                size="small"
                                                onClick={() => handleDeleteClick(slot)}
                                                disabled={deletingSlotId === slot.id || isBooked}
                                                sx={{
                                                    ml: 1,
                                                    color: isBooked
                                                        ? theme.palette.grey[400]
                                                        : theme.palette.error.main,
                                                    '&:hover': !isBooked ? {
                                                        backgroundColor: theme.palette.error.light,
                                                        color: theme.palette.error.contrastText
                                                    } : {}
                                                }}
                                            >
                                                <DeleteIcon fontSize="small" />
                                            </IconButton>
                                        </Tooltip>
                                    </Box>
                                );
                            })}
                    </Box>

                    <Typography
                        variant="body2"
                        sx={{
                            mt: 1,
                            color: theme.palette.text.secondary,
                            fontStyle: 'italic'
                        }}
                    >
                        {slots.length} óraidőpont
                    </Typography>
                </CardContent>
            </Card>
            <DeleteConfirmationModal
                open={!!slotToDelete}
                onClose={handleCancelDelete}
                onConfirm={handleConfirmDelete}
                slotTime={slotToDelete?.time || ''}
                isLoading={deletingSlotId === slotToDelete?.id} />
        </>
    );
};

export default DayCard;