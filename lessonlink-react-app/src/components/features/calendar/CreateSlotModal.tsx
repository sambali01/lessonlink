import {
    Alert,
    Box,
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Typography
} from '@mui/material';
import { DateTimePicker, LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { hu } from 'date-fns/locale';
import { useState } from 'react';
import { CreateAvailableSlotRequest } from '../../../models/AvailableSlot';

interface CreateSlotModalProps {
    open: boolean;
    onClose: () => void;
    onCreate: (data: CreateAvailableSlotRequest) => void;
    isLoading: boolean;
    error?: string;
}

const CreateSlotModal = ({ open, onClose, onCreate, isLoading, error }: CreateSlotModalProps) => {
    const [startTime, setStartTime] = useState<Date | null>(null);
    const [endTime, setEndTime] = useState<Date | null>(null);

    const handleSubmit = () => {
        if (!startTime || !endTime) {
            return;
        }

        onCreate({
            startTime: startTime.toISOString(),
            endTime: endTime.toISOString()
        });
    };

    const handleClose = () => {
        setStartTime(null);
        setEndTime(null);
        onClose();
    };

    return (
        <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={hu}>
            <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
                <DialogTitle>
                    Új óraidőpont hozzáadása
                </DialogTitle>
                <DialogContent>
                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
                        {error && (
                            <Alert severity="error">{error}</Alert>
                        )}

                        <DateTimePicker
                            label="Kezdési idő"
                            value={startTime}
                            onChange={(newValue) => setStartTime(newValue)}
                            minDateTime={new Date()}
                            slotProps={{
                                textField: {
                                    fullWidth: true,
                                    required: true
                                }
                            }}
                        />

                        <DateTimePicker
                            label="Befejezési idő"
                            value={endTime}
                            onChange={(newValue) => setEndTime(newValue)}
                            minDateTime={startTime || new Date()}
                            slotProps={{
                                textField: {
                                    fullWidth: true,
                                    required: true
                                }
                            }}
                        />

                        {startTime && endTime && (
                            <Typography variant="body2" color="text.secondary">
                                Időtartam: {Math.round((endTime.getTime() - startTime.getTime()) / (1000 * 60))} perc
                            </Typography>
                        )}
                    </Box>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleClose} disabled={isLoading}>
                        Mégse
                    </Button>
                    <Button
                        onClick={handleSubmit}
                        variant="contained"
                        disabled={!startTime || !endTime || isLoading}
                    >
                        {isLoading ? 'Létrehozás...' : 'Létrehozás'}
                    </Button>
                </DialogActions>
            </Dialog>
        </LocalizationProvider>
    );
};

export default CreateSlotModal;