import {
    Avatar,
    Box,
    Button,
    Chip,
    Container,
    Divider,
    Paper,
    Rating,
    Skeleton,
    Tab,
    Tabs,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import TabPanel from "../components/common/TabPanel";
import { useTeacherDetails } from "../hooks/teacherQueries";

const TeacherDetails: FunctionComponent = () => {
    const subjects = ['kémia', 'fizika'];
    const theme = useTheme();
    const { userId } = useParams<{ userId: string }>();
    const { data: teacher, isLoading, isError } = useTeacherDetails(userId || '');
    const [tabValue, setTabValue] = useState(0);
    const navigate = useNavigate();

    const handleTabChange = (_event: React.SyntheticEvent, newValue: number) => {
        setTabValue(newValue);
    };

    if (isError) {
        return (
            <Container sx={{ textAlign: 'center', py: 8 }}>
                <Typography variant="h4" color="error">
                    Hiba történt az adatok betöltése közben
                </Typography>
                <Button sx={{ mt: 2 }} onClick={() => navigate('/')}>
                    Vissza a főoldalra
                </Button>
            </Container>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ py: 4 }}>
            <Paper sx={{
                borderRadius: theme.shape.borderRadius,
                overflow: 'hidden',
                boxShadow: theme.shadows[3]
            }}>
                {/* Header Section */}
                <Box sx={{
                    bgcolor: theme.palette.mode === 'dark'
                        ? theme.palette.background.paper
                        : theme.palette.primary.light,
                    p: 4,
                    position: 'relative'
                }}>
                    <Box sx={{
                        display: 'flex',
                        gap: 4,
                        flexDirection: { xs: 'column', md: 'row' },
                        alignItems: 'center'
                    }}>
                        {isLoading ? (
                            <Skeleton
                                variant="circular"
                                width={150}
                                height={150}
                            />
                        ) : (
                            <Avatar
                                src={teacher?.profilePicture || '/src/assets/images/exampleteacher1.jpg'}
                                sx={{
                                    width: 150,
                                    height: 150,
                                    border: `4px solid ${theme.palette.background.paper}`
                                }}
                            >
                                {teacher?.firstName?.[0]}{teacher?.surName?.[0]}
                            </Avatar>
                        )}

                        <Box sx={{ flex: 1 }}>
                            {isLoading ? (
                                <>
                                    <Skeleton width="60%" height={40} />
                                    <Skeleton width="40%" height={30} sx={{ mt: 1 }} />
                                </>
                            ) : (
                                <>
                                    <Typography variant="h2" component="h1">
                                        {teacher?.firstName} {teacher?.surName}
                                        {teacher?.nickName && (
                                            <Typography
                                                variant="h4"
                                                component="span"
                                                sx={{ ml: 2, opacity: 0.8 }}
                                            >
                                                "{teacher.nickName}"
                                            </Typography>
                                        )}
                                    </Typography>
                                    <Box sx={{ display: 'flex', alignItems: 'center', mt: 1 }}>
                                        <Rating
                                            value={Number(teacher?.rating) || 0}
                                            precision={0.5}
                                            readOnly
                                            size="large"
                                        />
                                        <Typography variant="body1" sx={{ ml: 2 }}>
                                            ({teacher?.rating || 'Nincs'} értékelés)
                                        </Typography>
                                    </Box>
                                </>
                            )}
                        </Box>
                    </Box>
                </Box>

                {/* Main Content */}
                <Box sx={{ bgcolor: 'background.paper' }}>
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
                        <Tab label="Leírás" />
                        <Tab label="Időpontok" />
                        <Tab label="Vélemények" />
                    </Tabs>

                    {/* About Tab */}
                    <TabPanel value={tabValue} index={0}>
                        <Box sx={{ maxWidth: 800, mx: 'auto' }}>
                            {isLoading ? (
                                <>
                                    <Skeleton height={40} width="30%" />
                                    <Skeleton height={25} sx={{ mt: 2 }} />
                                    <Skeleton height={25} width="80%" />
                                </>
                            ) : (
                                <>
                                    <Typography variant="h4" gutterBottom>
                                        Bemutatkozás
                                    </Typography>
                                    <Typography variant="body1" paragraph>
                                        {teacher?.description || 'Nincs elérhető leírás'}
                                    </Typography>

                                    <Divider sx={{ my: 4 }} />

                                    <Typography variant="h4" gutterBottom>
                                        Tantárgyak
                                    </Typography>
                                    <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                                        {subjects.map((subject, index) => (
                                            <Chip
                                                key={index}
                                                label={subject}
                                                sx={{
                                                    bgcolor: theme.palette.primary.light,
                                                    color: theme.palette.getContrastText(
                                                        theme.palette.primary.light
                                                    )
                                                }}
                                            />
                                        ))}
                                        {/*{teacher?.subjects?.map((subject, index) => (
                                            <Chip
                                                key={index}
                                                label={subject}
                                                sx={{
                                                    bgcolor: theme.palette.primary.light,
                                                    color: theme.palette.getContrastText(
                                                        theme.palette.primary.light
                                                    )
                                                }}
                                            />
                                        ))}*/}
                                    </Box>

                                    <Divider sx={{ my: 4 }} />

                                    <Box sx={{
                                        display: 'grid',
                                        gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)' },
                                        gap: 3
                                    }}>
                                        {/*<Box>
                                            <Typography variant="h6" gutterBottom>
                                                Óradíj
                                            </Typography>
                                            <Typography variant="body1">
                                                {teacher?.hourlyRate
                                                    ? `${teacher.hourlyRate} Ft/óra`
                                                    : 'Egyedi árazás'}
                                            </Typography>
                                        </Box>*/}
                                        <Box>
                                            <Typography variant="h6" gutterBottom>
                                                Elérhetőség
                                            </Typography>
                                            <Typography variant="body1">
                                                {teacher?.location || 'Online'}
                                            </Typography>
                                        </Box>
                                    </Box>
                                </>
                            )}
                        </Box>
                    </TabPanel>

                    {/* Schedule Tab */}
                    <TabPanel value={tabValue} index={1}>
                        <Box sx={{
                            maxWidth: 800,
                            mx: 'auto',
                            minHeight: 400,
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center'
                        }}>
                            <Typography variant="h6" color="text.secondary">
                                Időpontválasztó (hamarosan...)
                            </Typography>
                        </Box>
                    </TabPanel>

                    {/* Reviews Tab */}
                    <TabPanel value={tabValue} index={2}>
                        <Box sx={{ maxWidth: 800, mx: 'auto' }}>
                            <Typography variant="h4" gutterBottom>
                                Vélemények
                            </Typography>
                            <Box sx={{
                                bgcolor: 'background.paper',
                                p: 3,
                                borderRadius: 2,
                                boxShadow: theme.shadows[1]
                            }}>
                                <Typography color="text.secondary">
                                    Még nincsenek vélemények. Legyél te az első!
                                </Typography>
                            </Box>
                        </Box>
                    </TabPanel>
                </Box>

                {/* Fixed CTA */}
                <Box sx={{
                    position: 'sticky',
                    bottom: 0,
                    bgcolor: 'background.paper',
                    borderTop: `1px solid ${theme.palette.divider}`,
                    p: 2,
                    zIndex: theme.zIndex.appBar
                }}>
                    <Container maxWidth="lg">
                        <Box sx={{
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center'
                        }}>
                            <Typography variant="h6">
                                {teacher?.hourlyRate
                                    ? `${teacher.hourlyRate} Ft/óra`
                                    : 'Egyedi árazás'}
                            </Typography>
                            <Button
                                variant="contained"
                                size="large"
                                onClick={() => navigate(`/book/${userId}`)}
                                sx={{
                                    bgcolor: theme.palette.secondary.main,
                                    '&:hover': {
                                        bgcolor: theme.palette.secondary.dark
                                    }
                                }}
                            >
                                Óra foglalása
                            </Button>
                        </Box>
                    </Container>
                </Box>
            </Paper>
        </Container>
    );
};

export default TeacherDetails;