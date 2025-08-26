import { Box } from '@mui/material';
import { FunctionComponent } from 'react';
import FeaturedTeachers from '../components/home/FeaturedTeachers';
import HeroSection from '../components/home/HeroSection';

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