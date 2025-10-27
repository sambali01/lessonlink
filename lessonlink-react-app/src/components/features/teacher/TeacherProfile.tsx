import React from 'react';
import {
    Avatar,
    Box,
    Chip,
    Divider,
    Paper,
    Rating,
    Skeleton,
    Typography,
    useTheme,
    Stack
} from '@mui/material';
import {
    LocationOn as LocationIcon,
    Language as OnlineIcon,
    Person as InPersonIcon,
    School as SubjectIcon,
    AttachMoney as PriceIcon
} from '@mui/icons-material';
import { TeacherDto } from '../../../dtos/TeacherDto';

interface TeacherProfileProps {
    teacher: TeacherDto;
    isLoading: boolean;
}

const TeacherProfile: React.FC<TeacherProfileProps> = ({ teacher, isLoading }) => {
    const theme = useTheme();

    if (isLoading) {
        return (
            <Paper sx={{ p: 4, borderRadius: 2 }}>
                <Stack spacing={3}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 3 }}>
                        <Skeleton variant="circular" width={120} height={120} />
                        <Box sx={{ flex: 1 }}>
                            <Skeleton width="60%" height={40} />
                            <Skeleton width="40%" height={30} sx={{ mt: 1 }} />
                            <Skeleton width="50%" height={25} sx={{ mt: 1 }} />
                        </Box>
                    </Box>
                    <Skeleton height={100} />
                    <Skeleton height={60} />
                </Stack>
            </Paper>
        );
    }

    return (
        <Paper sx={{ p: 4, borderRadius: 2, boxShadow: theme.shadows[2] }}>
            <Stack spacing={4}>
                {/* Header Section - Avatar and Basic Info */}
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 3, flexWrap: 'wrap' }}>
                    <Avatar
                        src={teacher.profilePicture || '/src/assets/images/exampleteacher1.jpg'}
                        sx={{
                            width: 120,
                            height: 120,
                            border: `3px solid ${theme.palette.primary.light}`
                        }}
                    >
                        {teacher.firstName?.[0]}{teacher.surName?.[0]}
                    </Avatar>

                    <Box sx={{ flex: 1, minWidth: 250 }}>
                        <Typography variant="h3" component="h1" gutterBottom>
                            {teacher.firstName} {teacher.surName}
                            {teacher.nickName && (
                                <Typography
                                    variant="h5"
                                    component="span"
                                    sx={{
                                        ml: 2,
                                        opacity: 0.7,
                                        fontStyle: 'italic'
                                    }}
                                >
                                    "{teacher.nickName}"
                                </Typography>
                            )}
                        </Typography>

                        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                            <Rating
                                value={Number(teacher.rating) || 0}
                                precision={0.5}
                                readOnly
                                size="large"
                            />
                            <Typography variant="body1" sx={{ ml: 2 }}>
                                ({teacher.rating || 'Nincs értékelés'})
                            </Typography>
                        </Box>

                        {/* Teaching Methods */}
                        <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                            {teacher.acceptsOnline && (
                                <Chip
                                    icon={<OnlineIcon />}
                                    label="Online órák"
                                    variant="outlined"
                                    color="primary"
                                />
                            )}
                            {teacher.acceptsInPerson && (
                                <Chip
                                    icon={<InPersonIcon />}
                                    label="Személyes órák"
                                    variant="outlined"
                                    color="secondary"
                                />
                            )}
                        </Stack>
                    </Box>
                </Box>

                <Divider />

                {/* Description Section */}
                {teacher.description && (
                    <>
                        <Box>
                            <Typography variant="h5" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                <SubjectIcon color="primary" />
                                Bemutatkozás
                            </Typography>
                            <Typography
                                variant="body1"
                                sx={{
                                    lineHeight: 1.7,
                                    whiteSpace: 'pre-wrap'
                                }}
                            >
                                {teacher.description}
                            </Typography>
                        </Box>
                        <Divider />
                    </>
                )}

                {/* Subjects Section */}
                <Box>
                    <Typography variant="h5" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <SubjectIcon color="primary" />
                        Tantárgyak
                    </Typography>
                    <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                        {teacher.subjects?.length > 0 ? (
                            teacher.subjects.map((subject, index) => (
                                <Chip
                                    key={index}
                                    label={subject}
                                    sx={{
                                        bgcolor: theme.palette.primary.light,
                                        color: theme.palette.getContrastText(theme.palette.primary.light),
                                        fontWeight: 'medium'
                                    }}
                                />
                            ))
                        ) : (
                            <Typography color="text.secondary">
                                Nincsenek megadott tantárgyak
                            </Typography>
                        )}
                    </Stack>
                </Box>

                <Divider />

                {/* Details Grid */}
                <Box sx={{
                    display: 'grid',
                    gridTemplateColumns: { xs: '1fr', md: 'repeat(2, 1fr)' },
                    gap: 3
                }}>
                    {/* Pricing */}
                    <Box>
                        <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <PriceIcon color="primary" />
                            Óradíj
                        </Typography>
                        <Typography variant="body1" sx={{ fontSize: '1.1rem', fontWeight: 'medium' }}>
                            {teacher.hourlyRate
                                ? `${teacher.hourlyRate.toLocaleString()} Ft/óra`
                                : 'Egyedi árazás'}
                        </Typography>
                    </Box>

                    {/* Location */}
                    <Box>
                        <Typography variant="h6" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                            <LocationIcon color="primary" />
                            Helyszín
                        </Typography>
                        <Typography variant="body1" sx={{ fontSize: '1.1rem' }}>
                            {teacher.location || 'Nem megadott'}
                        </Typography>
                    </Box>
                </Box>
            </Stack>
        </Paper>
    );
};

export default TeacherProfile;