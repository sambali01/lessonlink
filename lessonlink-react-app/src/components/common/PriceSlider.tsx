import { Box, Slider } from '@mui/material';
import { FunctionComponent, useState } from 'react';

interface PriceSliderProps {
    minPrice: number;
    maxPrice: number;
    minimumDistance: number;
    onValueChange?: (newValue: number[]) => void;
}

const PriceSlider: FunctionComponent<PriceSliderProps> = ({ minPrice, maxPrice, minimumDistance, onValueChange }) => {
    const [value, setValue] = useState<number[]>([minPrice, maxPrice]);

    const handleChange = (_event: Event, newValue: number[], activeThumb: number) => {
        if (activeThumb === 0) {
            setValue([Math.min(newValue[0], value[1] - minimumDistance), value[1]]);
        } else {
            setValue([value[0], Math.max(newValue[1], value[0] + minimumDistance)]);
        }

        onValueChange?.(value);
    };

    const valuetext = (value: number) => {
        return `${value} Ft`;
    }

    return (
        <Box>
            <Slider
                getAriaLabel={() => 'Price'}
                value={value}
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
