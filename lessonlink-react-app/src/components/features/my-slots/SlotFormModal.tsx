import {
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
import { useEffect, useState } from 'react';
import { CreateAvailableSlotRequest } from '../../../models/AvailableSlot';

interface SlotFormModalProps {
    open: boolean;
    onClose: () => void;
    onSubmit: (data: CreateAvailableSlotRequest) => void;
    isLoading: boolean;
    error?: string;
    mode?: 'create' | 'edit';
    initialStartTime?: string;
    initialEndTime?: string;
}

const SlotFormModal = ({
    open,
    onClose,
    onSubmit,
    isLoading,
    mode = 'create',
    initialStartTime,
    initialEndTime
}: SlotFormModalProps) => {
    const [startTime, setStartTime] = useState<Date | null>(null);
    const [endTime, setEndTime] = useState<Date | null>(null);

    useEffect(() => {
        if (open) {
            if (mode === 'edit' && initialStartTime && initialEndTime) {
                setStartTime(new Date(initialStartTime));
                setEndTime(new Date(initialEndTime));
            } else {
                setStartTime(null);
                setEndTime(null);
            }
        }
    }, [open, mode, initialStartTime, initialEndTime]);

    const handleSubmit = () => {
        if (!startTime || !endTime) {
            return;
        }

        onSubmit({
            startTime: startTime.toISOString(),
            endTime: endTime.toISOString()
        });
    };

    const handleClose = () => {
        setStartTime(null);
        setEndTime(null);
        onClose();
    };

    const title = mode === 'create' ? 'Új óraidőpont hozzáadása' : 'Időpont szerkesztése';
    const submitButtonText = mode === 'create' ? 'Létrehozás' : 'Mentés';
    const submitButtonLoadingText = mode === 'create' ? 'Létrehozás...' : 'Mentés...';

    return (
        <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={hu}>
            <Dialog open={open} onClose={handleClose} maxWidth="sm" fullWidth>
                <DialogTitle>
                    {title}
                </DialogTitle>
                <DialogContent>
                    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
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
                        {isLoading ? submitButtonLoadingText : submitButtonText}
                    </Button>
                </DialogActions>
            </Dialog>
        </LocalizationProvider>
    );
};

export default SlotFormModal;