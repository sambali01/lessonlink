import {
    Box,
    Card,
    CardContent,
    CircularProgress,
    Container,
    Typography,
    useTheme
} from "@mui/material";
import {
    CalendarMonth as CalendarIcon,
    PersonSearch as SearchIcon,
    MenuBook as MenuBookIcon,
    BookOnline as BookOnlineIcon,
    AddCircleOutline as AddCircleIcon
} from '@mui/icons-material';
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { Role } from "../models/User";
import { useFindUserById } from "../hooks/userQueries";

export default function Dashboard() {
    const theme = useTheme();
    const navigate = useNavigate();
    const { currentUserAuth } = useAuth();
    const { data: user, isLoading: userLoading } = useFindUserById(currentUserAuth?.userId || '');

    const isTeacher = currentUserAuth?.roles.includes(Role.Teacher);
    const isStudent = currentUserAuth?.roles.includes(Role.Student);

    const quickActions = [
        ...(isStudent ? [{
            title: 'Tanár keresése',
            description: 'Találd meg a számodra ideális tanárt',
            icon: <SearchIcon sx={{ fontSize: 64 }} />,
            color: theme.palette.primary.main,
            action: () => navigate('/teachers')
        },
        {
            title: 'Foglalásaim',
            description: 'Tekintsd meg az aktuális foglalásaidat',
            icon: <BookOnlineIcon sx={{ fontSize: 64 }} />,
            color: theme.palette.primary.main,
            action: () => navigate('/my-bookings')
        }] : []),
        ...(isTeacher ? [{
            title: 'Óraidőpontjaim',
            description: 'Kezeld az óraidőpontjaidat',
            icon: <CalendarIcon sx={{ fontSize: 64 }} />,
            color: theme.palette.primary.main,
            action: () => navigate('/my-slots')
        },
        {
            title: 'Új időpont',
            description: 'Hozz létre új óraidőpontot',
            icon: <AddCircleIcon sx={{ fontSize: 64 }} />,
            color: theme.palette.primary.main,
            action: () => navigate('/my-slots', { state: { openModal: true } })
        }] : []),
        {
            title: 'Profilom',
            description: isTeacher ? 'Szerkeszd a profilodat és beállításaidat' : 'Szerkeszd a profilodat',
            icon: <MenuBookIcon sx={{ fontSize: 64 }} />,
            color: theme.palette.primary.main,
            action: () => navigate('/profile')
        }
    ];

    if (userLoading) {
        return <CircularProgress sx={{ display: 'block', mx: 'auto', my: 4 }} />;
    }

    return (
        <Container maxWidth="lg">
            <Box sx={{
                minHeight: '80vh',
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                py: 4
            }}>
                <Box sx={{
                    mb: 8,
                    textAlign: 'center',
                    animation: 'fadeIn 0.8s ease-in',
                    '@keyframes fadeIn': {
                        from: { opacity: 0, transform: 'translateY(20px)' },
                        to: { opacity: 1, transform: 'translateY(0)' }
                    }
                }}>
                    <Typography
                        variant="h3"
                        component="h1"
                        gutterBottom
                        sx={{
                            fontWeight: 500,
                            color: theme.palette.text.primary
                        }}
                    >
                        Üdvözlet, {user?.nickName}!
                    </Typography>
                    <Typography
                        variant="h6"
                        sx={{
                            color: theme.palette.text.secondary,
                            fontWeight: 400
                        }}
                    >
                        {isTeacher ? 'Kezeld óraidőpontjaidat és foglalásaidat' : 'Kezdj el tanulni a legjobb tanárokkal'}
                    </Typography>
                </Box>

                <Box sx={{
                    display: 'flex',
                    flexWrap: 'wrap',
                    gap: 3,
                    justifyContent: 'center',
                    maxWidth: '1000px',
                    mx: 'auto',
                    '@keyframes float': {
                        '0%, 100%': { transform: 'translateY(0px)' },
                        '50%': { transform: 'translateY(-10px)' }
                    }
                }}>
                    {quickActions.map((action, index) => (
                        <Card
                            key={index}
                            sx={{
                                width: 280,
                                height: 320,
                                cursor: 'pointer',
                                transition: 'all 0.3s ease',
                                animation: `fadeIn 0.8s ease-in ${index * 0.1}s both`,
                                '@keyframes fadeIn': {
                                    from: { opacity: 0, transform: 'translateY(20px)' },
                                    to: { opacity: 1, transform: 'translateY(0)' }
                                },
                                '&:hover': {
                                    transform: 'translateY(-12px) scale(1.02)',
                                    boxShadow: theme.shadows[12],
                                    '& .card-icon': {
                                        animation: 'float 2s ease-in-out infinite'
                                    }
                                }
                            }}
                            onClick={action.action}
                        >
                            <CardContent sx={{
                                height: '100%',
                                display: 'flex',
                                flexDirection: 'column',
                                alignItems: 'center',
                                justifyContent: 'center',
                                textAlign: 'center',
                                p: 4
                            }}>
                                <Box
                                    className="card-icon"
                                    sx={{
                                        color: action.color,
                                        mb: 3,
                                        transition: 'transform 0.3s ease'
                                    }}
                                >
                                    {action.icon}
                                </Box>
                                <Typography
                                    variant="h5"
                                    component="h2"
                                    gutterBottom
                                    sx={{
                                        fontWeight: 600,
                                        mb: 2
                                    }}
                                >
                                    {action.title}
                                </Typography>
                                <Typography
                                    variant="body2"
                                    sx={{
                                        color: theme.palette.text.secondary
                                    }}
                                >
                                    {action.description}
                                </Typography>
                            </CardContent>
                        </Card>
                    ))}
                </Box>
            </Box>
        </Container>
    );
}