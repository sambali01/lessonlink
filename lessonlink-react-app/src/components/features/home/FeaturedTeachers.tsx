import { Box, Button, Container, Skeleton, Typography, useTheme } from "@mui/material";
import { FunctionComponent } from "react";
import { useNavigate } from "react-router-dom";
import { useFeaturedTeachers } from "../../../hooks/teacherQueries";
import TeacherCard from "../teacher/TeacherCard";

const FeaturedTeachers: FunctionComponent = () => {
    const theme = useTheme();
    const navigate = useNavigate();
    const { data: teachers = [], isLoading, isError } = useFeaturedTeachers();

    return (
        <Box sx={{
            py: 8
        }}>
            <Container>
                <Box sx={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center',
                    mb: 4,
                    flexDirection: { xs: 'column', sm: 'row' },
                    gap: { xs: 2, sm: 0 }
                }}>
                    <Typography
                        variant="h4"
                        component="h2"
                        sx={{
                            textAlign: { xs: 'center', sm: 'left' },
                            color: theme.palette.text.primary
                        }}
                    >
                        Kiemelt oktatóink
                    </Typography>
                    <Button
                        variant="outlined"
                        onClick={() => navigate('/teachers')}
                        sx={{ mt: { xs: 2, sm: 0 } }}
                    >
                        Összes tanár megtekintése
                    </Button>
                </Box>

                {isError ? (
                    <Typography color="error" textAlign="center">
                        Hiba történt az adatok betöltése közben.
                    </Typography>
                ) : (
                    <Box sx={{
                        display: 'grid',
                        gridTemplateColumns: {
                            xs: '1fr',
                            sm: 'repeat(2, 1fr)',
                            md: 'repeat(3, 1fr)',
                            lg: 'repeat(4, 1fr)'
                        },
                        gap: 4,
                        justifyItems: 'center'
                    }}>
                        {(isLoading ? Array.from(new Array(4)) : teachers).map((teacher, index) => (
                            isLoading ? (
                                <Skeleton
                                    key={`skeleton-${index}`}
                                    variant="rectangular"
                                    height={400}
                                    sx={{
                                        borderRadius: 2,
                                        width: '100%',
                                        maxWidth: 345
                                    }}
                                />
                            ) : (
                                <TeacherCard
                                    key={teacher.userId}
                                    teacher={teacher}
                                    picturePath={`src/assets/images/exampleteacher${index}.jpg`}
                                    onCardClick={() => navigate(`/teachers/${teacher.userId}`)}
                                />
                            )
                        ))}
                    </Box>
                )}
            </Container>
        </Box>
    );
};

export default FeaturedTeachers;