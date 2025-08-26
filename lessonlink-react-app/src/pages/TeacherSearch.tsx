import {
    Box,
    Container,
    Pagination,
    Paper,
    Skeleton,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent, useState } from "react";
import { useNavigate } from "react-router-dom";
import TeacherCard from "../components/features/TeacherCard";
import TeacherFilters from "../components/features/TeacherFilters";
import { useSubjects } from "../hooks/subjectQueries";
import { useSearchTeachers } from "../hooks/teacherQueries";
import { TeacherSearchFilters } from "../models/TeacherSearchFilters";

const PAGE_SIZE = 12;

const TeacherSearch: FunctionComponent = () => {
    const theme = useTheme();
    const navigate = useNavigate();
    const [filters, setFilters] = useState<TeacherSearchFilters>({
        searchQuery: '',
        subjects: [],
        minPrice: 0,
        maxPrice: 20000,
        minRating: 0,
        acceptsOnline: false,
        acceptsInPerson: false,
        page: 1,
        pageSize: PAGE_SIZE
    });

    const { data: paginatedData, isLoading, isError } = useSearchTeachers(filters);
    const teachers = paginatedData?.items ?? [];
    const totalPages = paginatedData?.totalCount ? Math.ceil(paginatedData.totalCount / PAGE_SIZE) : 0;

    const { data: subjects = [] } = useSubjects();

    const handlePageChange = (_event: React.ChangeEvent<unknown>, page: number) => {
        setFilters(prev => ({ ...prev, page }));
    };

    return (
        <Container maxWidth="xl" sx={{ py: 2 }}>
            <Typography variant="h2"
                sx={{
                    mb: 4,
                    color: theme.palette.text.primary,
                    fontWeight: 400,
                    letterSpacing: '-1px'
                }}>
                Oktatók böngészése
            </Typography>

            <Box sx={{
                display: 'grid',
                gridTemplateColumns: { xs: '300px 1fr', md: '300px 1fr' },
                gap: 4,
                alignItems: 'start'
            }}>
                {/* Filters */}
                <Box sx={{}}>
                    <Paper elevation={3} sx={{ borderRadius: theme.shape.borderRadius, p: 4 }}>
                        <TeacherFilters
                            filters={filters}
                            onFilterChange={(newFilters) => setFilters({ ...newFilters, page: 1 })}
                            subjectsOptions={subjects}
                        />
                    </Paper>
                </Box>

                {/* Results */}
                <Box sx={{
                    display: 'grid',
                    gridTemplateColumns: {
                        xs: '1fr',
                        sm: 'repeat(2, 1fr)',
                        lg: 'repeat(3, 1fr)'
                    },
                    gap: 4,
                    alignContent: 'start'
                }}>
                    {isError ? (
                        <Typography color="error">
                            Hiba történt az adatok betöltése közben
                        </Typography>
                    ) : (
                        (isLoading ? Array.from(new Array(6)) : teachers).map((teacher, index) => (
                            isLoading ? (
                                <Skeleton
                                    key={`skeleton-${index}`}
                                    variant="rectangular"
                                    height={400}
                                    sx={{ borderRadius: theme.shape.borderRadius }}
                                />
                            ) : (
                                <TeacherCard
                                    key={teacher.userId}
                                    teacher={teacher}
                                    picturePath="src/assets/images/exampleteacher2.jpg"
                                    onCardClick={() => navigate(`/teachers/${teacher.userId}`)}
                                />
                            )
                        ))
                    )}
                </Box>
            </Box>

            {/* Pagination */}
            <Box sx={{
                display: 'flex',
                justifyContent: 'center',
                mt: 4,
                visibility: teachers.length > 0 ? 'visible' : 'hidden'
            }}>
                <Pagination
                    count={totalPages}
                    page={filters.page}
                    onChange={handlePageChange}
                    color="primary"
                />
            </Box>
        </Container>
    );
};

export default TeacherSearch;