// Matches API service

import { apiService } from './api';
import { MatchDto } from '../types';

export const matchesApi = {
    // Get all matches
    getMatches: async (): Promise<MatchDto[]> => {
        return apiService.get<MatchDto[]>('/api/matches');
    },

    // Unmatch (delete match)
    unmatch: async (matchId: number): Promise<void> => {
        return apiService.delete(`/api/matches/${matchId}`);
    },
};

export default matchesApi;
