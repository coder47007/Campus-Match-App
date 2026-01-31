// Subscription Store with Zustand - manages subscription state globally
import { create } from 'zustand';
import { subscriptionsApi } from '../services/subscriptions';

export interface PlanFeatures {
    name: string;
    price: number;
    superLikesPerDay: number;
    rewindsPerDay: number;
    boostsPerPeriod: number;
    boostPeriodDays: number;
    swipesPerPeriod: number;
    swipePeriodHours: number;
    maxDistanceKm: number;
    crossCampusMatching: boolean;
    canSeeWhoLikedYou: boolean;
    unlimitedSwipes: boolean;
    advancedFilters: boolean;
    readReceipts: boolean;
    typingIndicators: boolean;
    highlightedBadge: boolean;
    noAds: boolean;
    priorityMatching: boolean;
    profileBoost: boolean;
}

export interface SubscriptionState {
    plan: 'free' | 'premium' | 'gold';
    planName: string;
    isActive: boolean;
    endDate: string | null;

    // Remaining counts
    superLikesRemaining: number;
    rewindsRemaining: number;
    boostsRemaining: number;
    swipesRemaining: number;

    // Reset times
    swipesResetAt: string | null;
    boostsResetAt: string | null;

    // Boost status
    isBoosted: boolean;
    boostExpiresAt: string | null;

    // Features
    features: PlanFeatures | null;

    // Loading state
    isLoading: boolean;
    error: string | null;
}

interface SubscriptionStore extends SubscriptionState {
    // Computed
    isPremium: boolean;
    canSwipe: boolean;
    canRewind: boolean;
    canSeeWhoLikedYou: boolean;
    hasAdvancedFilters: boolean;
    hasReadReceipts: boolean;
    hasTypingIndicators: boolean;

    // Actions
    fetchSubscription: () => Promise<void>;
    useSwipe: () => Promise<boolean>;
    useRewind: () => Promise<boolean>;
    useSuperLike: () => Promise<boolean>;
    useBoost: () => Promise<{ success: boolean; expiresAt?: string }>;
    upgrade: (plan: 'premium' | 'gold') => Promise<boolean>;
    clearError: () => void;
    reset: () => void;
}

const initialState: SubscriptionState = {
    plan: 'free',
    planName: 'Free',
    isActive: false,
    endDate: null,
    superLikesRemaining: 1,
    rewindsRemaining: 0,
    boostsRemaining: 0,
    swipesRemaining: 20,
    swipesResetAt: null,
    boostsResetAt: null,
    isBoosted: false,
    boostExpiresAt: null,
    features: null,
    isLoading: false,
    error: null,
};

