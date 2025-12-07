import {
    Alert,
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
    Snackbar,
    TextField,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { ProfileUpdateDto } from "../dtos/ProfileUpdateDto";
import { useSubjects } from "../hooks/subjectQueries";
import { useTeacherDetails } from "../hooks/teacherQueries";
import { useAuth } from "../hooks/useAuth";
import { useFindUserById, useUpdateProfile } from "../hooks/userQueries";
import { Role } from "../models/Role";
import { TeachingMethod } from "../utils/enums";
import { convertBoolsToTeachingMethod, convertTeachingMethodToBools } from "../utils/teachingMethodConverters";

const Profile: FunctionComponent = () => {
    const theme = useTheme();
    const navigate = useNavigate();
    const { currentUserAuth } = useAuth();
    const { data: user, isLoading: userLoading } = useFindUserById(currentUserAuth?.userId || '');
    const { data: teacher, isLoading: teacherLoading } = useTeacherDetails(
        currentUserAuth?.userId || '',
        { enabled: currentUserAuth?.roles.includes(Role.Teacher) }
    );

    const { mutate: updateProfile, isPending: isUpdating } = useUpdateProfile(currentUserAuth?.userId || '');

    const [isEditing, setIsEditing] = useState(false);
    const [imagePreview, setImagePreview] = useState<string | null>(null);

    const [error, setError] = useState<string | null>(null);

    // User state
    const [nickName, setNickName] = useState('');
    const [profilePicture, setProfilePicture] = useState<File | undefined>(undefined);

    // Teacher state
    const [teachingMethod, setTeachingMethod] = useState<TeachingMethod>(TeachingMethod.Online);
    const [location, setLocation] = useState('');
    const [hourlyRate, setHourlyRate] = useState<number>(0);
    const [selectedSubjects, setSelectedSubjects] = useState<string[]>([]);
    const { data: allSubjects = [] } = useSubjects();

    useEffect(() => {
        if (user) setNickName(user.nickName || '');
        if (teacher) {
            setTeachingMethod(
                convertBoolsToTeachingMethod(teacher.acceptsOnline, teacher.acceptsInPerson)
            );
            setLocation(teacher.location || '');
            setHourlyRate(teacher.hourlyRate || 0);
            setSelectedSubjects(teacher.subjects || []);
        }
    }, [user, teacher]);

    const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files?.[0]) {
            const file = e.target.files[0];
            setProfilePicture(file);
            const reader = new FileReader();
            reader.onloadend = () => setImagePreview(reader.result as string);
            reader.readAsDataURL(file);
        }
    };

    const handleSave = () => {
        if (!currentUserAuth?.userId) return;

        const { acceptsOnline, acceptsInPerson } = convertTeachingMethodToBools(teachingMethod);

        const updateData: ProfileUpdateDto = {
            nickName: nickName !== user?.nickName ? nickName : undefined,
            profilePicture: profilePicture || undefined,
            acceptsOnline,
            acceptsInPerson,
            location: teachingMethod !== TeachingMethod.Online ? location : undefined,
            hourlyRate: hourlyRate !== (teacher?.hourlyRate || 0) ? hourlyRate : undefined,
            subjects: currentUserAuth?.roles.includes(Role.Teacher) ? selectedSubjects : undefined
        };

        updateProfile(updateData, {
            onSuccess: () => {
                setIsEditing(false);
                setImagePreview(null);
                setProfilePicture(undefined);
            },
            onError: (error) => {
                setError(error instanceof Error ? error.message : 'Ismeretlen hiba történt');
            }
        });
    };

    const handleCancel = () => {
        setIsEditing(false);
        if (user) setNickName(user.nickName || '');
        if (teacher) {
            setTeachingMethod(
                convertBoolsToTeachingMethod(teacher.acceptsOnline, teacher.acceptsInPerson)
            );
            setLocation(teacher.location || '');
            setHourlyRate(teacher.hourlyRate || 0);
            setSelectedSubjects(teacher.subjects || []);
        }
        setImagePreview(null);
        setProfilePicture(undefined);
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
            <Snackbar open={!!error} autoHideDuration={6000} onClose={() => setError(null)}>
                <Alert severity="error">{error}</Alert>
            </Snackbar>

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
                            {!user?.imageUrl && `${user?.firstName?.[0]}${user?.surName?.[0]}`}
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
                        {user?.firstName} {user?.surName}
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
                                onClick={handleSave}
                                disabled={isUpdating}
                                startIcon={isUpdating && <CircularProgress size={20} />}
                            >
                                {isUpdating ? 'Mentés...' : 'Mentés'}
                            </Button>
                        </>
                    )}
                </Box>

                {/* Profile Form */}
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
                    <TextField
                        label="Becenév"
                        value={nickName}
                        onChange={(e) => setNickName(e.target.value)}
                        disabled={!isEditing}
                        fullWidth
                        variant="outlined"
                    />

                    {currentUserAuth?.roles.includes(Role.Teacher) ? (
                        <>
                            <Box sx={{ mt: 2 }}>
                                <FormLabel component="legend">Óraadás módja</FormLabel>
                                <RadioGroup
                                    value={teachingMethod}
                                    onChange={(e) => setTeachingMethod(e.target.value as TeachingMethod)}
                                    row
                                    sx={{
                                        gap: 3,
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
                                        label="Online és személyesen"
                                        disabled={!isEditing}
                                    />
                                </RadioGroup>

                                {(teachingMethod === TeachingMethod.InPerson || teachingMethod === TeachingMethod.Both) && (
                                    <TextField
                                        label="Helyszín"
                                        value={location}
                                        onChange={(e) => setLocation(e.target.value)}
                                        disabled={!isEditing}
                                        fullWidth
                                        sx={{ mt: 2 }}
                                    />
                                )}
                            </Box>

                            <Box sx={{ mt: 2 }}>
                                <FormLabel component="legend">Óradíj (Ft/óra)</FormLabel>
                                <Slider
                                    value={hourlyRate}
                                    onChange={(_, value) => setHourlyRate(value as number)}
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
                            </Box>
                            <Box sx={{ mt: 2 }}>
                                <Autocomplete
                                    multiple
                                    options={allSubjects}
                                    value={selectedSubjects}
                                    onChange={(_, newValue) => setSelectedSubjects(newValue)}
                                    disabled={!isEditing}
                                    renderInput={(params) => (
                                        <TextField
                                            {...params}
                                            label="Tantárgyak"
                                        />
                                    )}
                                    slotProps={{
                                        chip: {
                                            size: "small"
                                        }
                                    }}
                                    sx={{ mt: 1 }}
                                />
                            </Box>
                        </>
                    ) : (
                        <Button
                            variant="contained"
                            onClick={() => navigate('/become-teacher')}
                            sx={{ mt: 3, alignSelf: 'center' }}
                        >
                            Légy te is tanár!
                        </Button>
                    )}
                </Box>
            </Box>
        </Container>
    );
};

export default Profile;