import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import Loading from '../pages/Loading';

export const UnauthenticatedRoute = () => {
    const { currentUserAuth } = useAuth();

    if (currentUserAuth === undefined) {
        return (
            <Loading />
        );
    }

    return currentUserAuth ? <Navigate to="/dashboard" replace /> : <Outlet />;
};