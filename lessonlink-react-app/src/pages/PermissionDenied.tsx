import LockIcon from '@mui/icons-material/Lock';
import { Box, Button, Container, Typography } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';

const PermissionDenied = () => {
    return (
        <Container maxWidth="sm">
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                    minHeight: 'calc(100vh - 200px)',
                    textAlign: 'center',
                    gap: 3
                }}
            >
                <LockIcon sx={{ fontSize: 60, color: 'error.main' }} />
                <Typography variant="h4" color="error.main" gutterBottom>
                    Hozzáférés megtagadva
                </Typography>
                <Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
                    Nincs jogosultságod az oldal megtekintéséhez.
                </Typography>
                <Button
                    component={RouterLink}
                    to="/"
                    variant="contained"
                    sx={{ textTransform: 'none' }}
                >
                    Vissza a főoldalra
                </Button>
            </Box>
        </Container>
    )
}

export default PermissionDenied