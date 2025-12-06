import {
    Autocomplete,
    Box,
    Button,
    Chip,
    FormControlLabel,
    FormLabel,
    Radio,
    RadioGroup,
    Rating,
    TextField,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent, useState } from "react";
import { PAGE_SIZE } from "../../../constants/searchDefaults";
import { TeachingMethod } from "../../../enums/TeachingMethod";
import { TeacherSearchRequest } from "../../../models/TeacherSearchRequest";
import { convertBoolsToTeachingMethod, convertTeachingMethodToBools } from "../../../utils/teachingMethodConverters";
import { useSubjects } from "../../../hooks/subjectQueries";
import PriceSlider from "../../common/PriceSlider";

interface TeacherFiltersProps {
    initialFilters: TeacherSearchRequest;
    onSearch: (searchFilters: TeacherSearchRequest) => void;
}

const TeacherFilters: FunctionComponent<TeacherFiltersProps> = ({
    initialFilters,
    onSearch
}) => {
    const theme = useTheme();
    const { data: subjectsOptions = [] } = useSubjects();

    // Filter states
    const [searchQuery, setSearchQuery] = useState(initialFilters.searchQuery || '');
    const [subjects, setSubjects] = useState(initialFilters.subjects || []);
    const [minPrice, setMinPrice] = useState(initialFilters.minPrice || 0);
    const [maxPrice, setMaxPrice] = useState(initialFilters.maxPrice || 20000);
    const [minRating, setMinRating] = useState(initialFilters.minRating || 0);
    const [location, setLocation] = useState(initialFilters.location || '');
    const [teachingMethod, setTeachingMethod] = useState(
        convertBoolsToTeachingMethod(initialFilters.acceptsOnline, initialFilters.acceptsInPerson)
    );

    // Handle search button click
    const handleSearchClick = () => {
        const { acceptsOnline, acceptsInPerson } = convertTeachingMethodToBools(teachingMethod);

        const searchFilters: TeacherSearchRequest = {
            searchQuery,
            subjects,
            minPrice,
            maxPrice,
            minRating,
            acceptsOnline,
            acceptsInPerson,
            location,
            page: 1,
            pageSize: PAGE_SIZE
        };

        onSearch(searchFilters);
    };

    // Handle clear filters
    const handleClearFilters = () => {
        setSearchQuery('');
        setSubjects([]);
        setMinPrice(0);
        setMaxPrice(20000);
        setMinRating(0);
        setLocation('');
        setTeachingMethod(TeachingMethod.BOTH);
    };

    return (
        <Box sx={{ p: 1 }}>
            <Button
                fullWidth
                variant="outlined"
                color="info"
                onClick={handleClearFilters}
                sx={{ verticalAlign: 'middle', mb: 2 }}
            >
                Szűrők törlése
            </Button>

            <TextField
                fullWidth
                label="Név"
                variant="outlined"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                placeholder="Írj be egy nevet"
                sx={{ mb: 2 }}
            />

            <Autocomplete
                multiple
                options={subjectsOptions}
                value={subjects}
                onChange={(_, newValue) => setSubjects(newValue)}
                renderInput={(params) => (
                    <TextField
                        {...params}
                        label="Tantárgyak"
                    />
                )}
                renderValue={(value, getTagProps) =>
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
                sx={{ mb: 2 }}
            />

            <Box sx={{ mb: 2 }}>
                <Typography sx={{ mb: 1 }}>Óradíj tartomány (Ft/óra)</Typography>
                <PriceSlider
                    minPrice={0}
                    maxPrice={20000}
                    minimumDistance={500}
                    onValueChange={(newBoundaries) => {
                        setMinPrice(newBoundaries[0]);
                        setMaxPrice(newBoundaries[1]);
                    }}
                />
            </Box>

            <Box sx={{ mb: 2 }}>
                <Typography sx={{ mb: 1 }}>Minimum értékelés</Typography>
                <Rating
                    value={minRating}
                    onChange={(_, newValue) => {
                        setMinRating(newValue ?? 0);
                    }}
                />
            </Box>

            <Box sx={{ mb: 2 }}>
                <FormLabel component="legend">Óraadás módja</FormLabel>
                <RadioGroup
                    value={teachingMethod}
                    onChange={(e) => setTeachingMethod(e.target.value as TeachingMethod)}
                >
                    <FormControlLabel
                        value={TeachingMethod.ONLINE}
                        control={<Radio />}
                        label="Csak online"
                    />
                    <FormControlLabel
                        value={TeachingMethod.IN_PERSON}
                        control={<Radio />}
                        label="Csak személyesen"
                    />
                    <FormControlLabel
                        value={TeachingMethod.BOTH}
                        control={<Radio />}
                        label="Online vagy személyesen"
                    />
                </RadioGroup>
            </Box>

            {(teachingMethod === TeachingMethod.IN_PERSON || teachingMethod === TeachingMethod.BOTH) && (
                <Box sx={{ mb: 2 }}>
                    <TextField
                        fullWidth
                        label="Helyszín"
                        variant="outlined"
                        value={location}
                        onChange={(e) => setLocation(e.target.value)}
                        placeholder="Írj be egy településnevet"
                    />
                </Box>
            )}

            <Button
                fullWidth
                variant="contained"
                color="primary"
                onClick={handleSearchClick}
            >
                Keresés
            </Button>
        </Box>
    );
};

export default TeacherFilters;