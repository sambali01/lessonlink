import {
    Autocomplete,
    Avatar,
    Box,
    Button,
    CircularProgress,
    Container,
    FormControlLabel,
    FormHelperText,
    FormLabel,
    Link,
    Paper,
    Radio,
    RadioGroup,
    Slider,
    TextField,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent } from "react";
import { Controller, useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useRegisterStudent, useRegisterTeacher } from "../hooks/userQueries";
import { Role } from "../models/User";
import { TeachingMethod } from "../utils/enums";
import { convertTeachingMethodToExplicitBools } from "../utils/teachingMethodConverters";
import { useSubjects } from "../hooks/subjectQueries";

interface RegisterForm {
    firstName: string;
    surName: string;
    email: string;
    password: string;
    role: Role;
    // Teacher-specific fields
    teachingMethod?: TeachingMethod;
    location?: string;
    hourlyRate?: number;
    contact?: string;
    subjectNames?: string[];
}

const Register: FunctionComponent = () => {
    const theme = useTheme();
    const navigate = useNavigate();
    const { control, handleSubmit, watch, formState: { errors } } = useForm<RegisterForm>({
        defaultValues: {
            role: Role.Student,
            teachingMethod: TeachingMethod.Online,
            subjectNames: []
        }
    });

    const registerStudentMutation = useRegisterStudent();
    const registerTeacherMutation = useRegisterTeacher();
    const { data: allSubjects } = useSubjects();
    const subjectNames = allSubjects ? allSubjects.map((s) => s.name) : [];

    const selectedRole = watch('role');
    const selectedTeachingMethod = watch('teachingMethod');
    const isTeacher = selectedRole === Role.Teacher;
    const showLocationField = isTeacher && (
        selectedTeachingMethod === TeachingMethod.InPerson ||
        selectedTeachingMethod === TeachingMethod.Both
    );

    const onSubmit = async (data: RegisterForm) => {
        try {
            if (data.role === Role.Student) {
                await registerStudentMutation.mutateAsync({
                    firstName: data.firstName,
                    surName: data.surName,
                    email: data.email,
                    password: data.password
                });
            } else {
                const { acceptsOnline, acceptsInPerson } = convertTeachingMethodToExplicitBools(data.teachingMethod!);

                await registerTeacherMutation.mutateAsync({
                    firstName: data.firstName,
                    surName: data.surName,
                    email: data.email,
                    password: data.password,
                    acceptsOnline,
                    acceptsInPerson,
                    location: showLocationField ? data.location || null : null,
                    hourlyRate: data.hourlyRate!,
                    contact: data.contact!,
                    subjectNames: data.subjectNames || []
                });
            }
            navigate('/login');
        } catch (error) {
            console.error("Registration failed:", error);
        }
    };

    const loading = registerStudentMutation.isPending || registerTeacherMutation.isPending;

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

                    {isTeacher && (
                        <>
                            <FormLabel component="legend" sx={{ mb: 1 }}>
                                Oktatási mód
                            </FormLabel>
                            <Controller
                                name="teachingMethod"
                                control={control}
                                rules={{ required: isTeacher ? 'Kötelező mező' : false }}
                                render={({ field }) => (
                                    <RadioGroup row {...field} sx={{ mb: 2 }}>
                                        <FormControlLabel
                                            value={TeachingMethod.Online}
                                            control={<Radio />}
                                            label="Csak online"
                                        />
                                        <FormControlLabel
                                            value={TeachingMethod.InPerson}
                                            control={<Radio />}
                                            label="Csak személyesen"
                                        />
                                        <FormControlLabel
                                            value={TeachingMethod.Both}
                                            control={<Radio />}
                                            label="Online vagy személyesen"
                                        />
                                    </RadioGroup>
                                )}
                            />
                            {errors.teachingMethod && (
                                <FormHelperText error>{errors.teachingMethod.message}</FormHelperText>
                            )}

                            {showLocationField && (
                                <Controller
                                    name="location"
                                    control={control}
                                    defaultValue=""
                                    rules={{
                                        required: showLocationField ? 'Kötelező mező személyes órákhoz' : false
                                    }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            fullWidth
                                            sx={{ mb: 2 }}
                                            label="Helyszín"
                                            error={!!errors.location}
                                            helperText={errors.location?.message}
                                        />
                                    )}
                                />
                            )}

                            <Box sx={{ mb: 3 }}>
                                <FormLabel component="legend" sx={{ mb: 1 }}>
                                    Óradíj (Ft/óra)
                                </FormLabel>
                                <Controller
                                    name="hourlyRate"
                                    control={control}
                                    defaultValue={5000}
                                    rules={{
                                        required: isTeacher ? 'Kötelező mező' : false,
                                        min: { value: 0, message: 'Az óradíj nem lehet negatív' }
                                    }}
                                    render={({ field }) => (
                                        <Slider
                                            {...field}
                                            min={0}
                                            max={20000}
                                            step={500}
                                            valueLabelDisplay="auto"
                                            marks={[
                                                { value: 0, label: '0 Ft' },
                                                { value: 10000, label: '10.000 Ft' },
                                                { value: 20000, label: '20.000 Ft' }
                                            ]}
                                        />
                                    )}
                                />
                                {errors.hourlyRate && (
                                    <FormHelperText error>{errors.hourlyRate.message}</FormHelperText>
                                )}
                            </Box>

                            <Controller
                                name="contact"
                                control={control}
                                defaultValue=""
                                rules={{ required: isTeacher ? 'Kötelező mező' : false }}
                                render={({ field }) => (
                                    <TextField
                                        {...field}
                                        fullWidth
                                        sx={{ mb: 2 }}
                                        label="Elérhetőség"
                                        placeholder="Email, telefon vagy más elérhetőség"
                                        error={!!errors.contact}
                                        helperText={errors.contact?.message || "Ezt fogják látni a diákok a jóváhagyott foglalásokban"}
                                    />
                                )}
                            />

                            <Box sx={{ mb: 3 }}>
                                <Controller
                                    name="subjectNames"
                                    control={control}
                                    rules={{
                                        required: isTeacher ? 'Legalább egy tantárgyat válassz ki' : false,
                                        validate: (value) => (value && value.length > 0) || 'Legalább egy tantárgyat válassz ki'
                                    }}
                                    render={({ field }) => (
                                        <Autocomplete
                                            {...field}
                                            multiple
                                            options={subjectNames}
                                            value={field.value || []}
                                            onChange={(_, newValue) => field.onChange(newValue)}
                                            renderInput={(params) => (
                                                <TextField
                                                    {...params}
                                                    label="Tantárgyak"
                                                    error={!!errors.subjectNames}
                                                    helperText={errors.subjectNames?.message}
                                                />
                                            )}
                                            slotProps={{
                                                chip: {
                                                    size: "small"
                                                }
                                            }}
                                        />
                                    )}
                                />
                            </Box>
                        </>
                    )}

                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        color="secondary"
                        disabled={loading}
                        sx={{
                            py: 1.5,
                            mt: 2
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

export default Register;