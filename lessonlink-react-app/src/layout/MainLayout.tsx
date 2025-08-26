import { Box, useTheme } from '@mui/material';
import { Outlet } from 'react-router-dom';
import Footer from '../components/common/Footer';
import Header from '../components/common/Header';

export default function MainLayout() {
    const theme = useTheme();

    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                minHeight: '100vh'
            }}
        >
            <Header />
            <Box
                sx={{
                    display: 'flex',
                    flex: 1,
                    flexDirection: 'column',
                    bgcolor: theme.palette.background.default
                }}
            >
                <Outlet />
            </Box>
            <Footer />
        </Box>
    );
};