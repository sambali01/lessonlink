import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Typography
} from '@mui/material';

interface DeleteConfirmationModalProps {
    open: boolean;
    onClose: () => void;
    onConfirm: () => void;
    slotTime: string;
    isLoading: boolean;
}

const DeleteConfirmationModal = ({
    open,
    onClose,
    onConfirm,
    slotTime,
    isLoading
}: DeleteConfirmationModalProps) => {
    return (
        <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle>
                Időpont törlése
            </DialogTitle>
            <DialogContent>
                <Typography>
                    Biztosan törölni szeretnéd a(z) "{slotTime}" időpontot?
                </Typography>
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose} disabled={isLoading}>
                    Mégse
                </Button>
                <Button
                    onClick={onConfirm}
                    variant="contained"
                    color="error"
                    disabled={isLoading}
                >
                    {isLoading ? 'Törlés...' : 'Törlés'}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default DeleteConfirmationModal;