import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Typography
} from '@mui/material';
import { FunctionComponent } from 'react';

interface NegativeActionConfirmationModalProps {
    open: boolean;
    onClose: () => void;
    onConfirm: () => void;
    isLoading: boolean;
    title: string;
    content: string;
    confirmButtonText: string;
    confirmButtonLoadingText: string;
}

const NegativeActionConfirmationModal: FunctionComponent<NegativeActionConfirmationModalProps> = ({
    open,
    onClose,
    onConfirm,
    isLoading,
    title,
    content,
    confirmButtonText,
    confirmButtonLoadingText
}) => {
    return (
        <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle>
                {title}
            </DialogTitle>
            <DialogContent>
                <Typography>
                    {content}
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
                    {isLoading ? confirmButtonLoadingText : confirmButtonText}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default NegativeActionConfirmationModal;