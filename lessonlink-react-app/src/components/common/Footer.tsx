import { Box, Typography, useTheme } from "@mui/material";
import { FunctionComponent } from "react";
import "./Footer.less";

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
            <div className="footer-content">
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
                <div className="footer-links">
                    <Typography
                        variant="body2"
                        component="a"
                        href="/terms"
                        sx={{
                            color: theme.palette.text.secondary,
                            textDecoration: 'none',
                            '&:hover': { color: theme.palette.primary.main }
                        }}
                    >
                        Felhasználási feltételek
                    </Typography>
                    <Typography
                        variant="body2"
                        component="a"
                        href="/privacy"
                        sx={{
                            color: theme.palette.text.secondary,
                            textDecoration: 'none',
                            '&:hover': { color: theme.palette.primary.main }
                        }}
                    >
                        Adatvédelem
                    </Typography>
                </div>
            </div>
        </Box>
    );
};

export default Footer;