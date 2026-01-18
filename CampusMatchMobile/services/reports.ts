// Reports API service

import { apiService } from './api';
import { ReportRequest, BlockRequest, BlockedUserDto } from '../types';

export const reportsApi = {
    // Report a user
    reportUser: async (data: ReportRequest): Promise<{ message: string }> => {
        return apiService.post('/api/reports', data);
    },

    // Block a user
    blockUser: async (blockedId: number): Promise<{ message: string }> => {
        return apiService.post('/api/reports/block', { blockedId } as BlockRequest);
    },

    // Unblock a user
    unblockUser: async (blockedId: number): Promise<{ message: string }> => {
        return apiService.delete(`/api/reports/block/${blockedId}`);
    },

    // Get blocked users
    getBlockedUsers: async (): Promise<BlockedUserDto[]> => {
        return apiService.get<BlockedUserDto[]>('/api/reports/blocked');
    },
};

export default reportsApi;
