import { createTheme, Theme } from "@mui/material";

export const AppLightTheme: Theme = createTheme({
    palette: {
        primary: {
            main: "#2A5C8D", // Professzionális kék árnyalat - fő interakciós elemek
            light: "#5A8DB8",
            dark: "#1A4062",
            contrastText: "#FFFFFF"
        },
        secondary: {
            main: "#4CAF93", // Frissítő zöld - másodlagos CTA elemek
            light: "#7FC2B0",
            dark: "#357F6D",
            contrastText: "#FFFFFF"
        },
        error: {
            main: "#D32F2F",
            light: "#EF5350",
            dark: "#C62828",
            contrastText: "#FFFFFF"
        },
        warning: {
            main: "#F9A825",
            light: "#FFB74D",
            dark: "#F57F17",
            contrastText: "#212121"
        },
        info: {
            main: "#2979FF",
            light: "#448AFF",
            dark: "#1565C0",
            contrastText: "#FFFFFF"
        },
        success: {
            main: "#4CAF50",
            light: "#81C784",
            dark: "#388E3C",
            contrastText: "#FFFFFF"
        },
        background: {
            default: "#F8FAFC",
            paper: "#FFFFFF"
        },
        text: {
            primary: "#2D3748",
            secondary: "#718096",
            disabled: "#A0AEC0"
        },
        divider: "#E2E8F0",
        action: {
            active: "#2A5C8D",
            hover: "#F7FAFC",
            selected: "#EBF4FF",
            disabled: "#CBD5E0"
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