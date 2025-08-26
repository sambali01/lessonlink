import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export const UnauthenticatedRoute = () => {
    const { currentUserAuth } = useAuth();

    if (currentUserAuth === undefined) {
        return <div>Loading...</div>;
    }

    return currentUserAuth ? <Navigate to="/dashboard" replace /> : <Outlet />;
};