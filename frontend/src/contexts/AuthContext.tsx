import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { authApi } from '../api';
import type { MeResponse } from '../types';

interface AuthContextType {
    user: MeResponse | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    login: (userNameOrEmail: string, password: string) => Promise<void>;
    register: (email: string, password: string, displayName: string, userName?: string) => Promise<void>;
    logout: () => Promise<void>;
    refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};

interface AuthProviderProps {
    children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
    const [user, setUser] = useState<MeResponse | null>(null);
    const [isLoading, setIsLoading] = useState(true);

    const refreshUser = async () => {
        try {
            if (authApi.isAuthenticated()) {
                const userData = await authApi.me();
                setUser(userData);
            } else {
                setUser(null);
            }
        } catch {
            setUser(null);
        }
    };

    useEffect(() => {
        const initAuth = async () => {
            await refreshUser();
            setIsLoading(false);
        };
        initAuth();
    }, []);

    const login = async (userNameOrEmail: string, password: string) => {
        await authApi.login({ userNameOrEmail, password });
        await refreshUser();
    };

    const register = async (email: string, password: string, displayName: string, userName?: string) => {
        await authApi.register({ email, password, displayName, userName });
        await refreshUser();
    };

    const logout = async () => {
        await authApi.logout();
        setUser(null);
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                isAuthenticated: !!user,
                isLoading,
                login,
                register,
                logout,
                refreshUser,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};

export default AuthContext;
