import { AppBar, Avatar, Box, Button, IconButton, Menu, MenuItem, Toolbar, Typography, useTheme } from "@mui/material";
import { FunctionComponent, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../hooks/useAuth";
import { useFindUserById } from "../../hooks/userQueries";
import ThemeSwitcher from "../features/ThemeSwitcher";
import "./Header.less";
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
            <Toolbar className="header-toolbar">
                <Typography
                    variant="h6"
                    component={Link}
                    to="/"
                    sx={{
                        marginRight: 4,
                        textDecoration: 'none',
                        color: theme.palette.text.primary,
                        fontFamily: theme.typography.fontFamily,
                        fontWeight: 700,
                        letterSpacing: '-0.5px'
                    }}
                >
                    LessonLink
                </Typography>
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
                </Box>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                    <ThemeSwitcher />

                    {currentUserAuth ? (
                        <div className="user-section">
                            <IconButton onClick={handleMenuOpen} sx={{ p: 0 }}>
                                <Avatar
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
                        </div>
                    ) : (
                        <div className="auth-buttons">
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
                        </div>
                    )}
                </Box>
            </Toolbar>
        </AppBar>
    );
};

export default Header;