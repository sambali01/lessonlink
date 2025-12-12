import { Box, Slider } from '@mui/material';
import { FunctionComponent, useEffect, useState } from 'react';

interface PriceSliderProps {
    minPrice: number;
    maxPrice: number;
    minimumDistance: number;
    onValueChange?: (newValue: number[]) => void;
    resetTrigger?: number;
}

const PriceSlider: FunctionComponent<PriceSliderProps> = ({ minPrice, maxPrice, minimumDistance, onValueChange, resetTrigger }) => {
    const [boundaries, setBoundaries] = useState<number[]>([minPrice, maxPrice]);

    useEffect(() => {
        onValueChange?.(boundaries);
    }, [boundaries, onValueChange]);

    // Reset boundaries when resetTrigger changes
    useEffect(() => {
        if (resetTrigger !== undefined) {
            setBoundaries([minPrice, maxPrice]);
        }
    }, [resetTrigger, minPrice, maxPrice]);

    const handleChange = (_event: Event, newValue: number[], activeThumb: number) => {
        let newBoundaries: number[];

        if (activeThumb === 0) {
            const newMinValue = Math.min(newValue[0], boundaries[1] - minimumDistance);
            newBoundaries = [newMinValue, boundaries[1]];
        } else {
            const newMaxValue = Math.max(newValue[1], boundaries[0] + minimumDistance);
            newBoundaries = [boundaries[0], newMaxValue];
        }

        setBoundaries(newBoundaries);
    };

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
                getAriaValueText={(value: number) => `${value} Ft`}
                disableSwap
                marks={[
                    { value: minPrice, label: `${minPrice} Ft` },
                    { value: maxPrice, label: `${maxPrice} Ft` }
                ]}
            />
        </Box>
    );
}

export default PriceSlider
