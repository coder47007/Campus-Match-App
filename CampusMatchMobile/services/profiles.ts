// Profiles API service

import { apiService } from './api';
import {
    StudentDto,
    ProfileUpdateRequest,
    InterestDto,
    UpdateInterestsRequest,
} from '../types';

export const profilesApi = {
    // Get current user's profile
    getMyProfile: async (): Promise<StudentDto> => {
        return apiService.get<StudentDto>('/api/profiles/me');
    },

    // Update current user's profile
    updateMyProfile: async (data: ProfileUpdateRequest): Promise<StudentDto> => {
        return apiService.put<StudentDto>('/api/profiles/me', data);
    },

    // Get discover profiles (for swiping)
    getDiscoverProfiles: async (filters?: {
        minAge?: number;
        maxAge?: number;
        gender?: string;
        academicYears?: string[];
        majors?: string[];
    }): Promise<StudentDto[]> => {
        const params = new URLSearchParams();

        if (filters) {
            if (filters.minAge) params.append('minAge', filters.minAge.toString());
            if (filters.maxAge) params.append('maxAge', filters.maxAge.toString());
            if (filters.gender) params.append('gender', filters.gender);
            if (filters.academicYears && filters.academicYears.length > 0) {
                params.append('academicYears', filters.academicYears.join(','));
            }
            if (filters.majors && filters.majors.length > 0) {
                params.append('majors', filters.majors.join(','));
            }
        }

        const queryString = params.toString();
        const url = queryString ? `/api/profiles/discover?${queryString}` : '/api/profiles/discover';

        return apiService.get<StudentDto[]>(url);
    },

    // Get all available interests
    getInterests: async (): Promise<InterestDto[]> => {
        return apiService.get<InterestDto[]>('/api/profiles/interests');
    },

    // Update user's interests
    updateInterests: async (interestIds: number[]): Promise<StudentDto> => {
        return apiService.put<StudentDto>('/api/profiles/interests', { interestIds } as UpdateInterestsRequest);
    },

    // Get a specific user's profile
    getProfile: async (userId: number): Promise<StudentDto> => {
        return apiService.get<StudentDto>(`/api/profiles/${userId}`);
    },
};

export default profilesApi;
