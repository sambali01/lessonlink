import { Box, Slider } from '@mui/material';
import { FunctionComponent, useState } from 'react';

interface PriceSliderProps {
    minPrice: number;
    maxPrice: number;
    minimumDistance: number;
    onValueChange?: (newValue: number[]) => void;
}

const PriceSlider: FunctionComponent<PriceSliderProps> = ({ minPrice, maxPrice, minimumDistance, onValueChange }) => {
    const [boundaries, setBoundaries] = useState<number[]>([minPrice, maxPrice]);

    const handleChange = (_event: Event, newValue: number[], activeThumb: number) => {
        if (activeThumb === 0) {
            setBoundaries([Math.min(newValue[0], boundaries[1] - minimumDistance), boundaries[1]]);
        } else {
            setBoundaries([boundaries[0], Math.max(newValue[1], boundaries[0] + minimumDistance)]);
        }

        onValueChange?.(boundaries);
    };

    const valuetext = (value: number) => {
        return `${value} Ft`;
    }

    return (
        <Box>
            <Slider
                getAriaLabel={() => 'Price'}
                value={boundaries}
                min={minPrice}
                max={maxPrice}
                step={500}
                shiftStep={500}
                onChange={handleChange}
                valueLabelDisplay="auto"
                getAriaValueText={valuetext}
                disableSwap
            />
        </Box>
    );
}

export default PriceSlider
