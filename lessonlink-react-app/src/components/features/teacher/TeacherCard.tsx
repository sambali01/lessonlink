import {
    Card,
    CardContent,
    CardMedia,
    Typography,
    Chip,
    Box,
    Rating,
    useTheme,
    CardActionArea
} from "@mui/material";
import { FunctionComponent } from "react";
import { TeacherDto } from "../../dtos/TeacherDto";

interface TeacherCardProps {
    teacher: TeacherDto;
    picturePath: string;
    onCardClick?: () => void;
}

const TeacherCard: FunctionComponent<TeacherCardProps> = ({ teacher, picturePath, onCardClick }) => {
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
                    image={teacher.profilePicture || picturePath}
                    alt={`${teacher.firstName} profilképe`}
                    sx={{
                        objectFit: 'cover',
                        bgcolor: theme.palette.mode === 'dark'
                            ? theme.palette.grey[800]
                            : theme.palette.grey[200]
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

                    <Box sx={{ mb: 1 }}>
                        <Rating
                            value={Number(teacher.rating) || 0}
                            precision={0.1}
                            readOnly
                            size="small"
                        />
                        <Typography variant="caption" color="text.secondary" ml={1}>
                            ({teacher.rating || 'Nincs értékelés'})
                        </Typography>
                    </Box>

                    <Box sx={{ mb: 2 }}>
                        {teacher.subjects.map((subject, index) => (
                            <Chip
                                key={index}
                                label={subject}
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

                    <Typography variant="body2" color="text.secondary" minHeight={110} maxHeight={120}>
                        {teacher.description || 'Nincs leírás'}
                    </Typography>

                    <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                        <Typography variant="body2">
                            {teacher.hourlyRate ? `${teacher.hourlyRate} Ft/óra` : 'Ár egyeztetés'}
                        </Typography>
                        <Typography variant="body2">
                            {teacher.location || 'Online'}
                        </Typography>
                    </Box>
                </CardContent>
            </CardActionArea>
        </Card>
    );
};

export default TeacherCard;