import { Box } from '@mui/material';
import { FunctionComponent } from 'react';
import HeroSection from '../components/features/home/HeroSection';
import FeaturedTeachers from '../components/features/home/FeaturedTeachers';

const Home: FunctionComponent = () => {
    return (
        <Box
            sx={{
                flex: 1
            }}
        >
            <HeroSection />
            <FeaturedTeachers />
        </Box>
    );
};

export default Home;