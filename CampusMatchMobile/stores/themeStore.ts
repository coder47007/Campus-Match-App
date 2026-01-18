// Theme store with Zustand - manages dark mode preference

import { create } from 'zustand';
import { Appearance, ColorSchemeName } from 'react-native';
import { setStorageItem, getStorageItem } from '../utils/storage';

const THEME_STORAGE_KEY = 'theme_preference';

export type ThemeMode = 'light' | 'dark' | 'system';

interface ThemeState {
    // State
    themeMode: ThemeMode;
    effectiveTheme: ColorSchemeName;
    isLoading: boolean;

    // Actions
    setThemeMode: (mode: ThemeMode) => Promise<void>;
    loadStoredTheme: () => Promise<void>;
    getEffectiveTheme: () => ColorSchemeName;
}

export const useThemeStore = create<ThemeState>((set, get) => ({
    // Initial state
    themeMode: 'system',
    effectiveTheme: Appearance.getColorScheme() || 'dark',
    isLoading: true,

    // Set theme mode
    setThemeMode: async (mode: ThemeMode) => {
        await setStorageItem(THEME_STORAGE_KEY, mode);

        const effectiveTheme = mode === 'system'
            ? (Appearance.getColorScheme() || 'dark')
            : mode;

        set({ themeMode: mode, effectiveTheme });
    },

    // Load stored theme preference
    loadStoredTheme: async () => {
        try {
            const stored = await getStorageItem(THEME_STORAGE_KEY);
            const themeMode = (stored as ThemeMode) || 'system';

            const effectiveTheme = themeMode === 'system'
                ? (Appearance.getColorScheme() || 'dark')
                : themeMode;

            set({ themeMode, effectiveTheme, isLoading: false });
        } catch {
            set({ isLoading: false });
        }
    },

    // Get effective theme
    getEffectiveTheme: () => {
        const { themeMode } = get();
        if (themeMode === 'system') {
            return Appearance.getColorScheme() || 'dark';
        }
        return themeMode;
    },
}));

// Listen to system theme changes
Appearance.addChangeListener(({ colorScheme }) => {
    const { themeMode } = useThemeStore.getState();
    if (themeMode === 'system') {
        useThemeStore.setState({ effectiveTheme: colorScheme || 'dark' });
    }
});

export default useThemeStore;
