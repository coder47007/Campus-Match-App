// Subscription Service - Mobile integration with Premium features
import { api } from './api';

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
}

export interface SubscriptionDto {
    plan: string;
    planName: string;
    isActive: boolean;
    endDate: string | null;
    superLikesRemaining: number;
    rewindsRemaining: number;
    boostsRemaining: number;
    swipesRemaining?: number;
    swipesResetAt?: string | null;
    boostsResetAt?: string | null;
    features: PlanFeatures;
}

export interface UpgradeRequest {
    plan: 'premium' | 'gold';
    paymentToken?: string;
}

export interface SwipeStatusResponse {
    swipesRemaining: number;
    isUnlimited: boolean;
    resetsAt: string | null;
    canSwipe: boolean;
}

class SubscriptionsService {
    private apiUrl = '/api/subscriptions';

    /**
     * Get current user's subscription status
     */
    async getSubscription(): Promise<SubscriptionDto> {
        const response = await api.get<SubscriptionDto>(this.apiUrl);
        return response.data;
    }

    /**
     * Get swipe status (for free user limit tracking)
     */
    async getSwipeStatus(): Promise<SwipeStatusResponse> {
        const response = await api.get<SwipeStatusResponse>(`${this.apiUrl}/swipes`);
        return response.data;
    }

    /**
     * Use a swipe (call before each swipe for free users)
     */
    async useSwipe(): Promise<SwipeStatusResponse> {
        const response = await api.post<SwipeStatusResponse>(`${this.apiUrl}/use-swipe`);
        return response.data;
    }

    /**
     * Get all available subscription plans
     */
    async getPlans(): Promise<Record<string, PlanFeatures>> {
        const response = await api.get<Record<string, PlanFeatures>>(`${this.apiUrl}/plans`);
        return response.data;
    }

    /**
     * Upgrade subscription to a paid plan
     */
    async upgrade(request: UpgradeRequest): Promise<SubscriptionDto> {
        const response = await api.post<SubscriptionDto>(`${this.apiUrl}/upgrade`, request);
        return response.data;
    }

    /**
     * Cancel subscription (will remain active until end date)
     */
    async cancel(): Promise<{ message: string; subscription: SubscriptionDto }> {
        const response = await api.post<{ message: string; subscription: SubscriptionDto }>(`${this.apiUrl}/cancel`);
        return response.data;
    }

    /**
     * Use a super like
     * @returns Remaining super likes and whether unlimited
     */
    async useSuperLike(): Promise<{ superLikesRemaining: number; isUnlimited: boolean }> {
        const response = await api.post<{ superLikesRemaining: number; isUnlimited: boolean }>(`${this.apiUrl}/use-superlike`);
        return response.data;
    }

    /**
     * Use a rewind (undo last swipe)
     * @returns Remaining rewinds and whether unlimited
     */
    async useRewind(): Promise<{ rewindsRemaining: number; isUnlimited: boolean }> {
        const response = await api.post<{ rewindsRemaining: number; isUnlimited: boolean }>(`${this.apiUrl}/use-rewind`);
        return response.data;
    }

    /**
     * Use a profile boost (30 minutes of visibility)
     */
    async useBoost(): Promise<{ message: string; boostsRemaining: number; expiresAt: string; isUnlimited?: boolean }> {
        const response = await api.post<{ message: string; boostsRemaining: number; expiresAt: string; isUnlimited?: boolean }>(`${this.apiUrl}/use-boost`);
        return response.data;
    }

    /**
     * Purchase additional boosts
     */
    async purchaseBoost(quantity: number): Promise<{ message: string; boostsRemaining: number }> {
        const response = await api.post<{ message: string; boostsRemaining: number }>(`${this.apiUrl}/purchase-boost`, { quantity });
        return response.data;
    }

    /**
     * Check if user has premium features
     */
    async isPremium(): Promise<boolean> {
        try {
            const subscription = await this.getSubscription();
            return subscription.plan !== 'free' && subscription.isActive;
        } catch {
            return false;
        }
    }
}

export const subscriptionsApi = new SubscriptionsService();
