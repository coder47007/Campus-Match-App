// Likes API service - integration with backend for "who liked you" feature

import { api } from './api';

export interface LikePreviewDto {
    id: number;
    name?: string;
    initial?: string;
    age?: number;
    major?: string;
    university?: string;
    photoUrl?: string;
    photo?: string;
    isSuperLike: boolean;
    likedAt?: string;
    createdAt?: string;
}

export interface ReceivedLikesResponse {
    canSeeWhoLikedYou: boolean;
    upgradeRequired?: boolean;
    likesCount?: number;
    likes: LikePreviewDto[];
}

export interface LikesCountResponse {
    total: number;
    superLikes: number;
    hasNew: boolean;
    canSeeWhoLikedYou: boolean;
}

export const likesApi = {
    // Get who liked you - returns full profiles for premium, blurred for free
    getReceivedLikes: async (): Promise<ReceivedLikesResponse> => {
        const response = await api.get<ReceivedLikesResponse>('/api/likes/received');
        return response.data;
    },

    // Get count of likes received
    getLikesCount: async (): Promise<LikesCountResponse> => {
        const response = await api.get<LikesCountResponse>('/api/likes/count');
        return response.data;
    },
};

export default likesApi;
