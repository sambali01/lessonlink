import {
    Autocomplete,
    Avatar,
    Box,
    Button,
    CircularProgress,
    Container,
    FormControlLabel,
    FormLabel,
    Radio,
    RadioGroup,
    Slider,
    TextField,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent, useEffect, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useSubjects } from "../hooks/subjectQueries";
import { useTeacherDetails } from "../hooks/teacherQueries";
import { useAuth } from "../hooks/useAuth";
import { useFindUserById, useUpdateStudent, useUpdateTeacher } from "../hooks/userQueries";
import { Subject } from "../models/Subject";
import { Role, StudentUpdateRequest, TeacherUpdateRequest } from "../models/User";
import { TeachingMethod } from "../utils/enums";
import { convertBoolsToTeachingMethod, convertTeachingMethodToExplicitBools } from "../utils/teachingMethodConverters";
import { useNotification } from "../hooks/useNotification";
import { ApiError } from "../utils/ApiError";

interface ProfileForm {
    nickName: string;
    profilePicture?: File;
    teachingMethod?: TeachingMethod;
    location?: string;
    hourlyRate?: number;
    description?: string;
    contact?: string;
    subjectNames?: string[];
}

const Profile: FunctionComponent = () => {
    const theme = useTheme();
    const { currentUserAuth } = useAuth();
    const { data: user, isLoading: userLoading } = useFindUserById(currentUserAuth?.userId || '');
    const { data: teacher, isLoading: teacherLoading } = useTeacherDetails(
        currentUserAuth?.userId || '',
        { enabled: currentUserAuth?.roles.includes(Role.Teacher) }
    );

    const updateStudentMutation = useUpdateStudent(currentUserAuth?.userId || '');
    const updateTeacherMutation = useUpdateTeacher(currentUserAuth?.userId || '');

    const { data: allSubjects } = useSubjects();
    const subjectNames = allSubjects ? allSubjects.map((s: Subject) => s.name) : [];
    const isTeacher = currentUserAuth?.roles.includes(Role.Teacher);

    const { control, handleSubmit, watch, setValue, formState: { errors } } = useForm<ProfileForm>({
        defaultValues: {
            nickName: '',
            teachingMethod: TeachingMethod.Online,
            location: '',
            hourlyRate: 0,
            description: '',
            contact: '',
            subjectNames: []
        }
    });

    const selectedTeachingMethod = watch('teachingMethod');
    const showLocationField = isTeacher && (
        selectedTeachingMethod === TeachingMethod.InPerson ||
        selectedTeachingMethod === TeachingMethod.Both
    );

    const { showSuccess, showError } = useNotification();
    const [isEditing, setIsEditing] = useState(false);
    const [imagePreview, setImagePreview] = useState<string | null>(null);

    const isUpdating = updateStudentMutation.isPending || updateTeacherMutation.isPending;

    useEffect(() => {
        if (user) {
            setValue('nickName', user.nickName || '');
        }
        if (teacher) {
            const teachingMethod = convertBoolsToTeachingMethod(teacher.acceptsOnline, teacher.acceptsInPerson);
            setValue('teachingMethod', teachingMethod);
            setValue('location', teacher.location || '');
            setValue('hourlyRate', teacher.hourlyRate || 0);
            setValue('description', teacher.description || '');
            setValue('contact', teacher.contact || '');
            setValue('subjectNames', teacher.subjects.map((s: Subject) => s.name) || []);
        }
    }, [user, teacher, setValue]);

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files?.[0]) {
            const file = e.target.files[0];
            setValue('profilePicture', file);
            const reader = new FileReader();
            reader.onloadend = () => setImagePreview(reader.result as string);
            reader.readAsDataURL(file);
        }
    };

    const onSubmit = async (data: ProfileForm) => {
        if (!currentUserAuth?.userId) return;

        const onSuccess = () => {
            setIsEditing(false);
            setImagePreview(null);
            showSuccess('Profil sikeresen frissítve!');
        };

        const onError = (error: Error) => {
            if (error instanceof ApiError) {
                showError(error.errors);
            } else {
                showError('Ismeretlen hiba történt');
            }
        };

        if (isTeacher) {
            const { acceptsOnline, acceptsInPerson } = convertTeachingMethodToExplicitBools(data.teachingMethod!);

            const updateData: TeacherUpdateRequest = {
                nickName: data.nickName !== user?.nickName ? data.nickName : undefined,
                profilePicture: data.profilePicture || undefined,
                acceptsOnline: acceptsOnline !== teacher?.acceptsOnline ? acceptsOnline : undefined,
                acceptsInPerson: acceptsInPerson !== teacher?.acceptsInPerson ? acceptsInPerson : undefined,
                location: showLocationField && data.location !== teacher?.location ? data.location : undefined,
                hourlyRate: data.hourlyRate !== teacher?.hourlyRate ? data.hourlyRate : undefined,
                description: data.description !== teacher?.description ? data.description : undefined,
                contact: data.contact !== teacher?.contact ? data.contact : undefined,
                subjectNames: data.subjectNames && data.subjectNames.length > 0 ? data.subjectNames : undefined
            };

            updateTeacherMutation.mutate(updateData, { onSuccess, onError });
        } else {
            const updateData: StudentUpdateRequest = {
                nickName: data.nickName !== user?.nickName ? data.nickName : undefined,
                profilePicture: data.profilePicture || undefined
            };

            updateStudentMutation.mutate(updateData, { onSuccess, onError });
        }
    };

    const handleCancel = () => {
        setIsEditing(false);
        setImagePreview(null);

        // Reset to original values
        if (user) {
            setValue('nickName', user.nickName || '');
        }
        if (teacher) {
            const teachingMethod = convertBoolsToTeachingMethod(teacher.acceptsOnline, teacher.acceptsInPerson);
            setValue('teachingMethod', teachingMethod);
            setValue('location', teacher.location || '');
            setValue('hourlyRate', teacher.hourlyRate || 0);
            setValue('description', teacher.description || '');
            setValue('contact', teacher.contact || '');
            setValue('subjectNames', teacher.subjects.map((s: Subject) => s.name) || []);
        }
    };

    if (userLoading || (currentUserAuth?.roles.includes(Role.Teacher) && teacherLoading)) {
        return <CircularProgress sx={{ display: 'block', mx: 'auto', my: 4 }} />;
    }

    return (
        <Container
            maxWidth="md"
            sx={{
                minWidth: "315px",
                py: 4,
                alignItems: 'center'
            }}
        >
            <Box sx={{
                display: 'flex',
                flexDirection: 'column',
                gap: 4,
                bgcolor: theme.palette.background.paper,
                px: 8,
                py: 4,
                borderRadius: 2,
                boxShadow: theme.shadows[2]
            }}>
                {/* Profile Header */}
                <Box sx={{
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    gap: 4
                }}
                >
                    <Box sx={{
                        position: 'relative',
                        display: 'inline-block',
                        textAlign: 'center'
                    }}
                    >
                        <Avatar
                            src={imagePreview || user?.imageUrl}
                            sx={{
                                width: 120,
                                height: 120,
                                fontSize: 40,
                                bgcolor: theme.palette.primary.main,
                                mb: 2,
                            }}
                        >
                            {!user?.imageUrl && `${user?.surName?.[0]}${user?.firstName?.[0]}`}
                        </Avatar>
                        {isEditing && (
                            <Button
                                component="label"
                                variant="outlined"
                                size="small"
                                sx={{
                                    position: 'absolute',
                                    bottom: -24,
                                    transform: 'translateX(-50%)',
                                    whiteSpace: 'nowrap',
                                    bgcolor: theme.palette.background.paper
                                }}
                            >
                                Profilkép módosítása
                                <input type="file" hidden accept="image/*" onChange={handleImageChange} />
                            </Button>
                        )}
                    </Box>
                    <Typography variant="h4" color="text.primary">
                        {user?.surName} {user?.firstName}
                    </Typography>
                </Box>

                {/* Action Buttons */}
                <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
                    {!isEditing ? (
                        <Button
                            variant="contained"
                            onClick={() => setIsEditing(true)}
                            sx={{ textTransform: 'none' }}
                        >
                            Profil szerkesztése
                        </Button>
                    ) : (
                        <>
                            <Button
                                variant="outlined"
                                onClick={handleCancel}
                                disabled={isUpdating}
                            >
                                Mégse
                            </Button>
                            <Button
                                variant="contained"
                                onClick={handleSubmit(onSubmit)}
                                disabled={isUpdating}
                                startIcon={isUpdating && <CircularProgress size={20} />}
                            >
                                {isUpdating ? 'Mentés...' : 'Mentés'}
                            </Button>
                        </>
                    )}
                </Box>

                {/* Profile Form */}
                <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                    <Controller
                        name="nickName"
                        control={control}
                        rules={{ required: 'Kötelező mező' }}
                        render={({ field }) => (
                            <TextField
                                {...field}
                                label="Becenév"
                                disabled={!isEditing}
                                fullWidth
                                variant="outlined"
                                error={!!errors.nickName}
                                helperText={errors.nickName?.message}
                            />
                        )}
                    />

                    {isTeacher && (
                        <>
                            <Box sx={{ mt: 2 }}>
                                <FormLabel component="legend">Oktatási mód</FormLabel>
                                <Controller
                                    name="teachingMethod"
                                    control={control}
                                    rules={{ required: 'Kötelező mező' }}
                                    render={({ field }) => (
                                        <RadioGroup
                                            row
                                            {...field}
                                            sx={{
                                                gap: 2,
                                                mt: 1,
                                                color: theme.palette.text.secondary
                                            }}
                                        >
                                            <FormControlLabel
                                                value={TeachingMethod.Online}
                                                control={<Radio color="primary" />}
                                                label="Csak online"
                                                disabled={!isEditing}
                                            />
                                            <FormControlLabel
                                                value={TeachingMethod.InPerson}
                                                control={<Radio color="primary" />}
                                                label="Csak személyesen"
                                                disabled={!isEditing}
                                            />
                                            <FormControlLabel
                                                value={TeachingMethod.Both}
                                                control={<Radio color="primary" />}
                                                label="Online vagy személyesen"
                                                disabled={!isEditing}
                                            />
                                        </RadioGroup>
                                    )}
                                />

                                {showLocationField && (
                                    <Controller
                                        name="location"
                                        control={control}
                                        rules={{
                                            required: showLocationField ? 'Kötelező mező személyes órákhoz' : false
                                        }}
                                        render={({ field }) => (
                                            <TextField
                                                {...field}
                                                label="Helyszín"
                                                disabled={!isEditing}
                                                fullWidth
                                                sx={{ mt: 2 }}
                                                error={!!errors.location}
                                                helperText={errors.location?.message}
                                            />
                                        )}
                                    />
                                )}
                            </Box>

                            <Box sx={{ mt: 2 }}>
                                <FormLabel component="legend">Óradíj (Ft/óra)</FormLabel>
                                <Controller
                                    name="hourlyRate"
                                    control={control}
                                    rules={{
                                        required: 'Kötelező mező',
                                        min: { value: 0, message: 'Az óradíj nem lehet negatív' }
                                    }}
                                    render={({ field }) => (
                                        <Slider
                                            {...field}
                                            min={0}
                                            max={20000}
                                            step={500}
                                            valueLabelDisplay={isEditing ? 'auto' : 'off'}
                                            disabled={!isEditing}
                                            sx={{
                                                mt: 2
                                            }}
                                            marks={[
                                                { value: 0, label: '0 Ft' },
                                                { value: 10000, label: '10.000 Ft' },
                                                { value: 20000, label: '20.000 Ft' }
                                            ]}
                                        />
                                    )}
                                />
                            </Box>

                            <Controller
                                name="contact"
                                control={control}
                                rules={{ required: 'Kötelező mező' }}
                                render={({ field }) => (
                                    <TextField
                                        {...field}
                                        label="Elérhetőség"
                                        disabled={!isEditing}
                                        fullWidth
                                        error={!!errors.contact}
                                        helperText={errors.contact?.message || "Email, telefon vagy más elérhetőség"}
                                    />
                                )}
                            />

                            <Controller
                                name="description"
                                control={control}
                                render={({ field }) => (
                                    <TextField
                                        {...field}
                                        label="Bemutatkozás"
                                        disabled={!isEditing}
                                        fullWidth
                                        multiline
                                        rows={4}
                                        helperText="Mutatkozz be röviden a leendő diákjaidnak"
                                    />
                                )}
                            />

                            <Box sx={{ mt: 2 }}>
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
                                            disabled={!isEditing}
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
                                            sx={{ mt: 1 }}
                                        />
                                    )}
                                />
                            </Box>
                        </>
                    )}
                </Box>
            </Box>
        </Container>
    );
};

export default Profile;