// Messages service using Supabase directly
import { supabase, getCurrentUserId } from './supabase';
import { MessageDto, SendMessageRequest } from '../types';

export const messagesApi = {
    // Get messages for a match
    getMessages: async (matchId: number): Promise<MessageDto[]> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        const { data, error } = await supabase
            .from('Messages')
            .select('*')
            .eq('MatchId', matchId)
            .order('SentAt', { ascending: true });

        if (error) throw new Error(error.message);

        // Get sender names
        const senderIds = [...new Set((data || []).map(m => m.SenderId))];
        const { data: senders } = await supabase
            .from('Students')
            .select('Id, Name')
            .in('Id', senderIds);

        const senderMap = new Map(senders?.map(s => [s.Id, s.Name]) || []);

        return (data || []).map(msg => ({
            id: msg.Id,
            senderId: msg.SenderId,
            senderName: senderMap.get(msg.SenderId) || 'User',
            content: msg.Content,
            sentAt: msg.SentAt || msg.CreatedAt,
            isRead: !!msg.ReadAt,
            deliveredAt: msg.DeliveredAt,
            readAt: msg.ReadAt,
        }));
    },

    // Send a message
    sendMessage: async (data: SendMessageRequest): Promise<MessageDto> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        // Get sender name
        const { data: sender } = await supabase
            .from('Students')
            .select('Name')
            .eq('Id', userId)
            .single();

        const { data: msg, error } = await supabase
            .from('Messages')
            .insert({
                MatchId: data.matchId,
                SenderId: userId,
                Content: data.content,
                SentAt: new Date().toISOString(),
            })
            .select()
            .single();

        if (error) throw new Error(error.message);

        // Update match's last message
        await supabase
            .from('Matches')
            .update({
                LastMessage: data.content,
                LastMessageAt: new Date().toISOString(),
            })
            .eq('Id', data.matchId);

        return {
            id: msg.Id,
            senderId: msg.SenderId,
            senderName: sender?.Name || 'User',
            content: msg.Content,
            sentAt: msg.SentAt,
            isRead: false,
            deliveredAt: msg.DeliveredAt,
            readAt: msg.ReadAt,
        };
    },

    // Mark messages as delivered
    markDelivered: async (matchId: number, messageIds: number[]): Promise<void> => {
        const now = new Date().toISOString();

        await supabase
            .from('Messages')
            .update({ DeliveredAt: now })
            .eq('MatchId', matchId)
            .in('Id', messageIds)
            .is('DeliveredAt', null);
    },

    // Mark messages as read
    markRead: async (matchId: number, messageIds: number[]): Promise<void> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        const now = new Date().toISOString();

        await supabase
            .from('Messages')
            .update({ ReadAt: now })
            .eq('MatchId', matchId)
            .in('Id', messageIds)
            .neq('SenderId', userId)
            .is('ReadAt', null);
    },
};

export default messagesApi;
