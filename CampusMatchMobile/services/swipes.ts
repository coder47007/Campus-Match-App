// Swipes service using Supabase directly
import { supabase, getCurrentUserId, getStudentId } from './supabase';
import {
    SwipeRequest,
    SwipeResponse,
    UndoSwipeResponse,
    RewindsRemainingResponse,
    MatchDto,
} from '../types';

export const swipesApi = {
    // Create a swipe (like/dislike/super like)
    swipe: async (data: SwipeRequest): Promise<SwipeResponse> => {
        const studentId = await getStudentId();
        if (!studentId) throw new Error('Student profile not found');

        // Insert the swipe
        const { error } = await supabase
            .from('Swipes')
            .upsert({
                SwiperId: studentId,
                SwipedId: data.swipedId,
                IsLike: data.isLike,
                IsSuperLike: data.isSuperLike || false,
                CreatedAt: new Date().toISOString(),
            }, { onConflict: 'SwiperId,SwipedId' });

        if (error) throw new Error(error.message);

        // Check for mutual like (match)
        let matchData: MatchDto | undefined;
        if (data.isLike) {
            const { data: reciprocalSwipe } = await supabase
                .from('Swipes')
                .select('*')
                .eq('SwiperId', data.swipedId)
                .eq('SwipedId', studentId)
                .eq('IsLike', true)
                .single();

            if (reciprocalSwipe) {
                // Create match record
                const { data: newMatch, error: matchError } = await supabase
                    .from('Matches')
                    .insert({
                        Student1Id: studentId,
                        Student2Id: data.swipedId,
                        MatchedAt: new Date().toISOString(),
                        IsActive: true,
                    })
                    .select()
                    .single();

                if (!matchError && newMatch) {
                    // Get the other student's info
                    const { data: otherStudent } = await supabase
                        .from('Students')
                        .select('*')
                        .eq('Id', data.swipedId)
                        .single();

                    matchData = {
                        id: newMatch.Id,
                        otherStudentId: data.swipedId,
                        otherStudentName: otherStudent?.Name || 'User',
                        otherStudentPhotoUrl: otherStudent?.PhotoUrl || otherStudent?.Photos?.[0],
                        otherStudentMajor: otherStudent?.Major,
                        matchedAt: newMatch.MatchedAt,
                        isActive: true,
                    };
                }
            }
        }

        return {
            isMatch: !!matchData,
            match: matchData,
        };
    },

    // Undo last swipe (rewind)
    undoSwipe: async (): Promise<UndoSwipeResponse> => {
        const studentId = await getStudentId();
        if (!studentId) throw new Error('Student profile not found');

        // Get last swipe
        const { data: lastSwipe, error: fetchError } = await supabase
            .from('Swipes')
            .select('*')
            .eq('SwiperId', studentId)
            .order('CreatedAt', { ascending: false })
            .limit(1)
            .single();

        if (fetchError || !lastSwipe) {
            return { success: false, message: 'No swipe to undo' };
        }

        // Delete the last swipe
        const { error: deleteError } = await supabase
            .from('Swipes')
            .delete()
            .eq('Id', lastSwipe.Id);

        if (deleteError) {
            return { success: false, message: deleteError.message };
        }

        // Also delete any match that was created
        await supabase
            .from('Matches')
            .delete()
            .or(`and(Student1Id.eq.${studentId},Student2Id.eq.${lastSwipe.SwipedId}),and(Student1Id.eq.${lastSwipe.SwipedId},Student2Id.eq.${studentId})`);

        return { success: true };
    },

    // Get remaining rewinds count (for now, return default)
    getRewindsRemaining: async (): Promise<RewindsRemainingResponse> => {
        return {
            remaining: 5,
            resetsAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(),
        };
    },
};

export default swipesApi;
