// Supabase client configuration for direct database access
import 'react-native-url-polyfill/auto';
import { createClient } from '@supabase/supabase-js';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { SUPABASE_URL, SUPABASE_ANON_KEY } from '../constants/config';

// Create Supabase client with AsyncStorage for session persistence
export const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: {
        storage: AsyncStorage,
        autoRefreshToken: true,
        persistSession: true,
        detectSessionInUrl: false, // Important for React Native
    },
});

// Helper to get the current user ID
export const getCurrentUserId = async (): Promise<string | null> => {
    const { data: { user } } = await supabase.auth.getUser();
    return user?.id || null;
};

// Helper to get current session
export const getCurrentSession = async () => {
    const { data: { session } } = await supabase.auth.getSession();
    return session;
};

// Helper to get the integer Student Id from the authenticated user's email
let cachedStudentId: number | null = null;
let cachedEmail: string | null = null; // To invalidate if user changes

export const clearSessionCache = () => {
    cachedStudentId = null;
    cachedEmail = null;
};

export const getStudentId = async (): Promise<number | null> => {
    let { data: { user } } = await supabase.auth.getUser();

    // Fallback to session if getUser fails (network or token issue)
    if (!user) {
        const { data: { session } } = await supabase.auth.getSession();
        user = session?.user || null;
    }

    // PRODUCTION: Proper authentication enforcement - no fallbacks
    if (!user || !user.email) {
        console.error('[getStudentId] No authenticated user found');
        throw new Error('Authentication required. Please log in.');
    }

    // Return cached ID if email matches
    if (cachedStudentId && cachedEmail === user.email) {
        return cachedStudentId;
    }

    // Otherwise fetch fresh
    const { data, error } = await supabase
        .from('Students')
        .select('Id')
        .eq('Email', user.email)
        .single();

    if (data?.Id) {
        cachedStudentId = data.Id;
        cachedEmail = user.email;
        return data.Id;
    }

    // If profile missing (e.g. registration error), auto-create it now

    const { data: newProfile, error: createError } = await supabase
        .from('Students')
        .insert({
            Email: user.email,
            Name: user.user_metadata?.full_name || 'New Student',
            Age: 18,
            Gender: 'Other',
            University: 'CampusMatch U',
            Year: 'Freshman',
            Major: 'Undeclared',
            Bio: 'Ready to match!',
            PhoneNumber: '000-000-0000',
            InstagramHandle: '@newuser',
            PhotoUrl: 'https://randomuser.me/api/portraits/lego/1.jpg',
            PasswordHash: 'external-auth-placeholder',
            CreatedAt: new Date().toISOString(),
            LastActiveAt: new Date().toISOString(),
            // Required Integer/Boolean Defaults from Entities.cs
            SuperLikesRemaining: 3,
            SuperLikesResetAt: new Date(Date.now() + 86400000).toISOString(),
            RewindsRemaining: 1,
            RewindsResetAt: new Date(Date.now() + 86400000).toISOString(),
            MinAgePreference: 18,
            MaxAgePreference: 30,
            MaxDistancePreference: 25,
            ShowOnlineStatus: true,
            NotifyOnMatch: true,
            NotifyOnMessage: true,
            NotifyOnSuperLike: true,
            IsProfileHidden: false,
            IsAdmin: false,
            IsBanned: false,
            IsBoosted: false
        })
        .select('Id')
        .single();

    if (createError) {
        console.error('Failed to auto-create profile:', createError.message);
        throw new Error('Failed to create user profile. Please contact support.');
    }

    cachedStudentId = newProfile.Id;
    cachedEmail = user.email;
    return newProfile.Id;
};

export default supabase;
