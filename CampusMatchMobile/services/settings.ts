// Settings API service

import { apiService } from './api';
import { SettingsDto, UpdateSettingsRequest } from '../types';

export const settingsApi = {
    // Get current settings
    getSettings: async (): Promise<SettingsDto> => {
        return apiService.get<SettingsDto>('/api/settings');
    },

    // Update settings
    updateSettings: async (data: UpdateSettingsRequest): Promise<SettingsDto> => {
        return apiService.put<SettingsDto>('/api/settings', data);
    },
};

export default settingsApi;
