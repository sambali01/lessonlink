import { Box, Button, Container, Typography, useTheme } from '@mui/material';
import { FunctionComponent } from 'react';
import ReplayIcon from '@mui/icons-material/Replay';
import BugReportIcon from '@mui/icons-material/BugReport';

const ErrorFallback: FunctionComponent = () => {
    const theme = useTheme();

    const handleReload = () => {
        window.location.href = '/';
    };

    return (
        <Container>
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                    minHeight: '100vh',
                    textAlign: 'center',
                    gap: 3
                }}
            >
                <BugReportIcon
                    sx={{
                        fontSize: { xs: '5rem', md: '8rem' },
                        color: theme.palette.error.main,
                        opacity: 0.8
                    }}
                />

                <Typography
                    variant="h1"
                    sx={{
                        fontSize: { xs: '3rem', md: '4rem' },
                        fontWeight: 'bold',
                        color: theme.palette.text.primary,
                    }}
                >
                    Váratlan hiba
                </Typography>

                <Typography
                    variant="h5"
                    sx={{
                        fontWeight: 'medium',
                        color: theme.palette.text.primary,
                        mb: 1
                    }}
                >
                    Sajnos valami hiba történt az alkalmazás betöltése során
                </Typography>

                <Typography
                    variant="body1"
                    sx={{
                        color: theme.palette.text.secondary,
                        maxWidth: '500px',
                        lineHeight: 1.6
                    }}
                >
                    Ez általában átmeneti probléma. Próbáld újratölteni az oldalt.
                </Typography>

                <Button
                    variant="contained"
                    size="large"
                    startIcon={<ReplayIcon />}
                    onClick={handleReload}
                    sx={{
                        px: 4,
                        py: 1.5,
                        textTransform: 'none',
                        fontSize: '1rem'
                    }}
                >
                    Oldal újratöltése
                </Button>
            </Box>
        </Container>
    );
};

export default ErrorFallback;
