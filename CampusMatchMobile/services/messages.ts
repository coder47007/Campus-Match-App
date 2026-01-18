// Messages API service

import { apiService } from './api';
import { MessageDto, SendMessageRequest } from '../types';

export const messagesApi = {
    // Get messages for a match
    getMessages: async (matchId: number): Promise<MessageDto[]> => {
        return apiService.get<MessageDto[]>(`/api/messages/${matchId}`);
    },

    // Send a message
    sendMessage: async (data: SendMessageRequest): Promise<MessageDto> => {
        return apiService.post<MessageDto>('/api/messages', data);
    },

    // Mark messages as delivered
    markDelivered: async (matchId: number, messageIds: number[]): Promise<void> => {
        return apiService.put(`/api/messages/${matchId}/delivered`, { messageIds });
    },

    // Mark messages as read
    markRead: async (matchId: number, messageIds: number[]): Promise<void> => {
        return apiService.put(`/api/messages/${matchId}/read`, { messageIds });
    },
};

export default messagesApi;
