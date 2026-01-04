import axios, { type AxiosError, type AxiosInstance, type InternalAxiosRequestConfig } from 'axios';
import type { ApiError } from '../types';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

// Create axios instance
const api: AxiosInstance = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true,
});

// Token storage
const TOKEN_KEY = 'affiliate_access_token';
const REFRESH_TOKEN_KEY = 'affiliate_refresh_token';

export const getAccessToken = (): string | null => localStorage.getItem(TOKEN_KEY);
export const getRefreshToken = (): string | null => localStorage.getItem(REFRESH_TOKEN_KEY);

export const setTokens = (accessToken: string, refreshToken: string): void => {
    localStorage.setItem(TOKEN_KEY, accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
};

export const clearTokens = (): void => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
};

// Request interceptor - add auth header
api.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
        const token = getAccessToken();
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response interceptor - handle 401 and refresh token
api.interceptors.response.use(
    (response) => response,
    async (error: AxiosError<ApiError>) => {
        const originalRequest = error.config;

        // If 401 and we have a refresh token, try to refresh
        if (error.response?.status === 401 && originalRequest && !originalRequest.headers['X-Retry']) {
            const refreshToken = getRefreshToken();
            const accessToken = getAccessToken();

            if (refreshToken && accessToken) {
                try {
                    const response = await axios.post(`${API_BASE_URL}/account/refresh`,
                        { refreshToken },
                        { headers: { Authorization: `Bearer ${accessToken}` } }
                    );

                    const { accessToken: newAccessToken, refreshToken: newRefreshToken } = response.data;
                    setTokens(newAccessToken, newRefreshToken);

                    // Retry original request with new token
                    originalRequest.headers['X-Retry'] = 'true';
                    originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
                    return api(originalRequest);
                } catch (refreshError) {
                    clearTokens();
                    window.location.href = '/login';
                }
            } else {
                clearTokens();
                window.location.href = '/login';
            }
        }

        return Promise.reject(error);
    }
);

export default api;
