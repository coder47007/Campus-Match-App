// Matches service using Supabase directly
import { supabase, getCurrentUserId, getStudentId } from './supabase';
import { MatchDto } from '../types';

export const matchesApi = {
    // Get all matches
    getMatches: async (): Promise<MatchDto[]> => {
        const studentId = await getStudentId();
        if (!studentId) throw new Error('Student profile not found');

        // Get matches where user is either Student1 or Student2
        const { data, error } = await supabase
            .from('Matches')
            .select('*')
            .or(`Student1Id.eq.${studentId},Student2Id.eq.${studentId}`)
            .eq('IsActive', true);

        if (error) throw new Error(error.message);

        // Manually fetch user data for each match
        const matches: MatchDto[] = [];
        for (const match of data || []) {
            const otherUserId = match.Student1Id === studentId ? match.Student2Id : match.Student1Id;
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
        const studentId = await getStudentId();
        if (!studentId) throw new Error('Student profile not found');

        // Set match to inactive rather than deleting
        const { error } = await supabase
            .from('Matches')
            .update({ IsActive: false })
            .eq('Id', matchId)
            .or(`Student1Id.eq.${studentId},Student2Id.eq.${studentId}`);

        if (error) throw new Error(error.message);
    },
};

export default matchesApi;
