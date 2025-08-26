import { createContext } from "react";
import { AuthDto } from "../dtos/AuthDto";

interface IAuthContext {
    token?: string | null;
    currentUserAuth?: AuthDto | null;
    handleLogin: (email: string, password: string) => void;
    handleLogout: () => void;
}

export const AuthContext = createContext<IAuthContext | undefined>({} as IAuthContext);