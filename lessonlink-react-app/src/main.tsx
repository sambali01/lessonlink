import { QueryClientProvider } from '@tanstack/react-query';
//import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import AuthProvider from './components/auth/AuthProvider';
import { queryClient } from './configs/queryConfig';
import './main.less';
import { ThemeContextProvider } from './components/theme/ThemeContextProvider';

ReactDOM.createRoot(document.getElementById('root')!).render(
    //<React.StrictMode>
    <ThemeContextProvider>
        <QueryClientProvider client={queryClient}>
            <AuthProvider>
                <App />
            </AuthProvider>
        </QueryClientProvider>
    </ThemeContextProvider>
    //</React.StrictMode>
);