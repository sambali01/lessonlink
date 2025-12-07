import {
    Autocomplete,
    Box,
    Button,
    FormControlLabel,
    FormLabel,
    Radio,
    RadioGroup,
    TextField,
    Typography,
    useTheme
} from "@mui/material";
import { FunctionComponent, useState } from "react";
import { useSubjects } from "../../../hooks/subjectQueries";
import { TeacherSearchRequest } from "../../../models/TeacherSearchRequest";
import { TEACHER_SEARCH_PAGE_SIZE } from "../../../utils/constants";
import { TeachingMethod } from "../../../utils/enums";
import { convertBoolsToTeachingMethod, convertTeachingMethodToBools } from "../../../utils/teachingMethodConverters";
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
    const [resetTrigger, setResetTrigger] = useState(0);

    // Filter states
    const [searchText, setSearchText] = useState(initialFilters.searchText || '');
    const [subjects, setSubjects] = useState(initialFilters.subjects || []);
    const [minPrice, setMinPrice] = useState(initialFilters.minPrice || 0);
    const [maxPrice, setMaxPrice] = useState(initialFilters.maxPrice || 20000);
    const [location, setLocation] = useState(initialFilters.location || '');

    const initialTeachingMethod = convertBoolsToTeachingMethod(initialFilters.acceptsOnline, initialFilters.acceptsInPerson);
    const [teachingMethod, setTeachingMethod] = useState(initialTeachingMethod);

    // Handle search button click
    const handleSearchClick = () => {
        const { acceptsOnline, acceptsInPerson } = convertTeachingMethodToBools(teachingMethod);

        const searchFilters: TeacherSearchRequest = {
            searchText,
            subjects,
            minPrice,
            maxPrice,
            acceptsOnline,
            acceptsInPerson,
            location,
            page: 1,
            pageSize: TEACHER_SEARCH_PAGE_SIZE
        };

        onSearch(searchFilters);
    };

    // Handle clear filters
    const handleClearFilters = () => {
        setSearchText('');
        setSubjects([]);
        setResetTrigger(prev => prev + 1);
        setLocation('');
        setTeachingMethod(initialTeachingMethod);
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
                value={searchText}
                onChange={(e) => setSearchText(e.target.value)}
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
                slotProps={{
                    chip: {
                        size: "small",
                        sx: {
                            bgcolor: theme.palette.primary.main,
                            color: theme.palette.getContrastText(theme.palette.primary.main)
                        }
                    }
                }}
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
                    resetTrigger={resetTrigger}
                />
            </Box>

            <Box sx={{ mb: 2 }}>
                <FormLabel component="legend">Óraadás módja</FormLabel>
                <RadioGroup
                    value={teachingMethod}
                    onChange={(e) => setTeachingMethod(e.target.value as TeachingMethod)}
                >
                    <FormControlLabel
                        value={TeachingMethod.Online}
                        control={<Radio />}
                        label="Csak online"
                    />
                    <FormControlLabel
                        value={TeachingMethod.InPerson}
                        control={<Radio />}
                        label="Csak személyesen"
                    />
                    <FormControlLabel
                        value={TeachingMethod.Both}
                        control={<Radio />}
                        label="Online vagy személyesen"
                    />
                </RadioGroup>
            </Box>

            {(teachingMethod === TeachingMethod.InPerson || teachingMethod === TeachingMethod.Both) && (
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