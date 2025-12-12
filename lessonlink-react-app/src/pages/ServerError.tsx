import { Box, Button, Container, Typography, useTheme } from '@mui/material';
import { FunctionComponent } from 'react';
import ReplayIcon from '@mui/icons-material/Replay';
import ErrorOutlineIcon from '@mui/icons-material/ErrorOutline';

const ServerError: FunctionComponent = () => {
    const theme = useTheme();

    const handleRetry = () => {
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
                    minHeight: 'calc(100vh - 200px)',
                    textAlign: 'center',
                    gap: 3
                }}
            >
                <ErrorOutlineIcon
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
                    Hoppá!
                </Typography>

                <Typography
                    variant="h5"
                    sx={{
                        fontWeight: 'medium',
                        color: theme.palette.text.primary,
                        mb: 1
                    }}
                >
                    Valami hiba történt
                </Typography>

                <Typography
                    variant="body1"
                    sx={{
                        color: theme.palette.text.secondary,
                        maxWidth: '500px',
                        lineHeight: 1.6
                    }}
                >
                    Hiba történt a szerver oldalon. Próbálkozz újra később!
                </Typography>

                <Button
                    variant="contained"
                    size="large"
                    startIcon={<ReplayIcon />}
                    onClick={handleRetry}
                    sx={{
                        px: 4,
                        py: 1.5,
                        textTransform: 'none',
                        fontSize: '1rem'
                    }}
                >
                    Újrapróbálás
                </Button>
            </Box>
        </Container>
    );
};

export default ServerError;