export const useSubscriptionStore = create<SubscriptionStore>((set, get) => ({
    ...initialState,

    // Computed properties
    get isPremium() {
        const { plan } = get();
        return plan === 'premium' || plan === 'gold';
    },

    get canSwipe() {
        const { features, swipesRemaining } = get();
        return features?.unlimitedSwipes || swipesRemaining > 0;
    },

    get canRewind() {
        const { features, rewindsRemaining } = get();
        if (!features) return false;
        return features.rewindsPerDay !== 0 && (features.rewindsPerDay === -1 || rewindsRemaining > 0);
    },

    get canSeeWhoLikedYou() {
        return get().features?.canSeeWhoLikedYou ?? false;
    },

    get hasAdvancedFilters() {
        return get().features?.advancedFilters ?? false;
    },

    get hasReadReceipts() {
        return get().features?.readReceipts ?? false;
    },

    get hasTypingIndicators() {
        return get().features?.typingIndicators ?? false;
    },

    // Actions
    fetchSubscription: async () => {
        set({ isLoading: true, error: null });
        try {
            const data = await subscriptionsApi.getSubscription();
            set({
                plan: data.plan as 'free' | 'premium' | 'gold',
                planName: data.planName,
                isActive: data.isActive,
                endDate: data.endDate,
                superLikesRemaining: data.superLikesRemaining,
                rewindsRemaining: data.rewindsRemaining,
                boostsRemaining: data.boostsRemaining,
                swipesRemaining: data.swipesRemaining ?? 20,
                swipesResetAt: data.swipesResetAt ?? null,
                boostsResetAt: data.boostsResetAt ?? null,
                features: data.features as PlanFeatures,
                isLoading: false,
            });
        } catch (error) {
            set({
                isLoading: false,
                error: error instanceof Error ? error.message : 'Failed to load subscription',
            });
        }
    },

    useSwipe: async () => {
        const { features, swipesRemaining } = get();

        // If unlimited, no need to track
        if (features?.unlimitedSwipes) {
            return true;
        }

        // Check if can swipe
        if (swipesRemaining <= 0) {
            set({ error: 'No swipes remaining. Upgrade to Premium for unlimited swipes!' });
            return false;
        }

        // Optimistically decrement
        set({ swipesRemaining: swipesRemaining - 1 });

        try {
            // Sync with server (optional - for accuracy)
            // const response = await subscriptionsApi.useSwipe();
            // set({ swipesRemaining: response.swipesRemaining });
            return true;
        } catch (error) {
            // Revert on error
            set({ swipesRemaining: swipesRemaining });
            return false;
        }
    },

    useRewind: async () => {
        const { features, rewindsRemaining } = get();

        if (!features || features.rewindsPerDay === 0) {
            set({ error: 'Rewinds are not available on the free plan. Upgrade to Premium!' });
            return false;
        }

        if (features.rewindsPerDay !== -1 && rewindsRemaining <= 0) {
            set({ error: 'No rewinds remaining today.' });
            return false;
        }

        try {
            const response = await subscriptionsApi.useRewind();
            set({ rewindsRemaining: response.rewindsRemaining });
            return true;
        } catch (error) {
            set({ error: error instanceof Error ? error.message : 'Failed to use rewind' });
            return false;
        }
    },

    useSuperLike: async () => {
        const { features, superLikesRemaining } = get();

        if (features?.superLikesPerDay !== -1 && superLikesRemaining <= 0) {
            set({ error: 'No super likes remaining today.' });
            return false;
        }

        try {
            const response = await subscriptionsApi.useSuperLike();
            set({ superLikesRemaining: response.superLikesRemaining });
            return true;
        } catch (error) {
            set({ error: error instanceof Error ? error.message : 'Failed to use super like' });
            return false;
        }
    },

    useBoost: async () => {
        const { boostsRemaining, features } = get();

        if (!features?.profileBoost) {
            set({ error: 'Profile boost is a premium feature. Upgrade to Premium!' });
            return { success: false };
        }

        if (boostsRemaining <= 0) {
            set({ error: 'No boosts remaining.' });
            return { success: false };
        }

        try {
            const response = await subscriptionsApi.useBoost();
            set({
                boostsRemaining: response.boostsRemaining,
                isBoosted: true,
                boostExpiresAt: response.expiresAt,
            });
            return { success: true, expiresAt: response.expiresAt };
        } catch (error) {
            set({ error: error instanceof Error ? error.message : 'Failed to use boost' });
            return { success: false };
        }
    },

    upgrade: async (plan: 'premium' | 'gold') => {
        set({ isLoading: true, error: null });
        try {
            const data = await subscriptionsApi.upgrade({ plan });
            set({
                plan: data.plan as 'free' | 'premium' | 'gold',
                planName: data.planName,
                isActive: data.isActive,
                endDate: data.endDate,
                superLikesRemaining: data.superLikesRemaining,
                rewindsRemaining: data.rewindsRemaining,
                boostsRemaining: data.boostsRemaining,
                swipesRemaining: data.swipesRemaining ?? 999,
                features: data.features as PlanFeatures,
                isLoading: false,
            });
            return true;
        } catch (error) {
            set({
                isLoading: false,
                error: error instanceof Error ? error.message : 'Failed to upgrade',
            });
            return false;
        }
    },

    clearError: () => set({ error: null }),

    reset: () => set(initialState),
}));

export default useSubscriptionStore;
