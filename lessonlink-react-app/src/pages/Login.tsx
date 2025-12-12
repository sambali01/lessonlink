import {
    Avatar,
    Box,
    Button,
    CircularProgress,
    Container,
    Link,
    Paper,
    TextField,
    Typography,
    useTheme
} from "@mui/material";
import LoginIcon from "@mui/icons-material/Login";
import { FunctionComponent, useState } from 'react';
import { Controller, useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { useNotification } from "../hooks/useNotification";
import { ApiError } from "../utils/ApiError";

interface LoginForm {
    email: string;
    password: string;
}

const Login: FunctionComponent = () => {
    const theme = useTheme();
    const navigate = useNavigate();
    const { handleLogin } = useAuth();
    const { showError } = useNotification();
    const { control, handleSubmit, formState: { errors } } = useForm<LoginForm>();
    const [loading, setLoading] = useState(false);

    const onSubmit = async (data: LoginForm) => {
        setLoading(true);
        try {
            handleLogin(data.email, data.password);
            navigate('/dashboard');
        } catch (error) {
            if (error instanceof ApiError) {
                showError(error.errors);
            } else {
                showError('Bejelentkezés sikertelen');
            }
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container
            component="main"
            maxWidth="xs"
            sx={{
                flex: 1,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                py: 4
            }}
        >
            <Paper elevation={3} sx={{
                width: '100%',
                p: 4,
                borderRadius: theme.shape.borderRadius,
                backgroundColor: theme.palette.background.paper
            }}>
                <Box sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                    gap: 2,
                    mb: 3
                }}>
                    <Avatar sx={{
                        bgcolor: theme.palette.primary.main,
                        width: 36,
                        height: 36
                    }}>
                        <LoginIcon />
                    </Avatar>
                    <Typography component="h1" variant="h5">
                        Bejelentkezés
                    </Typography>
                </Box>

                <Box component="form" onSubmit={handleSubmit(onSubmit)}>
                    <Controller
                        name="email"
                        control={control}
                        defaultValue=""
                        rules={{
                            required: 'Kötelező mező',
                            pattern: {
                                value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                                message: 'Érvénytelen email cím'
                            }
                        }}
                        render={({ field }) => (
                            <TextField
                                {...field}
                                fullWidth
                                sx={{ mb: 2 }}
                                label="Email cím"
                                autoComplete="email"
                                error={!!errors.email}
                                helperText={errors.email?.message}
                            />
                        )}
                    />

                    <Controller
                        name="password"
                        control={control}
                        defaultValue=""
                        rules={{ required: 'Kötelező mező' }}
                        render={({ field }) => (
                            <TextField
                                {...field}
                                fullWidth
                                sx={{ mb: 3 }}
                                label="Jelszó"
                                type="password"
                                autoComplete="current-password"
                                error={!!errors.password}
                                helperText={errors.password?.message}
                            />
                        )}
                    />

                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        disabled={loading}
                        sx={{ py: 1.5 }}
                    >
                        {loading ? <CircularProgress size={24} /> : 'Belépés'}
                    </Button>

                    <Box sx={{
                        display: 'flex',
                        justifyContent: 'center',
                        mt: 2
                    }}>
                        <Link
                            component="button"
                            variant="body2"
                            onClick={(e) => {
                                e.preventDefault();
                                navigate('/register');
                            }}
                            sx={{
                                color: theme.palette.text.secondary,
                                '&:hover': { color: theme.palette.primary.main }
                            }}
                        >
                            Nincs még fiókod? Regisztrálj!
                        </Link>
                    </Box>
                </Box>
            </Paper>
        </Container>
    );
};

export default Login;