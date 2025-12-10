import {
    Person as InPersonIcon,
    Language as OnlineIcon
} from '@mui/icons-material';
import {
    Box,
    Card,
    CardActionArea,
    CardContent,
    CardMedia,
    Chip,
    Stack,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent } from "react";
import { Teacher } from "../../../models/Teacher";
import { BLANK_PROFILE_PICTURE_PATH } from "../../../utils/constants";
import { Subject } from '../../../models/Subject';

interface TeacherCardProps {
    teacher: Teacher;
    onCardClick?: () => void;
}

const TeacherCard: FunctionComponent<TeacherCardProps> = ({ teacher, onCardClick }) => {
    const theme = useTheme();

    return (
        <Card sx={{
            width: '100%',
            maxWidth: 345,
            height: '100%',
            display: 'flex',
            flexDirection: 'column',
            transition: 'transform 0.2s, box-shadow 0.2s',
            '&:hover': {
                transform: 'translateY(-4px)',
                boxShadow: theme.shadows[6]
            }
        }}>
            <CardActionArea onClick={onCardClick} sx={{ flexGrow: 1 }}>
                <CardMedia
                    component="img"
                    height={345}
                    image={teacher.imageUrl || BLANK_PROFILE_PICTURE_PATH}
                    alt={`${teacher.surName} ${teacher.firstName} profilképe`}
                    sx={{
                        objectFit: 'cover',
                        bgcolor: theme.palette.background.default
                    }}
                />
                <CardContent>
                    <Typography variant="h6" component="div">
                        {teacher.surName} {teacher.firstName}
                    </Typography>

                    {teacher.nickName && (
                        <Typography variant="subtitle2" color="text.secondary" mb={1}>
                            "{teacher.nickName}"
                        </Typography>
                    )}

                    {/* Teaching Methods */}
                    <Stack direction="row" spacing={0.5} sx={{ mb: 1 }}>
                        {teacher.acceptsOnline && (
                            <Chip
                                icon={<OnlineIcon />}
                                label="Online"
                                size="small"
                                variant="outlined"
                                color="primary"
                            />
                        )}
                        {teacher.acceptsInPerson && (
                            <Chip
                                icon={<InPersonIcon />}
                                label="Személyes"
                                size="small"
                                variant="outlined"
                                color="secondary"
                            />
                        )}
                    </Stack>

                    <Box sx={{ mb: 2 }}>
                        {teacher.subjects.map((subject: Subject, index: number) => (
                            <Chip
                                key={index}
                                label={subject.name}
                                size="small"
                                sx={{
                                    mr: 0.5,
                                    mb: 0.5,
                                    bgcolor: theme.palette.primary.light,
                                    color: theme.palette.getContrastText(theme.palette.primary.light)
                                }}
                            />
                        ))}
                    </Box>

                    <Typography
                        variant="body2"
                        color="text.secondary"
                        sx={{
                            mb: 2,
                            height: 60,
                            overflow: 'hidden',
                            textOverflow: 'ellipsis',
                            display: '-webkit-box',
                            WebkitLineClamp: 3,
                            WebkitBoxOrient: 'vertical',
                            lineHeight: 1.43
                        }}
                    >
                        {teacher.description || ''}
                    </Typography>

                    <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Typography variant="body2">
                            {teacher.hourlyRate ? `${teacher.hourlyRate} Ft/óra` : 'Ár egyeztetés'}
                        </Typography>
                        <Typography variant="body2">
                            {teacher.location || ''}
                        </Typography>
                    </Box>
                </CardContent>
            </CardActionArea>
        </Card>
    );
};

export default TeacherCard;