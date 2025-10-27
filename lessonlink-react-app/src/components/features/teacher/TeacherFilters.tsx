import {
    Autocomplete,
    Box,
    Checkbox,
    Chip,
    FormControlLabel,
    Rating,
    TextField,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent } from "react";
import { TeacherSearchFilters } from "../../../models/TeacherSearchFilters";
import PriceSlider from "../../common/PriceSlider";

interface TeacherFiltersProps {
    filters: TeacherSearchFilters;
    onFilterChange: (newFilters: TeacherSearchFilters) => void;
    subjectsOptions: string[];
}

const TeacherFilters: FunctionComponent<TeacherFiltersProps> = ({
    filters,
    onFilterChange,
    subjectsOptions
}) => {
    const theme = useTheme();

    return (
        <Box sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom sx={{ mb: 2 }}>
                Szűrők
            </Typography>

            {/* Name */}
            <TextField
                fullWidth
                label="Név keresése"
                variant="outlined"
                value={filters.searchQuery}
                onChange={(e) => onFilterChange({ ...filters, searchQuery: e.target.value })}
                sx={{ mb: 3 }}
            />

            {/* Subjects */}
            <Autocomplete
                multiple
                options={subjectsOptions}
                value={filters.subjects}
                onChange={(_, newValue) => onFilterChange({ ...filters, subjects: newValue })}
                renderInput={(params) => (
                    <TextField
                        {...params}
                        label="Tantárgyak"
                        placeholder="Válassz tantárgyakat"
                    />
                )}
                renderTags={(value, getTagProps) =>
                    value.map((option, index) => (
                        <Chip
                            {...getTagProps({ index })}
                            key={option}
                            label={option}
                            size="small"
                            sx={{
                                bgcolor: theme.palette.primary.light,
                                color: theme.palette.getContrastText(theme.palette.primary.light)
                            }}
                        />
                    ))
                }
                sx={{ mb: 3 }}
            />

            {/* Price */}
            <Box sx={{ mb: 3 }}>
                <Typography gutterBottom>Óradíj tartomány (Ft/óra)</Typography>
                <PriceSlider
                    minPrice={0}
                    maxPrice={20000}
                    minimumDistance={500}
                    onValueChange={(newValue) => {
                        onFilterChange({ ...filters, minPrice: newValue[0], maxPrice: newValue[1] });
                    }}
                />
            </Box>

            {/* Minimum rating */}
            <Box sx={{ mb: 3 }}>
                <Typography gutterBottom>Minimum értékelés</Typography>
                <Rating
                    value={filters.minRating}
                    onChange={(_, newValue) => {
                        onFilterChange({ ...filters, minRating: newValue ?? 0 });
                    }}
                />
            </Box>

            {/* Online and/or in person */}
            <Box>
                <Typography gutterBottom sx={{ mb: 1 }}>Óraadás módja</Typography>
                <FormControlLabel
                    control={
                        <Checkbox
                            checked={filters.acceptsOnline}
                            onChange={(e) => onFilterChange({ ...filters, acceptsOnline: e.target.checked })}
                        />
                    }
                    label="Online"
                />
                <FormControlLabel
                    control={
                        <Checkbox
                            checked={filters.acceptsInPerson}
                            onChange={(e) => onFilterChange({ ...filters, acceptsInPerson: e.target.checked })}
                        />
                    }
                    label="Személyesen"
                />
            </Box>
        </Box>
    );
};

export default TeacherFilters;