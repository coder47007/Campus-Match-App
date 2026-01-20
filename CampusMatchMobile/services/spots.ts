// Trending Spots API Service using Supabase

import { supabase, getStudentId } from './supabase';

export interface TrendingSpot {
    id: number;
    name: string;
    type: string;
    imageUrl: string;
    campus?: string;
    studentCount: number;
    isCheckedIn: boolean;
}

export interface SpotCheckIn {
    id: number;
    studentId: number;
    spotId: number;
    checkedInAt: string;
    expiresAt: string;
}

export interface WhosThere {
    id: number;
    name: string;
    photoUrl?: string;
    major?: string;
    checkedInAt: string;
}

export const spotsApi = {
    // Get all trending spots with check-in counts
    getSpots: async (): Promise<TrendingSpot[]> => {
        const studentId = await getStudentId();

        // Get all spots
        const { data: spots, error: spotsError } = await supabase
            .from('TrendingSpots')
            .select('*')
            .order('Name');

        if (spotsError) {
            console.error('Error fetching spots:', spotsError);
            return [];
        }

        // Get active check-ins count for each spot
        const { data: checkIns, error: checkInsError } = await supabase
            .from('SpotCheckIns')
            .select('SpotId, StudentId')
            .eq('IsActive', true)
            .gt('ExpiresAt', new Date().toISOString());

        if (checkInsError) {
            console.error('Error fetching check-ins:', checkInsError);
        }

        // Map spots with counts and user's check-in status
        return spots.map(spot => {
            const spotCheckIns = checkIns?.filter(c => c.SpotId === spot.Id) || [];
            return {
                id: spot.Id,
                name: spot.Name,
                type: spot.Type || 'Campus Location',
                imageUrl: spot.ImageUrl,
                campus: spot.Campus,
                studentCount: spotCheckIns.length,
                isCheckedIn: spotCheckIns.some(c => c.StudentId === studentId)
            };
        });
    },

    // Check in to a spot (expires in 3 hours)
    checkIn: async (spotId: number): Promise<boolean> => {
        const studentId = await getStudentId();
        if (!studentId) throw new Error('Not authenticated');

        // Check if already checked in
        const { data: existing } = await supabase
            .from('SpotCheckIns')
            .select('*')
            .eq('StudentId', studentId)
            .eq('SpotId', spotId)
            .eq('IsActive', true)
            .gt('ExpiresAt', new Date().toISOString())
            .single();

        if (existing) {
            return true;
        }

        // Create new check-in
        const expiresAt = new Date();
        expiresAt.setHours(expiresAt.getHours() + 3); // 3 hours from now

        const { error } = await supabase
            .from('SpotCheckIns')
            .insert({
                StudentId: studentId,
                SpotId: spotId,
                CheckedInAt: new Date().toISOString(),
                ExpiresAt: expiresAt.toISOString(),
                IsActive: true
            });

        if (error) {
            console.error('Check-in error:', error);
            return false;
        }


        return true;
    },

    // Check out from a spot
    checkOut: async (spotId: number): Promise<boolean> => {
        const studentId = await getStudentId();
        if (!studentId) throw new Error('Not authenticated');

        const { error } = await supabase
            .from('SpotCheckIns')
            .update({ IsActive: false })
            .eq('StudentId', studentId)
            .eq('SpotId', spotId)
            .eq('IsActive', true);

        if (error) {
            console.error('Check-out error:', error);
            return false;
        }


        return true;
    },

    // Get list of matches who are at this spot
    getWhosThere: async (spotId: number): Promise<WhosThere[]> => {
        const studentId = await getStudentId();
        if (!studentId) return [];

        // Get all active check-ins for this spot
        const { data: checkIns, error: checkInsError } = await supabase
            .from('SpotCheckIns')
            .select('StudentId, CheckedInAt')
            .eq('SpotId', spotId)
            .eq('IsActive', true)
            .gt('ExpiresAt', new Date().toISOString());

        if (checkInsError || !checkIns || checkIns.length === 0) {
            return [];
        }

        const studentIds = checkIns.map(c => c.StudentId);

        // Get student details for those checked in
        const { data: students, error: studentsError } = await supabase
            .from('Students')
            .select('Id, Name, PhotoUrl, Major')
            .in('Id', studentIds);

        if (studentsError) {
            console.error('Error fetching students:', studentsError);
            return [];
        }

        // Get user's matches to filter
        const { data: matches, error: matchesError } = await supabase
            .from('Matches')
            .select('Student1Id, Student2Id')
            .or(`Student1Id.eq.${studentId},Student2Id.eq.${studentId}`)
            .eq('IsActive', true);

        if (matchesError) {
            console.error('Error fetching matches:', matchesError);
        }

        // Get IDs of matched students
        const matchedIds = new Set(
            matches?.map(m => m.Student1Id === studentId ? m.Student2Id : m.Student1Id) || []
        );

        // Filter to only show matches
        return students
            .filter(s => matchedIds.has(s.Id))
            .map(s => {
                const checkIn = checkIns.find(c => c.StudentId === s.Id);
                return {
                    id: s.Id,
                    name: s.Name,
                    photoUrl: s.PhotoUrl,
                    major: s.Major,
                    checkedInAt: checkIn?.CheckedInAt || new Date().toISOString()
                };
            });
    },

    // Get total count (including non-matches) for a spot
    getTotalCount: async (spotId: number): Promise<number> => {
        const { count, error } = await supabase
            .from('SpotCheckIns')
            .select('*', { count: 'exact', head: true })
            .eq('SpotId', spotId)
            .eq('IsActive', true)
            .gt('ExpiresAt', new Date().toISOString());

        if (error) {
            console.error('Error getting count:', error);
            return 0;
        }

        return count || 0;
    }
};

export default spotsApi;
