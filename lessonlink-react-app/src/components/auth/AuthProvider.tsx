import { PropsWithChildren, useEffect, useLayoutEffect, useState } from "react";
import { axiosInstance } from "../../configs/axiosConfig";
import { AuthContext } from "../../contexts/AuthContext";
import { AuthDto } from "../../dtos/AuthDto";
import { login, logout, refresh } from "../../services/auth.service";

export default function AuthProvider({ children }: PropsWithChildren) {
    const [token, setToken] = useState<string | null>();
    const [currentUserAuth, setCurrentUserAuth] = useState<AuthDto | null>();

    useEffect(() => {
        (async function () {
            try {
                const data = await refresh();
                if (data) {
                    const tokenStr = data.token;
                    const currentUserAuthObj = {
                        userId: data.id,
                        roles: data.roles
                    }

                    setToken(tokenStr);
                    setCurrentUserAuth(currentUserAuthObj);
                } else {
                    throw new Error("No data provided.");
                }
            } catch {
                setToken(null);
                setCurrentUserAuth(null);
            }
        })();
    }, [])

    useLayoutEffect(() => {
        const authInterceptor = axiosInstance.interceptors.request.use((config) => {
            if (!(config as { _retry?: boolean })._retry && token) {
                config.headers.Authorization = `Bearer ${token}`;
            }
            return config;
        });

        return () => {
            axiosInstance.interceptors.request.eject(authInterceptor);
        };
    }, [token])

    useLayoutEffect(() => {
        const refreshInterceptor = axiosInstance.interceptors.response.use(
            response => response,
            async error => {
                const originalRequest = error.config;

                if (error.response.status === 401 && error.response.data.message === 'Unauthorized') {
                    try {
                        const data = await refresh();
                        if (data?.token) {
                            setToken(data.token);

                            originalRequest.headers.Authorization = `Bearer ${data.token}`;
                            originalRequest._retry = true;

                            return axiosInstance(originalRequest);
                        } else {
                            throw new Error("No token provided.");
                        }
                    } catch {
                        setToken(null);
                        setCurrentUserAuth(null);
                        return Promise.reject(error);
                    }
                }

                return Promise.reject(error);
            }
        );

        return () => {
            axiosInstance.interceptors.response.eject(refreshInterceptor);
        };
    }, [])

    async function handleLogin(email: string, password: string) {
        try {
            await login(email, password).then((data) => {
                if (data) {
                    const tokenStr = data.token;
                    const currentUserAuthObj = {
                        userId: data.id,
                        roles: data.roles
                    }

                    setToken(tokenStr);
                    setCurrentUserAuth(currentUserAuthObj);
                } else {
                    setToken(null);
                    setCurrentUserAuth(null);
                }
            })
        } catch {
            setToken(null);
            setCurrentUserAuth(null);
        };
    }

    async function handleLogout() {
        try {
            await logout();
            setToken(null);
            setCurrentUserAuth(null);
        } catch {
            setToken(null);
            setCurrentUserAuth(null);
        }
    }

    return <AuthContext.Provider value={
        {
            token,
            currentUserAuth,
            handleLogin,
            handleLogout
        }
    }>
        {children}
    </AuthContext.Provider>
}