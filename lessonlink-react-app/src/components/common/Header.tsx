import { AppBar, Avatar, Box, Button, IconButton, Menu, MenuItem, Toolbar, useTheme } from "@mui/material";
import { FunctionComponent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";
import { useFindUserById } from "../../hooks/userQueries";
import ThemeSwitcher from "./ThemeSwitcher";
import { Role } from "../../models/Role";

const Header: FunctionComponent = () => {
    const theme = useTheme();
    const navigate = useNavigate();
    const { currentUserAuth, handleLogout } = useAuth();
    const { data: user } = useFindUserById(currentUserAuth?.userId || '');
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

    const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleMenuClose = () => {
        setAnchorEl(null);
    };

    const handleProfileClick = () => {
        handleMenuClose();
        navigate('/profile');
    }

    const handleLogoutClick = () => {
        handleLogout();
        handleMenuClose();
        navigate("/");
    };

    return (
        <AppBar
            position="sticky"
            sx={{
                bgcolor: theme.palette.background.paper
            }}
        >
            <Toolbar sx={{
                display: 'flex',
                justifyContent: 'space-between',
                px: { xs: 2, sm: 4 }
            }}>
                <Box
                    component={Link}
                    to="/"
                    sx={{
                        marginRight: 4,
                        textDecoration: 'none',
                        display: 'flex',
                        alignItems: 'center'
                    }}
                >
                    <img
                        src="/LessonLink_logo.png"
                        alt="LessonLink"
                        style={{
                            height: '40px',
                            width: 'auto'
                        }}
                    />
                </Box>
                <Box sx={{ display: 'flex', flexGrow: 1, alignItems: 'center' }}>
                    {currentUserAuth && (
                        <Button
                            component={Link}
                            to="/dashboard"
                            sx={{
                                color: theme.palette.text.primary,
                                mx: 1,
                                '&:hover': {
                                    backgroundColor: theme.palette.action.hover
                                }
                            }}
                        >
                            Irányítópult
                        </Button>
                    )}

                    <Button
                        component={Link}
                        to="/teachers"
                        sx={{
                            color: theme.palette.text.primary,
                            mx: 1,
                            '&:hover': {
                                backgroundColor: theme.palette.action.hover
                            }
                        }}
                    >
                        Oktatók
                    </Button>

                    {currentUserAuth?.roles.includes(Role.Teacher) && (
                        <Button
                            component={Link}
                            to="/my-slots"
                            sx={{
                                color: theme.palette.text.primary,
                                mx: 1,
                                '&:hover': {
                                    backgroundColor: theme.palette.action.hover
                                }
                            }}
                        >
                            Óraidőpontjaim
                        </Button>
                    )}

                    {currentUserAuth?.roles.includes(Role.Student) && (
                        <Button
                            component={Link}
                            to="/my-bookings"
                            sx={{
                                color: theme.palette.text.primary,
                                mx: 1,
                                '&:hover': {
                                    backgroundColor: theme.palette.action.hover
                                }
                            }}
                        >
                            Saját foglalások
                        </Button>
                    )}
                </Box>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <ThemeSwitcher />

                    {currentUserAuth ? (
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                            <IconButton onClick={handleMenuOpen} sx={{ p: 0 }}>
                                <Avatar
                                    src={user?.imageUrl}
                                    sx={{
                                        bgcolor: theme.palette.primary.main,
                                        color: theme.palette.primary.contrastText,
                                        ml: 2,
                                        transition: 'transform 0.2s',
                                        '&:hover': {
                                            transform: 'scale(1.1)'
                                        }
                                    }}
                                >
                                    {user?.firstName?.[0]}{user?.surName?.[0]}
                                </Avatar>
                            </IconButton>
                            <Menu
                                anchorEl={anchorEl}
                                open={Boolean(anchorEl)}
                                onClose={handleMenuClose}
                            >
                                <MenuItem onClick={handleProfileClick}>
                                    Profil
                                </MenuItem>
                                <MenuItem onClick={handleLogoutClick} sx={{ color: theme.palette.error.main }}>
                                    Kijelentkezés
                                </MenuItem>
                            </Menu>
                        </Box>
                    ) : (
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                            <Button
                                variant="outlined"
                                color="primary"
                                onClick={() => navigate('/login')}
                                sx={{ ml: 2 }}
                            >
                                Belépés
                            </Button>
                            <Button
                                variant="contained"
                                color="secondary"
                                onClick={() => navigate('/register')}
                                sx={{ ml: 2 }}
                            >
                                Regisztráció
                            </Button>
                        </Box>
                    )}
                </Box>
            </Toolbar>
        </AppBar>
    );
};

export default Header;