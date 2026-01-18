// Axios API service with JWT authentication and retry logic

import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig } from 'axios';
import { setStorageItem, getStorageItem, deleteStorageItem } from '../utils/storage';
import { API_BASE_URL, API_TIMEOUT, MAX_RETRIES, RETRY_DELAY, STORAGE_KEYS } from '../constants/config';

// Create axios instance
const api: AxiosInstance = axios.create({
    baseURL: API_BASE_URL,
    timeout: API_TIMEOUT,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Token management
let authToken: string | null = null;
let refreshToken: string | null = null;
let isRefreshing = false;
let failedQueue: Array<{
    resolve: (value?: unknown) => void;
    reject: (reason?: unknown) => void;
}> = [];

const processQueue = (error: Error | null, token: string | null = null) => {
    failedQueue.forEach(prom => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token);
        }
    });
    failedQueue = [];
};

// Set auth token
export const setAuthToken = async (token: string | null, refresh: string | null = null) => {
    authToken = token;
    refreshToken = refresh;

    if (token) {
        await setStorageItem(STORAGE_KEYS.AUTH_TOKEN, token);
        if (refresh) {
            await setStorageItem(STORAGE_KEYS.REFRESH_TOKEN, refresh);
        }
    } else {
        await deleteStorageItem(STORAGE_KEYS.AUTH_TOKEN);
        await deleteStorageItem(STORAGE_KEYS.REFRESH_TOKEN);
    }
};

// Load stored token
export const loadStoredToken = async (): Promise<string | null> => {
    try {
        authToken = await getStorageItem(STORAGE_KEYS.AUTH_TOKEN);
        refreshToken = await getStorageItem(STORAGE_KEYS.REFRESH_TOKEN);
        return authToken;
    } catch (error) {
        console.error('Error loading stored token:', error);
        return null;
    }
};

// Get current token
export const getAuthToken = () => authToken;

// Request interceptor - add auth token
api.interceptors.request.use(
    async (config: InternalAxiosRequestConfig) => {
        if (authToken) {
            config.headers.Authorization = `Bearer ${authToken}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response interceptor - handle token refresh
api.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
        const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

        // If 401 and we have a refresh token, try to refresh
        if (error.response?.status === 401 && refreshToken && !originalRequest._retry) {
            if (isRefreshing) {
                // Wait for the refresh to complete
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                }).then(token => {
                    if (originalRequest.headers) {
                        originalRequest.headers.Authorization = `Bearer ${token}`;
                    }
                    return api(originalRequest);
                });
            }

            originalRequest._retry = true;
            isRefreshing = true;

            try {
                const response = await axios.post(`${API_BASE_URL}/api/auth/refresh`, {
                    refreshToken: refreshToken,
                });

                const { token: newToken, refreshToken: newRefresh } = response.data;
                await setAuthToken(newToken, newRefresh);

                processQueue(null, newToken);

                if (originalRequest.headers) {
                    originalRequest.headers.Authorization = `Bearer ${newToken}`;
                }

                return api(originalRequest);
            } catch (refreshError) {
                processQueue(refreshError as Error, null);
                await setAuthToken(null);
                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
            }
        }

        return Promise.reject(error);
    }
);

// Retry logic helper
const retryRequest = async <T>(
    requestFn: () => Promise<T>,
    retries: number = MAX_RETRIES
): Promise<T> => {
    let lastError: Error | null = null;

    for (let i = 0; i < retries; i++) {
        try {
            return await requestFn();
        } catch (error) {
            lastError = error as Error;
            if (i < retries - 1) {
                await new Promise(resolve => setTimeout(resolve, RETRY_DELAY * (i + 1)));
            }
        }
    }

    throw lastError;
};

// API methods with types
export const apiService = {
    get: <T>(url: string, config?: object) =>
        api.get<T>(url, config).then(res => res.data),

    post: <T>(url: string, data?: object, config?: object) =>
        api.post<T>(url, data, config).then(res => res.data),

    put: <T>(url: string, data?: object, config?: object) =>
        api.put<T>(url, data, config).then(res => res.data),

    patch: <T>(url: string, data?: object, config?: object) =>
        api.patch<T>(url, data, config).then(res => res.data),

    delete: <T>(url: string, config?: object) =>
        api.delete<T>(url, config).then(res => res.data),

    // For file uploads
    upload: <T>(url: string, formData: FormData) =>
        api.post<T>(url, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        }).then(res => res.data),
};

export { api, retryRequest };
export default api;
