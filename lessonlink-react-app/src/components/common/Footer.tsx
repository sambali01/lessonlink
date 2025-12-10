import { Box, Typography, useTheme } from "@mui/material";
import { FunctionComponent } from "react";

const Footer: FunctionComponent = () => {
    const theme = useTheme();

    return (
        <Box
            component="footer"
            sx={{
                py: 2,
                backgroundColor: theme.palette.background.paper,
                borderTop: `1px solid ${theme.palette.divider}`
            }}
        >
            <Box sx={{
                maxWidth: '1200px',
                margin: '0 auto',
                px: { xs: 2, sm: 4 },
                display: 'flex',
                flexDirection: 'column',
                gap: 1
            }}>
                <Typography
                    variant="body2"
                    color="textSecondary"
                    sx={{
                        textAlign: 'center',
                        fontFamily: theme.typography.fontFamily,
                        opacity: 0.8
                    }}
                >
                    © {new Date().getFullYear()} LessonLink
                </Typography>
            </Box>
        </Box>
    );
};

export default Footer;