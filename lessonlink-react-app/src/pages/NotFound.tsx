import { Box, Button, Container, Typography, useTheme } from '@mui/material';
import { FunctionComponent } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import HomeIcon from '@mui/icons-material/Home';

const NotFound: FunctionComponent = () => {
    const theme = useTheme();

    return (
        <Container >
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
                <Typography
                    variant="h1"
                    sx={{
                        fontSize: { xs: '4rem', md: '6rem' },
                        fontWeight: 'bold',
                        color: theme.palette.primary.main,
                        textShadow: '2px 2px 4px rgba(0,0,0,0.1)'
                    }}
                >
                    404
                </Typography>

                <Typography
                    variant="h4"
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
                        mt: 2,
                        px: 4,
                        py: 1.5,
                        borderRadius: 2,
                        textTransform: 'none',
                        fontSize: '1.1rem'
                    }}
                >
                    Vissza a főoldalra
                </Button>
            </Box>
        </Container>
    );
};

export default NotFound;