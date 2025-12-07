import {
    Box,
    Pagination,
    Paper,
    Skeleton,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useSearchTeachers } from "../hooks/teacherQueries";
import { TeacherSearchRequest } from "../models/TeacherSearchRequest";
import TeacherFilters from "../components/features/teacher/TeacherFilters";
import TeacherCard from "../components/features/teacher/TeacherCard";
import { TEACHER_SEARCH_PAGE_SIZE } from "../utils/constants";

const TeacherSearch: FunctionComponent = () => {
    const theme = useTheme();
    const navigate = useNavigate();
    const [searchFilters, setSearchFilters] = useState<TeacherSearchRequest>({ page: 1, pageSize: TEACHER_SEARCH_PAGE_SIZE });

    const { data: paginatedData, isLoading, isError } = useSearchTeachers(searchFilters);
    const teachers = paginatedData?.items ?? [];
    const totalPages = paginatedData?.totalCount ? Math.ceil(paginatedData.totalCount / TEACHER_SEARCH_PAGE_SIZE) : 0;

    const handlePageChange = (_event: React.ChangeEvent<unknown>, page: number) => {
        setSearchFilters(prev => ({ ...prev, page }));
    };

    return (
        <Box sx={{ p: { xs: 1, sm: 2 } }}>
            <Typography
                variant="h2"
                component="h1"
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
                            initialFilters={searchFilters}
                            onSearch={setSearchFilters}
                        />
                    </Paper>
                </Box>

                {/* Results */}
                <Box sx={{
                    display: 'flex',
                    flexWrap: 'wrap',
                    justifyContent: 'flex-start',
                    gap: 4
                }}>
                    {isError ? (
                        <Typography color="error" sx={{ gridColumn: '1 / -1', textAlign: 'center', py: 4 }}>
                            Hiba történt az adatok betöltése közben
                        </Typography>
                    ) : isLoading ? (
                        Array.from(new Array(6)).map((_, index) => (
                            <Skeleton
                                key={`skeleton-${index}`}
                                variant="rectangular"
                                height={400}
                                sx={{ borderRadius: theme.shape.borderRadius }}
                            />
                        ))
                    ) : teachers.length === 0 ? (
                        <Typography
                            variant="h6"
                            color="text.secondary"
                            sx={{
                                gridColumn: '1 / -1',
                                textAlign: 'center',
                                py: 8,
                                fontStyle: 'italic'
                            }}
                        >
                            Nincs találat a megadott szűrési feltételekkel.
                        </Typography>
                    ) : (
                        teachers.map(teacher => (
                            <TeacherCard
                                key={teacher.userId}
                                teacher={teacher}
                                onCardClick={() => navigate(`/teachers/${teacher.userId}`)}
                            />
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
                    page={searchFilters.page || 1}
                    onChange={handlePageChange}
                    color="primary"
                />
            </Box>
        </Box>
    );
};

export default TeacherSearch;