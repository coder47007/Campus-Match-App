// Authentication store with Zustand

import { create } from 'zustand';
import { StudentDto, RegisterRequest } from '../types';
import { authApi, profilesApi, loadStoredToken, setAuthToken } from '../services';
import { signalRService } from '../services/signalr';
import { STORAGE_KEYS } from '../constants/config';
import { setStorageItem, getStorageItem, deleteStorageItem } from '../utils/storage';

interface AuthState {
    // State
    token: string | null;
    user: StudentDto | null;
    isLoading: boolean;
    isInitialized: boolean;
    error: string | null;

    // Actions
    login: (email: string, password: string) => Promise<void>;
    register: (data: RegisterRequest) => Promise<void>;
    logout: () => Promise<void>;
    loadStoredAuth: () => Promise<boolean>;
    updateUser: (user: StudentDto) => void;
    clearError: () => void;
}

export const useAuthStore = create<AuthState>((set, get) => ({
    // Initial state
    token: null,
    user: null,
    isLoading: false,
    isInitialized: false,
    error: null,

    // Login
    login: async (email: string, password: string) => {
        set({ isLoading: true, error: null });
        try {
            const response = await authApi.login({ email, password });

            // Store user data
            await setStorageItem(STORAGE_KEYS.USER_DATA, JSON.stringify(response.student));

            set({
                token: response.token,
                user: response.student,
                isLoading: false,
            });

            // Temporarily disabled SignalR - using Supabase for real-time
            // signalRService.start();
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Login failed. Please try again.';
            set({ isLoading: false, error: errorMessage });
            throw error;
        }
    },

    // Register
    register: async (data: RegisterRequest) => {
        set({ isLoading: true, error: null });
        try {
            const response = await authApi.register(data);

            // Store user data
            await setStorageItem(STORAGE_KEYS.USER_DATA, JSON.stringify(response.student));

            set({
                token: response.token,
                user: response.student,
                isLoading: false,
            });

            // Temporarily disabled SignalR - using Supabase for real-time
            // signalRService.start();
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : (error as { response?: { data?: { message?: string } } })?.response?.data?.message || 'Registration failed. Please try again.';
            set({ isLoading: false, error: errorMessage });
            throw error;
        }
    },

    // Logout
    logout: async () => {
        set({ isLoading: true });
        try {
            await authApi.logout();
        } catch (error) {
            // Ignore errors
        } finally {
            // Stop SignalR
            signalRService.stop();

            // Clear stored data
            await deleteStorageItem(STORAGE_KEYS.USER_DATA);

            set({
                token: null,
                user: null,
                isLoading: false,
            });
        }
    },

    // Load stored authentication
    loadStoredAuth: async () => {
        try {
            const token = await loadStoredToken();

            if (token) {
                // Try to get stored user data
                const userData = await getStorageItem(STORAGE_KEYS.USER_DATA);
                let user: StudentDto | null = null;

                if (userData) {
                    user = JSON.parse(userData);
                }

                // Try to fetch fresh user data
                try {
                    const freshUser = await profilesApi.getMyProfile();
                    user = freshUser;
                    await setStorageItem(STORAGE_KEYS.USER_DATA, JSON.stringify(freshUser));
                } catch (error) {
                    // If token is invalid, clear auth
                    if ((error as { response?: { status?: number } })?.response?.status === 401) {
                        await setAuthToken(null);
                        await deleteStorageItem(STORAGE_KEYS.USER_DATA);
                        set({ token: null, user: null, isInitialized: true });
                        return false;
                    }
                }

                set({ token, user, isInitialized: true });

                // Temporarily disabled SignalR
                // signalRService.start();

                return true;
            }

            set({ isInitialized: true });
            return false;
        } catch (error) {
            set({ isInitialized: true });
            return false;
        }
    },

    // Update user
    updateUser: (user: StudentDto) => {
        set({ user });
        setStorageItem(STORAGE_KEYS.USER_DATA, JSON.stringify(user));
    },

    // Clear error
    clearError: () => set({ error: null }),
}));

export default useAuthStore;
