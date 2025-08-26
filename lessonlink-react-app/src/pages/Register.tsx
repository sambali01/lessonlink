import {
    Avatar,
    Box,
    Button,
    CircularProgress,
    Container,
    FormHelperText,
    Link, Paper,
    TextField,
    Typography,
    useTheme
} from "@mui/material";
import { useState } from 'react';
import { Controller, useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { register } from "../services/user.service";
import "./Register.less";
import { Role } from "../models/Role";

interface RegisterForm {
    firstName: string;
    surName: string;
    email: string;
    password: string;
    role: Role;
}

export default function Register() {
    const theme = useTheme();
    const navigate = useNavigate();
    const { control, handleSubmit, formState: { errors } } = useForm<RegisterForm>({
        defaultValues: { role: Role.Student }
    });
    const [loading, setLoading] = useState(false);

    const onSubmit = async (data: RegisterForm) => {
        setLoading(true);
        try {
            await register(data.firstName, data.surName, data.email, data.password, data.role);
            navigate('/dashboard');
        } catch (error) {
            console.error("Registration failed:", error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container
            component="main"
            maxWidth="md"
            sx={{
                flex: 1,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                py: 2
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
                        bgcolor: theme.palette.secondary.main,
                        width: 36,
                        height: 36
                    }}>
                        ✍️
                    </Avatar>
                    <Typography component="h1" variant="h5">
                        Új fiók létrehozása
                    </Typography>
                </Box>

                <Box component="form" onSubmit={handleSubmit(onSubmit)}>
                    <Box sx={{
                        display: 'grid',
                        gridTemplateColumns: { sm: '1fr 1fr' },
                        gap: 2,
                        mb: 2
                    }}>
                        <Controller
                            name="firstName"
                            control={control}
                            defaultValue=""
                            rules={{ required: 'Kötelező mező' }}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    label="Keresztnév"
                                    error={!!errors.firstName}
                                    helperText={errors.firstName?.message}
                                />
                            )}
                        />

                        <Controller
                            name="surName"
                            control={control}
                            defaultValue=""
                            rules={{ required: 'Kötelező mező' }}
                            render={({ field }) => (
                                <TextField
                                    {...field}
                                    fullWidth
                                    label="Vezetéknév"
                                    error={!!errors.surName}
                                    helperText={errors.surName?.message}
                                />
                            )}
                        />
                    </Box>

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
                                error={!!errors.email}
                                helperText={errors.email?.message}
                            />
                        )}
                    />

                    <Controller
                        name="password"
                        control={control}
                        defaultValue=""
                        rules={{
                            required: 'Kötelező mező',
                            minLength: {
                                value: 8,
                                message: 'A jelszónak legalább 8 karakter hosszúnak kell lennie.'
                            }
                        }}
                        render={({ field }) => (
                            <TextField
                                {...field}
                                fullWidth
                                sx={{ mb: 3 }}
                                label="Jelszó"
                                type="password"
                                error={!!errors.password}
                                helperText={errors.password?.message}
                            />
                        )}
                    />

                    <Box
                        sx={{
                            display: 'grid',
                            gridTemplateColumns: { sm: '1fr 5fr' },
                            alignItems: 'center',
                            gap: 4,
                            mb: 3
                        }}
                    >
                        <Typography variant="subtitle1">
                            Fiók típusa:
                        </Typography>

                        <Controller
                            name="role"
                            control={control}
                            render={({ field }) => (
                                <Box sx={{
                                    display: 'grid',
                                    gridTemplateColumns: { sm: '1fr 1fr' },
                                    gap: 4
                                }}>
                                    <Box
                                        onClick={() => field.onChange(Role.Student)}
                                        sx={{
                                            cursor: 'pointer',
                                            alignContent: 'center',
                                            py: 1,
                                            px: 3,
                                            border: `2px solid ${field.value === Role.Student ? theme.palette.primary.main : theme.palette.divider}`,
                                            borderRadius: theme.shape.borderRadius,
                                            bgcolor: field.value === Role.Student ? theme.palette.primary.light : 'transparent'
                                        }}
                                    >
                                        <Typography
                                            variant="h6"
                                            sx={{
                                                textAlign: "center",
                                                color: field.value === Role.Student ? theme.palette.getContrastText(theme.palette.primary.light) : theme.palette.text.secondary,
                                                opacity: 0.9
                                            }}
                                        >
                                            Diák</Typography>
                                        <FormHelperText
                                            sx={{
                                                textAlign: "center",
                                                color: field.value === Role.Student ? theme.palette.getContrastText(theme.palette.primary.light) : theme.palette.text.secondary,
                                                opacity: 0.9
                                            }}
                                        >
                                            Órák foglalása
                                        </FormHelperText>
                                    </Box>

                                    <Box
                                        onClick={() => field.onChange(Role.Teacher)}
                                        sx={{
                                            cursor: 'pointer',
                                            alignContent: 'center',
                                            py: 1,
                                            px: 3,
                                            border: `2px solid ${field.value === Role.Teacher ? theme.palette.primary.main : theme.palette.divider}`,
                                            borderRadius: theme.shape.borderRadius,
                                            bgcolor: field.value === Role.Teacher ? theme.palette.primary.light : 'transparent'
                                        }}
                                    >
                                        <Typography
                                            variant="h6"
                                            sx={{
                                                textAlign: "center",
                                                color: field.value === Role.Teacher ? theme.palette.getContrastText(theme.palette.primary.light) : theme.palette.text.secondary,
                                                opacity: 0.9
                                            }}
                                        >
                                            Tanár</Typography>
                                        <FormHelperText
                                            sx={{
                                                textAlign: "center",
                                                color: field.value === Role.Teacher ? theme.palette.getContrastText(theme.palette.primary.light) : theme.palette.text.secondary,
                                                opacity: 0.9
                                            }}
                                        >
                                            Órák hirdetése
                                        </FormHelperText>
                                    </Box>
                                </Box>
                            )}
                        />
                    </Box>

                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        disabled={loading}
                        sx={{
                            bgcolor: theme.palette.secondary.main,
                            '&:hover': { bgcolor: theme.palette.secondary.dark },
                            py: 2
                        }}
                    >
                        {loading ? <CircularProgress size={24} /> : 'Regisztráció'}
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
                                navigate('/login');
                            }}
                            sx={{
                                color: theme.palette.text.secondary,
                                '&:hover': { color: theme.palette.primary.main }
                            }}
                        >
                            Már van fiókod? Jelentkezz be!
                        </Link>
                    </Box>
                </Box>
            </Paper>
        </Container>
    );
}