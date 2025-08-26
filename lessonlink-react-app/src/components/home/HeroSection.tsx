import { Box, Typography, Button, Container, useTheme } from "@mui/material";
import { FunctionComponent } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";

const HeroSection: FunctionComponent = () => {
    const theme = useTheme();
    const navigate = useNavigate();
    const { currentUserAuth } = useAuth();

    return (
        <Box sx={{
            pt: 8,
            pb: 6,
            bgcolor: theme.palette.mode === 'dark'
                ? theme.palette.primary.dark
                : theme.palette.primary.light,
            color: theme.palette.getContrastText(
                theme.palette.mode === 'dark'
                    ? theme.palette.primary.dark
                    : theme.palette.primary.light
            )
        }}>
            <Container maxWidth="md">
                <Typography
                    component="h1"
                    variant="h2"
                    align="center"
                    sx={{
                        fontWeight: 700,
                        mb: 4,
                        textShadow: '2px 2px 4px rgba(0,0,0,0.2)'
                    }}
                >
                    Találd meg ideális oktatód
                </Typography>

                <Typography variant="h5" align="center" sx={{ mb: 4 }}>
                    Bármely tantárgyról legyen szó
                </Typography>

                <Box sx={{
                    display: 'flex',
                    justifyContent: 'center',
                    gap: 2,
                    flexWrap: 'wrap'
                }}>
                    <Button
                        variant="contained"
                        size="large"
                        onClick={() => navigate(currentUserAuth ? '/dashboard' : '/register')}
                        sx={{
                            bgcolor: theme.palette.secondary.main,
                            '&:hover': {
                                bgcolor: theme.palette.secondary.dark
                            }
                        }}
                    >
                        {currentUserAuth ? 'Ugrás a dashboardra' : 'Regisztrálj most'}
                    </Button>

                    <Button
                        variant="outlined"
                        size="large"
                        onClick={() => navigate('/teachers')}
                        sx={{
                            color: 'inherit',
                            borderColor: 'currentColor',
                            '&:hover': {
                                bgcolor: 'rgba(255,255,255,0.1)'
                            }
                        }}
                    >
                        Böngészd az oktatókat
                    </Button>
                </Box>
            </Container>
        </Box>
    );
};

export default HeroSection;