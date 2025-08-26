import { createContext } from "react";

export enum IThemeMode {
    LIGHT = "light",
    DARK = "dark",
    SYSTEM = "system"
}

export interface IThemeContext {
    themeMode: IThemeMode;
    switchThemeMode: (mode: IThemeMode) => void;
}

export const AppThemeContext = createContext<IThemeContext | null>(null);