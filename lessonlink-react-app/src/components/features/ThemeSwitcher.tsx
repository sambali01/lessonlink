import PaletteIcon from "@mui/icons-material/Palette";
import { Button, IconButton, Menu, MenuItem, useMediaQuery } from "@mui/material";
import { FunctionComponent, useContext, useRef, useState } from "react";
import { AppThemeContext, IThemeContext, IThemeMode } from "../../contexts/AppThemeContext";
import "./ThemeSwitcher.less"

const ThemeSwitcher: FunctionComponent = () => {
    const buttonRef = useRef<HTMLButtonElement>(null);
    const [openMenu, setOpenMenu] = useState<boolean>(false);
    const { themeMode, switchThemeMode } = useContext(AppThemeContext) as IThemeContext;

    const isMobile = useMediaQuery('(max-width:600px)');

    const handleOpen = () => {
        setOpenMenu(true);
    }

    const handleClose = () => {
        setOpenMenu(false);
    }

    const handleSwitchTheme = (mode: IThemeMode) => {
        switchThemeMode(mode);
        handleClose();
    }

    return (
        <>
            {isMobile ? (
                <IconButton
                    onClick={handleOpen}
                    ref={buttonRef}
                    color="primary"
                    sx={{
                        ml: 2,
                        '&:hover': { backgroundColor: 'action.hover' }
                    }}
                >
                    <PaletteIcon />
                </IconButton>
            ) : (
                <Button
                    variant="contained"
                    onClick={handleOpen}
                    startIcon={<PaletteIcon />}
                    ref={buttonRef}
                    sx={{
                        ml: 2
                    }}
                >
                    Témaváltás
                </Button>
            )}
            <Menu open={openMenu} anchorEl={buttonRef.current} onClose={handleClose}>
                <MenuItem
                    onClick={() => handleSwitchTheme(IThemeMode.LIGHT)}
                    selected={themeMode === IThemeMode.LIGHT}
                >
                    Világos
                </MenuItem>
                <MenuItem
                    onClick={() => handleSwitchTheme(IThemeMode.DARK)}
                    selected={themeMode === IThemeMode.DARK}
                >
                    Sötét
                </MenuItem>
                <MenuItem
                    onClick={() => handleSwitchTheme(IThemeMode.SYSTEM)}
                    selected={themeMode === IThemeMode.SYSTEM}
                >
                    Rendszer
                </MenuItem>
            </Menu>
        </>
    )
}

export default ThemeSwitcher