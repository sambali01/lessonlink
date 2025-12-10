import {
    Box,
    Button,
    Container,
    Paper,
    Tab,
    Tabs,
    Typography,
    useTheme,
    Alert
} from "@mui/material";
import { FunctionComponent, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import TabPanel from "../components/common/TabPanel";
import TeacherProfile from "../components/features/teacher/TeacherProfile";
import TeacherScheduleView from "../components/features/teacher/TeacherScheduleView";
import { useTeacherDetails } from "../hooks/teacherQueries";
import { useAvailableSlotsByTeacherId } from "../hooks/avaliableSlotQueries";
import { useCreateBooking } from "../hooks/bookingQueries";
import { AvailableSlot } from "../models/AvailableSlot";

const TeacherDetails: FunctionComponent = () => {
    const theme = useTheme();
    const { userId } = useParams<{ userId: string }>();
    const navigate = useNavigate();
    const [tabValue, setTabValue] = useState(0);

    // Queries
    const {
        data: teacher,
        isLoading: teacherLoading,
        isError: teacherError
    } = useTeacherDetails(userId || '');

    const {
        data: availableSlots,
        isLoading: slotsLoading,
        error: slotsError
    } = useAvailableSlotsByTeacherId(userId || '');

    const createBookingMutation = useCreateBooking();

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    const handleBookSlot = async (slot: AvailableSlot) => {
        try {
            await createBookingMutation.mutateAsync({ availableSlotId: slot.id });
            // Show success message or navigate to bookings page
            navigate('/dashboard');
        } catch (error) {
            console.error('Booking failed:', error);
            // Handle error (show toast, etc.)
        }
    };

    // Error handling
    if (teacherError) {
        return (
            <Container sx={{ textAlign: 'center', py: 8 }}>
                <Alert severity="error" sx={{ mb: 3 }}>
                    Hiba történt a tanár adatainak betöltése közben
                </Alert>
                <Button variant="contained" onClick={() => navigate('/')}>
                    Vissza a főoldalra
                </Button>
            </Container>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ py: 4 }}>
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>

                {/* Teacher Profile Section */}
                <TeacherProfile
                    teacher={teacher!}
                    isLoading={teacherLoading}
                />

                {/* Tabs Section */}
                <Paper sx={{
                    borderRadius: theme.shape.borderRadius,
                    overflow: 'hidden',
                    boxShadow: theme.shadows[2]
                }}>
                    <Tabs
                        value={tabValue}
                        onChange={handleTabChange}
                        variant="fullWidth"
                        sx={{
                            borderBottom: 1,
                            borderColor: 'divider',
                            bgcolor: theme.palette.mode === 'dark'
                                ? theme.palette.background.default
                                : '#fff'
                        }}
                    >
                        <Tab label="Elérhető időpontok" />
                        <Tab label="Kapcsolat" />
                    </Tabs>

                    {/* Available Slots Tab */}
                    <TabPanel value={tabValue} index={0}>
                        <Box sx={{ p: 3 }}>
                            <Typography variant="h5" gutterBottom sx={{ mb: 3 }}>
                                Elérhető időpontok
                            </Typography>
                            <TeacherScheduleView
                                slots={availableSlots || []}
                                isLoading={slotsLoading}
                                error={slotsError}
                                onBookSlot={handleBookSlot}
                            />
                        </Box>
                    </TabPanel>

                    {/* Contact Tab */}
                    <TabPanel value={tabValue} index={1}>
                        <Box sx={{
                            p: 4,
                            textAlign: 'center',
                            minHeight: 200,
                            display: 'flex',
                            flexDirection: 'column',
                            justifyContent: 'center'
                        }}>
                            <Typography variant="h6" color="text.secondary" gutterBottom>
                                Kapcsolat
                            </Typography>
                            <Typography variant="body1" color="text.secondary">
                                A kapcsolatfelvétel funkcionalitás hamarosan elérhető lesz.
                            </Typography>
                            <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
                                Addig is foglalj órát az elérhető időpontok közül!
                            </Typography>
                        </Box>
                    </TabPanel>
                </Paper>
            </Box>
        </Container>
    );
};

export default TeacherDetails;