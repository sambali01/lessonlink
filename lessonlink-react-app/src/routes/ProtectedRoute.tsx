import { PropsWithChildren } from 'react';
import { AuthDto } from '../dtos/AuthDto.ts';
import { useAuth } from '../hooks/useAuth.ts';
import { Navigate } from 'react-router-dom';

type ProtectedRouteProps = PropsWithChildren & {
    allowedRoles: AuthDto['roles'];
};

export default function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
    const { currentUserAuth } = useAuth();

    if (currentUserAuth === undefined) {
        return <div>Loading...</div>;
    }

    if (currentUserAuth === null) {
        return <Navigate to="/login" replace />;
    }

    let userHasAllowedRole = false;
    const userRoles = currentUserAuth.roles;
    let i = 0;
    while (i < userRoles.length && userHasAllowedRole === false) {
        if (allowedRoles.includes(userRoles[i]))
            userHasAllowedRole = true;
        ++i;
    }

    if (userHasAllowedRole === false) {
        return <div>Permission denied.</div>
    }

    return children;
};