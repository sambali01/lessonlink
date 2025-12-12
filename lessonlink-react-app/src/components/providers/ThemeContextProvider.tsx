import { Theme, ThemeProvider, useMediaQuery } from "@mui/material";
import { FunctionComponent, PropsWithChildren, useEffect, useState } from "react";
import { AppThemeContext, IThemeMode } from "../../contexts/AppThemeContext";
import { AppDarkTheme } from "../../themes/darkTheme";
import { AppLightTheme } from "../../themes/lightTheme";

export const ThemeContextProvider: FunctionComponent<PropsWithChildren> = ({ children }) => {
    const [themeMode, setThemeMode] = useState<IThemeMode>(IThemeMode.LIGHT);
    const [theme, setTheme] = useState<Theme>(AppLightTheme);

    const SYSTEM_THEME: Exclude<IThemeMode, IThemeMode.SYSTEM> = useMediaQuery(
        "(prefers-color-scheme: dark)"
    ) ? IThemeMode.DARK : IThemeMode.LIGHT;

    useEffect(() => {
        const storedTheme = _getStoredThemeMode();
        setThemeMode(storedTheme);
    }, []);

    useEffect(() => {
        switch (themeMode) {
            case IThemeMode.LIGHT:
                setTheme(AppLightTheme);
                break;
            case IThemeMode.DARK:
                setTheme(AppDarkTheme);
                break;
            case IThemeMode.SYSTEM:
                switch (SYSTEM_THEME) {
                    case IThemeMode.LIGHT:
                        setTheme(AppLightTheme);
                        break;
                    case IThemeMode.DARK:
                        setTheme(AppDarkTheme);
                        break;
                }
                break;
            default:
                setTheme(AppLightTheme);
                break;
        }
    }, [themeMode, SYSTEM_THEME]);

    const _getStoredThemeMode = (): IThemeMode => {
        const storedTheme = localStorage.getItem("themeMode") as IThemeMode;
        return storedTheme ?? IThemeMode.LIGHT;
    }

    const _setStoredThemeMode = (mode: IThemeMode) => {
        localStorage.setItem("themeMode", mode);
    }

    const switchThemeMode = (mode: IThemeMode) => {
        setThemeMode(mode);
        _setStoredThemeMode(mode);
    }

    return (
        <AppThemeContext.Provider
            value={{
                themeMode,
                switchThemeMode
            }}
        >
            <ThemeProvider theme={theme}>
                {children}
            </ThemeProvider>
        </AppThemeContext.Provider>
    );
}