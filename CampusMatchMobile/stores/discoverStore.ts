// Discover store with Zustand - manages swipeable profiles

import { create } from 'zustand';
import { StudentDto, SwipeResponse, MatchDto } from '../types';
import { profilesApi, swipesApi } from '../services';

interface DiscoverState {
    // State
    profiles: StudentDto[];
    currentIndex: number;
    isLoading: boolean;
    error: string | null;
    rewindsRemaining: number;
    rewindsResetAt: string | null;
    lastSwipedProfile: StudentDto | null;
    newMatch: MatchDto | null;
    filters: {
        minAge?: number;
        maxAge?: number;
        gender?: string;
        academicYears?: string[];
        majors?: string[];
    } | null;

    // Actions
    fetchProfiles: (filters?: {
        minAge?: number;
        maxAge?: number;
        gender?: string;
        academicYears?: string[];
        majors?: string[];
    }) => Promise<void>;
    swipe: (swipedId: number, isLike: boolean, isSuperLike?: boolean) => Promise<SwipeResponse>;
    undoSwipe: () => Promise<boolean>;
    nextProfile: () => void;
    fetchRewindsRemaining: () => Promise<void>;
    setNewMatch: (match: MatchDto | null) => void;
    setFilters: (filters: any) => void;
    clearError: () => void;
    reset: () => void;
}

export const useDiscoverStore = create<DiscoverState>((set, get) => ({
    // Initial state
    profiles: [],
    currentIndex: 0,
    isLoading: false,
    error: null,
    rewindsRemaining: 1,
    rewindsResetAt: null,
    lastSwipedProfile: null,
    newMatch: null,
    filters: null,

    // Fetch discover profiles
    fetchProfiles: async (filters) => {
        set({ isLoading: true, error: null });

        // Store filters if provided
        if (filters) {
            set({ filters });
        }

        try {
            // Use stored filters or passed filters
            const { filters: storedFilters } = get();
            const filtersToUse = filters || storedFilters;

            const profiles = await profilesApi.getDiscoverProfiles(filtersToUse || undefined);
            set({
                profiles,
                currentIndex: 0,
                isLoading: false,
            });
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to load profiles';
            set({ isLoading: false, error: errorMessage });
        }
    },

    // Swipe on a profile
    swipe: async (swipedId: number, isLike: boolean, isSuperLike: boolean = false) => {
        const { profiles, currentIndex } = get();
        const swipedProfile = profiles[currentIndex];

        try {
            const response = await swipesApi.swipe({
                swipedId,
                isLike,
                isSuperLike,
            });

            // Store for potential undo
            set({ lastSwipedProfile: swipedProfile });

            // Move to next profile
            set({ currentIndex: currentIndex + 1 });

            // If it's a match, show match popup
            if (response.isMatch && response.match) {
                set({ newMatch: response.match });
            }

            return response;
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to swipe';
            set({ error: errorMessage });
            throw error;
        }
    },

    // Undo last swipe
    undoSwipe: async () => {
        const { currentIndex, lastSwipedProfile, profiles, rewindsRemaining } = get();

        if (!lastSwipedProfile || currentIndex === 0 || rewindsRemaining <= 0) {
            return false;
        }

        try {
            const response = await swipesApi.undoSwipe();

            if (response.success) {
                // Go back one profile
                set({
                    currentIndex: currentIndex - 1,
                    lastSwipedProfile: null,
                    rewindsRemaining: rewindsRemaining - 1,
                });
                return true;
            }

            set({ error: response.message || 'Failed to undo' });
            return false;
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to undo swipe';
            set({ error: errorMessage });
            return false;
        }
    },

    // Move to next profile manually
    nextProfile: () => {
        const { currentIndex, profiles } = get();
        if (currentIndex < profiles.length - 1) {
            set({ currentIndex: currentIndex + 1 });
        }
    },

    // Fetch rewinds remaining
    fetchRewindsRemaining: async () => {
        try {
            const response = await swipesApi.getRewindsRemaining();
            set({
                rewindsRemaining: response.remaining,
                rewindsResetAt: response.resetsAt,
            });
        } catch (error) {
            // Ignore errors
        }
    },

    // Set new match for popup
    setNewMatch: (match: MatchDto | null) => set({ newMatch: match }),

    // Set filters
    setFilters: (filters: any) => set({ filters }),

    // Clear error
    clearError: () => set({ error: null }),

    // Reset store
    reset: () => set({
        profiles: [],
        currentIndex: 0,
        isLoading: false,
        error: null,
        lastSwipedProfile: null,
        newMatch: null,
    }),
}));

export default useDiscoverStore;
