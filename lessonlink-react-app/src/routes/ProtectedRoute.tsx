import { PropsWithChildren } from 'react';
import { UserAuth } from '../models/User.ts';
import { useAuth } from '../hooks/useAuth.ts';
import { Navigate } from 'react-router-dom';
import PermissionDenied from '../pages/PermissionDenied.tsx';
import Loading from '../pages/Loading.tsx';

type ProtectedRouteProps = PropsWithChildren & {
    allowedRoles: UserAuth['roles'];
};

export default function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
    const { currentUserAuth } = useAuth();

    if (currentUserAuth === undefined) {
        return (
            <Loading />
        );
    }

    if (currentUserAuth === null) {
        return <Navigate to="/login" replace />;
    }

    const userHasAllowedRole = currentUserAuth.roles.some(role => allowedRoles.includes(role));

    if (!userHasAllowedRole) {
        return (
            <PermissionDenied />
        );
    }

    return children;
};