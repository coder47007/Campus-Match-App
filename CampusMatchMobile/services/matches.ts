// Matches service using Supabase directly
import { supabase, getCurrentUserId } from './supabase';
import { MatchDto } from '../types';

export const matchesApi = {
    // Get all matches
    getMatches: async (): Promise<MatchDto[]> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        // Get matches where user is either Student1 or Student2
        const { data, error } = await supabase
            .from('Matches')
            .select('*')
            .or(`Student1Id.eq.${userId},Student2Id.eq.${userId}`)
            .eq('IsActive', true);

        if (error) throw new Error(error.message);

        // Manually fetch user data for each match
        const matches: MatchDto[] = [];
        for (const match of data || []) {
            const otherUserId = match.Student1Id === userId ? match.Student2Id : match.Student1Id;
            const { data: otherUser } = await supabase
                .from('Students')
                .select('*')
                .eq('Id', otherUserId)
                .single();

            if (otherUser) {
                matches.push({
                    id: match.Id,
                    otherStudentId: otherUser.Id,
                    otherStudentName: otherUser.Name || 'User',
                    otherStudentPhotoUrl: otherUser.PhotoUrl || otherUser.Photos?.[0],
                    otherStudentMajor: otherUser.Major,
                    matchedAt: match.MatchedAt,
                    isActive: match.IsActive,
                    lastMessage: match.LastMessage,
                });
            }
        }
        return matches;
    },

    // Unmatch (delete match)
    unmatch: async (matchId: number): Promise<void> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        // Set match to inactive rather than deleting
        const { error } = await supabase
            .from('Matches')
            .update({ IsActive: false })
            .eq('Id', matchId)
            .or(`Student1Id.eq.${userId},Student2Id.eq.${userId}`);

        if (error) throw new Error(error.message);
    },
};

export default matchesApi;
