import { QueryClientProvider } from '@tanstack/react-query';
//import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import AuthProvider from './components/providers/AuthProvider';
import { queryClient } from './configs/queryConfig';
import './main.less';
import { ThemeContextProvider } from './components/providers/ThemeContextProvider';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { hu } from 'date-fns/locale';
import { ErrorBoundary } from 'react-error-boundary';
import ServerError from './pages/ServerError';

ReactDOM.createRoot(document.getElementById('root')!).render(
    //<React.StrictMode>
    <ThemeContextProvider>
        <LocalizationProvider dateAdapter={AdapterDateFns} adapterLocale={hu}>
            <QueryClientProvider client={queryClient}>
                <AuthProvider>
                    <ErrorBoundary FallbackComponent={ServerError}>
                        <App />
                    </ErrorBoundary>
                </AuthProvider>
            </QueryClientProvider>
        </LocalizationProvider>
    </ThemeContextProvider>
    //</React.StrictMode>
);