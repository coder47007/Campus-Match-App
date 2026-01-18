// Authentication API service

import { apiService, setAuthToken } from './api';
import {
    AuthResponse,
    LoginRequest,
    RegisterRequest,
    ChangePasswordRequest,
    DeleteAccountRequest,
    ForgotPasswordRequest,
    ResetPasswordRequest,
} from '../types';

export const authApi = {
    // Login
    login: async (data: LoginRequest): Promise<AuthResponse> => {
        const response = await apiService.post<AuthResponse>('/api/auth/login', data);
        await setAuthToken(response.token, response.refreshToken);
        return response;
    },

    // Register
    register: async (data: RegisterRequest): Promise<AuthResponse> => {
        const response = await apiService.post<AuthResponse>('/api/auth/register', data);
        await setAuthToken(response.token, response.refreshToken);
        return response;
    },

    // Logout
    logout: async (): Promise<void> => {
        try {
            await apiService.post('/api/auth/logout');
        } catch (error) {
            // Ignore errors on logout
        } finally {
            await setAuthToken(null);
        }
    },

    // Forgot password
    forgotPassword: async (email: string): Promise<{ message: string }> => {
        return apiService.post('/api/auth/forgot-password', { email } as ForgotPasswordRequest);
    },

    // Reset password
    resetPassword: async (data: ResetPasswordRequest): Promise<{ message: string }> => {
        return apiService.post('/api/auth/reset-password', data);
    },

    // Change password
    changePassword: async (data: ChangePasswordRequest): Promise<{ message: string }> => {
        return apiService.post('/api/auth/change-password', data);
    },

    // Delete account
    deleteAccount: async (data: DeleteAccountRequest): Promise<void> => {
        await apiService.delete('/api/auth/account', { data });
        await setAuthToken(null);
    },

    // Refresh token
    refreshToken: async (refreshToken: string): Promise<AuthResponse> => {
        const response = await apiService.post<AuthResponse>('/api/auth/refresh', { refreshToken });
        await setAuthToken(response.token, response.refreshToken);
        return response;
    },
};

export default authApi;
