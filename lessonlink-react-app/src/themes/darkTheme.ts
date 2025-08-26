import { createTheme, Theme } from "@mui/material";

export const AppDarkTheme: Theme = createTheme({
    palette: {
        mode: "dark",
        primary: {
            main: "#5B9BD5", // Világosabb kék - jobban látható sötét hátteren
            light: "#89CFF0",
            dark: "#357ABD",
            contrastText: "#1A202C"
        },
        secondary: {
            main: "#66BB6A", // Lágy zöld - jó kontraszt a sötét hátteren
            light: "#81C784",
            dark: "#4CAF50",
            contrastText: "#1A202C"
        },
        error: {
            main: "#EF5350",
            light: "#FF867C",
            dark: "#D32F2F",
            contrastText: "#FFFFFF"
        },
        warning: {
            main: "#FFB74D",
            light: "#FFE082",
            dark: "#FFA726",
            contrastText: "#1A202C"
        },
        info: {
            main: "#64B5F6",
            light: "#90CAF9",
            dark: "#42A5F5",
            contrastText: "#1A202C"
        },
        success: {
            main: "#81C784",
            light: "#A5D6A7",
            dark: "#66BB6A",
            contrastText: "#1A202C"
        },
        background: {
            default: "#1A202C", // Mély szürkés-kék - fő háttér
            paper: "#2D3748"     // Közepes szürke felületeknek
        },
        text: {
            primary: "#E2E8F0",   // Világos szürke elsődleges szöveg
            secondary: "#CBD5E0", // Halványabb szövegek
            disabled: "#718096"   // Letiltott elemek szövege
        },
        divider: "#4A5568",
        action: {
            active: "#5B9BD5",
            hover: "#2D3748",
            selected: "#4A5568",
            disabled: "#4A5568"
        }
    },
    typography: {
        fontFamily: [
            '"Inter"',
            'system-ui',
            '-apple-system',
            'BlinkMacSystemFont',
            '"Segoe UI"',
            'Roboto',
            '"Helvetica Neue"',
            'Arial',
            'sans-serif'
        ].join(','),
    },
    shape: {
        borderRadius: 8
    }
})