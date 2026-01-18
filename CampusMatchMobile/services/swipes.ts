// Swipes API service

import { apiService } from './api';
import {
    SwipeRequest,
    SwipeResponse,
    UndoSwipeResponse,
    RewindsRemainingResponse,
} from '../types';

export const swipesApi = {
    // Create a swipe (like/dislike/super like)
    swipe: async (data: SwipeRequest): Promise<SwipeResponse> => {
        return apiService.post<SwipeResponse>('/api/swipes', data);
    },

    // Undo last swipe (rewind)
    undoSwipe: async (): Promise<UndoSwipeResponse> => {
        return apiService.delete<UndoSwipeResponse>('/api/swipes/last');
    },

    // Get remaining rewinds count
    getRewindsRemaining: async (): Promise<RewindsRemainingResponse> => {
        return apiService.get<RewindsRemainingResponse>('/api/swipes/rewinds');
    },
};

export default swipesApi;
