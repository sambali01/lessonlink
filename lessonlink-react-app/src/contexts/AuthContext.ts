import { createContext } from "react";
import { UserAuth } from "../models/UserAuth";

interface IAuthContext {
    token?: string | null;
    currentUserAuth?: UserAuth | null;
    handleLogin: (email: string, password: string) => void;
    handleLogout: () => void;
}

export const AuthContext = createContext<IAuthContext | undefined>(undefined);