// Profiles service using Supabase directly
import { supabase, getCurrentUserId } from './supabase';
import {
    StudentDto,
    ProfileUpdateRequest,
    InterestDto,
} from '../types';

// Map database row to StudentDto
const mapStudent = (row: any): StudentDto => ({
    id: row.Id || row.id,
    email: row.Email || row.email,
    name: row.Name || row.name || `${row.FirstName || ''} ${row.LastName || ''}`.trim(),
    age: row.Age || row.age,
    major: row.Major || row.major,
    year: row.Year || row.year,
    bio: row.Bio || row.bio,
    photoUrl: row.PhotoUrl || row.photoUrl || (row.Photos?.[0]),
    university: row.University || row.university,
    gender: row.Gender || row.gender,
    preferredGender: row.PreferredGender || row.preferredGender || row.GenderPreference,
    phoneNumber: row.PhoneNumber || row.phoneNumber,
    instagramHandle: row.InstagramHandle || row.instagramHandle,
    latitude: row.Latitude || row.latitude,
    longitude: row.Longitude || row.longitude,
    interests: row.Interests || row.interests || [],
    photos: row.Photos || row.photos || [],
    prompts: row.Prompts || row.prompts || [],
});

export const profilesApi = {
    // Get current user's profile
    getMyProfile: async (): Promise<StudentDto> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        const { data, error } = await supabase
            .from('Students')
            .select('*')
            .eq('Id', userId)
            .single();

        if (error) throw new Error(error.message);
        return mapStudent(data);
    },

    // Update current user's profile
    updateMyProfile: async (profileData: ProfileUpdateRequest): Promise<StudentDto> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        const updateData: any = {};
        if (profileData.name !== undefined) updateData.Name = profileData.name;
        if (profileData.age !== undefined) updateData.Age = profileData.age;
        if (profileData.university !== undefined) updateData.University = profileData.university;
        if (profileData.major !== undefined) updateData.Major = profileData.major;
        if (profileData.year !== undefined) updateData.Year = profileData.year;
        if (profileData.gender !== undefined) updateData.Gender = profileData.gender;
        if (profileData.preferredGender !== undefined) updateData.PreferredGender = profileData.preferredGender;
        if (profileData.bio !== undefined) updateData.Bio = profileData.bio;
        if (profileData.phoneNumber !== undefined) updateData.PhoneNumber = profileData.phoneNumber;
        if (profileData.instagramHandle !== undefined) updateData.InstagramHandle = profileData.instagramHandle;
        if (profileData.latitude !== undefined) updateData.Latitude = profileData.latitude;
        if (profileData.longitude !== undefined) updateData.Longitude = profileData.longitude;
        if (profileData.photoUrl !== undefined) updateData.PhotoUrl = profileData.photoUrl;
        updateData.LastActive = new Date().toISOString();

        const { data, error } = await supabase
            .from('Students')
            .update(updateData)
            .eq('Id', userId)
            .select()
            .single();

        if (error) throw new Error(error.message);
        return mapStudent(data);
    },

    // Get discover profiles (for swiping)
    getDiscoverProfiles: async (filters?: {
        minAge?: number;
        maxAge?: number;
        gender?: string;
    }): Promise<StudentDto[]> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        // Get already swiped profiles
        const { data: swipes } = await supabase
            .from('Swipes')
            .select('SwipedId')
            .eq('SwiperId', userId);

        const swipedIds = swipes?.map(s => s.SwipedId) || [];

        // Build query
        let query = supabase
            .from('Students')
            .select('*')
            .neq('Id', userId)
            .limit(20);

        // Filter out already swiped
        if (swipedIds.length > 0) {
            query = query.not('Id', 'in', `(${swipedIds.join(',')})`);
        }

        if (filters?.gender) {
            query = query.eq('Gender', filters.gender);
        }

        const { data, error } = await query;

        if (error) throw new Error(error.message);

        // Filter by age if needed
        let profiles = (data || []).map(mapStudent);

        if (filters?.minAge || filters?.maxAge) {
            profiles = profiles.filter(p => {
                const age = p.age;
                if (!age) return true;
                if (filters.minAge && age < filters.minAge) return false;
                if (filters.maxAge && age > filters.maxAge) return false;
                return true;
            });
        }

        return profiles;
    },

    // Get all available interests
    getInterests: async (): Promise<InterestDto[]> => {
        const { data, error } = await supabase
            .from('Interests')
            .select('*');

        if (error) {
            // If table doesn't exist, return default interests
            return [
                { id: 1, name: 'Music', emoji: '🎵', category: 'Entertainment' },
                { id: 2, name: 'Sports', emoji: '⚽', category: 'Lifestyle' },
                { id: 3, name: 'Reading', emoji: '📚', category: 'Education' },
                { id: 4, name: 'Travel', emoji: '✈️', category: 'Lifestyle' },
                { id: 5, name: 'Gaming', emoji: '🎮', category: 'Entertainment' },
            ];
        }

        return data?.map(i => ({
            id: i.Id || i.id,
            name: i.Name || i.name,
            emoji: i.Emoji || i.emoji || '⭐',
            category: i.Category || i.category
        })) || [];
    },

    // Update user's interests
    updateInterests: async (interestIds: number[]): Promise<StudentDto> => {
        const userId = await getCurrentUserId();
        if (!userId) throw new Error('Not authenticated');

        const { data, error } = await supabase
            .from('Students')
            .update({ Interests: interestIds, LastActive: new Date().toISOString() })
            .eq('Id', userId)
            .select()
            .single();

        if (error) throw new Error(error.message);
        return mapStudent(data);
    },

    // Get a specific user's profile
    getProfile: async (userId: number): Promise<StudentDto> => {
        const { data, error } = await supabase
            .from('Students')
            .select('*')
            .eq('Id', userId)
            .single();

        if (error) throw new Error(error.message);
        return mapStudent(data);
    },
};

export default profilesApi;
