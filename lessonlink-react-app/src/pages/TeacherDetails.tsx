import {
    Box,
    Pagination,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent, useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import TeacherProfile from "../components/features/teacher-details/TeacherProfile";
import TeacherScheduleView from "../components/features/teacher-details/TeacherScheduleView";
import { useAvailableSlotsByTeacherId } from "../hooks/avaliableSlotQueries";
import { useCreateBooking } from "../hooks/bookingQueries";
import { useTeacherDetails } from "../hooks/teacherQueries";
import { AvailableSlot } from "../models/AvailableSlot";
import { TEACHER_SLOTS_PAGE_SIZE } from "../utils/constants";
import { useNotification } from "../hooks/useNotification";
import { ApiError } from "../utils/ApiError";

const TeacherDetails: FunctionComponent = () => {
    const theme = useTheme();
    const { userId } = useParams<{ userId: string }>();
    const navigate = useNavigate();
    const [currentPage, setCurrentPage] = useState(1);

    const { data: teacher, isLoading: teacherLoading } = useTeacherDetails(userId || '');
    const { data: slotsResponse, isLoading: slotsLoading } = useAvailableSlotsByTeacherId(userId || '', currentPage, TEACHER_SLOTS_PAGE_SIZE);

    const createBookingMutation = useCreateBooking();
    const { showSuccess, showError } = useNotification();

    // Navigate to not-found if teacher doesn't exist
    useEffect(() => {
        if (!teacher && !teacherLoading) {
            navigate('/not-found');
        }
    }, [teacher, teacherLoading, navigate]);

    const handleBookSlot = async (slot: AvailableSlot) => {
        try {
            await createBookingMutation.mutateAsync({ availableSlotId: slot.id });
            showSuccess('Foglalás sikeres!');
            navigate('/my-bookings');
        } catch (error) {
            if (error instanceof ApiError) {
                showError(error.errors);
            } else {
                showError('Hiba történt a foglalás során');
            }
        }
    };

    const handlePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
        setCurrentPage(value);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };

    const slots = slotsResponse?.items || [];
    const totalPages = slotsResponse?.totalPages || 0;

    return (
        <Box sx={{
            minHeight: '100vh',
            bgcolor: theme.palette.mode === 'dark' ? theme.palette.background.default : '#f5f5f5',
            py: 4
        }}>
            <Box sx={{
                maxWidth: '1400px',
                mx: 'auto',
                px: { xs: 2, sm: 3, md: 4 }
            }}>
                {/* Teacher Profile Section */}
                <TeacherProfile
                    teacher={teacher}
                    isLoading={teacherLoading}
                />

                {/* Available Slots Section */}
                <Box sx={{ mt: 4 }}>
                    <Typography
                        variant="h4"
                        sx={{
                            mb: 3,
                            color: theme.palette.text.primary,
                            fontWeight: 500
                        }}
                    >
                        Elérhető időpontok
                    </Typography>

                    <TeacherScheduleView
                        slots={slots}
                        isLoading={slotsLoading}
                        onBookSlot={handleBookSlot}
                    />

                    {/* Pagination */}
                    {totalPages > 1 && (
                        <Box display="flex" justifyContent="center" mt={4}>
                            <Pagination
                                count={totalPages}
                                page={currentPage}
                                onChange={handlePageChange}
                                color="primary"
                                size="large"
                                sx={{
                                    '& .MuiPaginationItem-root': {
                                        fontSize: '1rem',
                                    },
                                }}
                            />
                        </Box>
                    )}
                </Box>
            </Box>
        </Box>
    );
};

export default TeacherDetails;