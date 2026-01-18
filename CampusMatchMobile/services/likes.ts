// Likes API service

import { apiService } from './api';
import { LikePreviewDto, LikesCountResponse } from '../types';

export const likesApi = {
    // Get blurred preview of who liked you
    getReceivedLikes: async (): Promise<LikePreviewDto[]> => {
        return apiService.get<LikePreviewDto[]>('/api/likes/received');
    },

    // Get count of likes received
    getLikesCount: async (): Promise<LikesCountResponse> => {
        return apiService.get<LikesCountResponse>('/api/likes/count');
    },
};

export default likesApi;
