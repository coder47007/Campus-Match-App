// Prompts API service

import { apiService } from './api';
import {
    PromptDto,
    StudentPromptDto,
    UpdatePromptRequest,
    UpdatePromptsRequest,
} from '../types';

export const promptsApi = {
    // Get all available prompts
    getAllPrompts: async (): Promise<PromptDto[]> => {
        return apiService.get<PromptDto[]>('/api/prompts');
    },

    // Get current user's prompt answers
    getMyPrompts: async (): Promise<StudentPromptDto[]> => {
        return apiService.get<StudentPromptDto[]>('/api/prompts/me');
    },

    // Add or update a prompt answer
    addOrUpdatePrompt: async (data: UpdatePromptRequest): Promise<StudentPromptDto> => {
        return apiService.post<StudentPromptDto>('/api/prompts', data);
    },

    // Update multiple prompts at once
    updatePrompts: async (prompts: UpdatePromptRequest[]): Promise<StudentPromptDto[]> => {
        return apiService.put<StudentPromptDto[]>('/api/prompts', { prompts } as UpdatePromptsRequest);
    },

    // Delete a prompt answer
    deletePrompt: async (promptId: number): Promise<void> => {
        return apiService.delete(`/api/prompts/${promptId}`);
    },
};

export default promptsApi;
