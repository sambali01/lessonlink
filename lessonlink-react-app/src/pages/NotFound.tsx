import { Box, Button, Container, Typography, useTheme } from '@mui/material';
import { FunctionComponent } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import HomeIcon from '@mui/icons-material/Home';
import SearchOffIcon from '@mui/icons-material/SearchOff';

const NotFound: FunctionComponent = () => {
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
                <SearchOffIcon
                    sx={{
                        fontSize: { xs: '5rem', md: '8rem' },
                        color: theme.palette.primary.main,
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
                    404
                </Typography>

                <Typography
                    variant="h5"
                    sx={{
                        fontWeight: 'medium',
                        color: theme.palette.text.primary,
                        mb: 1
                    }}
                >
                    A manóba!
                </Typography>

                <Typography
                    variant="body1"
                    sx={{
                        color: theme.palette.text.secondary,
                        lineHeight: 1.6
                    }}
                >
                    A keresett oldal nem található. Lehet, hogy az URL hibás, vagy az oldal nem létezik.
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

export default NotFound;