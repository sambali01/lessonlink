import LockIcon from '@mui/icons-material/Lock';
import HomeIcon from '@mui/icons-material/Home';
import { Box, Button, Container, Typography, useTheme } from '@mui/material';
import { FunctionComponent } from 'react';
import { Link as RouterLink } from 'react-router-dom';

const PermissionDenied: FunctionComponent = () => {
    const theme = useTheme();

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
                <LockIcon
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
                    403
                </Typography>

                <Typography
                    variant="h5"
                    sx={{
                        fontWeight: 'medium',
                        color: theme.palette.text.primary,
                        mb: 1
                    }}
                >
                    Hozzáférés megtagadva
                </Typography>

                <Typography
                    variant="body1"
                    sx={{
                        color: theme.palette.text.secondary,
                        maxWidth: '500px',
                        lineHeight: 1.6
                    }}
                >
                    Nincs jogosultságod az oldal megtekintéséhez.
                </Typography>

                <Button
                    component={RouterLink}
                    to="/"
                    variant="contained"
                    size="large"
                    startIcon={<HomeIcon />}
                    sx={{
                        px: 4,
                        py: 1.5,
                        textTransform: 'none',
                        fontSize: '1rem'
                    }}
                >
                    Vissza a főoldalra
                </Button>
            </Box>
        </Container>
    );
};

export default PermissionDenied;