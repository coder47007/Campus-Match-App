// Settings store with Zustand

import { create } from 'zustand';
import { SettingsDto, UpdateSettingsRequest } from '../types';
import { settingsApi } from '../services';

interface SettingsState {
    // State
    settings: SettingsDto | null;
    isLoading: boolean;
    isSaving: boolean;
    error: string | null;

    // Actions
    fetchSettings: () => Promise<void>;
    updateSettings: (data: UpdateSettingsRequest) => Promise<void>;
    clearError: () => void;
    reset: () => void;
}

export const useSettingsStore = create<SettingsState>((set) => ({
    // Initial state
    settings: null,
    isLoading: false,
    isSaving: false,
    error: null,

    // Fetch settings
    fetchSettings: async () => {
        set({ isLoading: true, error: null });
        try {
            const settings = await settingsApi.getSettings();
            set({ settings, isLoading: false });
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to load settings';
            set({ isLoading: false, error: errorMessage });
        }
    },

    // Update settings
    updateSettings: async (data: UpdateSettingsRequest) => {
        set({ isSaving: true, error: null });
        try {
            const settings = await settingsApi.updateSettings(data);
            set({ settings, isSaving: false });
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to save settings';
            set({ isSaving: false, error: errorMessage });
            throw error;
        }
    },

    // Clear error
    clearError: () => set({ error: null }),

    // Reset store
    reset: () => set({
        settings: null,
        isLoading: false,
        isSaving: false,
        error: null,
    }),
}));

export default useSettingsStore;
