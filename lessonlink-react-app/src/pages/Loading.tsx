import { Container, Box, CircularProgress, Typography } from '@mui/material'

const Loading = () => {
    return (
        <Container maxWidth="sm">
            <Box
                sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    justifyContent: 'center',
                    minHeight: 'calc(100vh - 200px)',
                    gap: 2
                }}
            >
                <CircularProgress size={40} />
                <Typography variant="body1" color="text.secondary">
                    Betöltés...
                </Typography>
            </Box>
        </Container>
    )
}

export default Loading