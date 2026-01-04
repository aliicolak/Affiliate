import api, { setTokens, clearTokens, getAccessToken } from './client';
import type {
    AuthResponse,
    LoginRequest,
    RegisterRequest,
    MeResponse
} from '../types';

export const authApi = {
    login: async (data: LoginRequest): Promise<AuthResponse> => {
        const response = await api.post<AuthResponse>('/account/login', data);
        setTokens(response.data.accessToken, response.data.refreshToken);
        return response.data;
    },

    register: async (data: RegisterRequest): Promise<AuthResponse> => {
        const response = await api.post<AuthResponse>('/account/register', data);
        setTokens(response.data.accessToken, response.data.refreshToken);
        return response.data;
    },

    logout: async (): Promise<void> => {
        try {
            await api.post('/account/logout');
        } finally {
            clearTokens();
        }
    },

    me: async (): Promise<MeResponse> => {
        const response = await api.get<MeResponse>('/account/me');
        return response.data;
    },

    isAuthenticated: (): boolean => {
        return !!getAccessToken();
    },
};

export default authApi;
