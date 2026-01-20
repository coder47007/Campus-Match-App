// Authentication API service using Supabase Auth directly
import { supabase, clearSessionCache } from './supabase';
import { setStorageItem, deleteStorageItem } from '../utils/storage';
import { STORAGE_KEYS } from '../constants/config';
import {
    AuthResponse,
    LoginRequest,
    RegisterRequest,
    ChangePasswordRequest,
    StudentDto,
} from '../types';

// Helper to map Supabase user to StudentDto format
const mapSupabaseToStudent = (row: any): StudentDto => ({
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

export const authApi = {
    // Login with Supabase Auth
    login: async (data: LoginRequest): Promise<AuthResponse> => {
        const { data: authData, error } = await supabase.auth.signInWithPassword({
            email: data.email,
            password: data.password,
        });

        if (error) {
            throw new Error(error.message);
        }

        if (!authData.session || !authData.user) {
            throw new Error('Login failed - no session returned');
        }

        // Get the student profile from the database
        const { data: student } = await supabase
            .from('Students')
            .select('*')
            .eq('Email', authData.user.email)
            .single();

        const mappedStudent = student ? mapSupabaseToStudent(student) : {
            id: 0,
            email: authData.user.email || '',
            name: authData.user.user_metadata?.name || 'User',
        };

        await setStorageItem(STORAGE_KEYS.AUTH_TOKEN, authData.session.access_token);
        if (authData.session.refresh_token) {
            await setStorageItem(STORAGE_KEYS.REFRESH_TOKEN, authData.session.refresh_token);
        }

        return {
            token: authData.session.access_token,
            refreshToken: authData.session.refresh_token,
            student: mappedStudent,
        };
    },

    // Register new user with Supabase Auth
    register: async (data: RegisterRequest): Promise<AuthResponse> => {
        // First, sign up with Supabase Auth
        const { data: authData, error } = await supabase.auth.signUp({
            email: data.email,
            password: data.password,
            options: {
                data: {
                    name: data.name,
                }
            }
        });

        if (error) {
            throw new Error(error.message);
        }

        if (!authData.session || !authData.user) {
            // Check if email confirmation is required
            if (authData.user && !authData.session) {
                throw new Error('Please check your email to confirm your account');
            }
            throw new Error('Registration failed - no session returned');
        }

        // Create student profile in the database
        const { error: insertError } = await supabase
            .from('Students')
            .insert({
                Id: authData.user.id,
                Email: data.email,
                Name: data.name,
                PhoneNumber: data.phoneNumber || '',
                InstagramHandle: data.instagramHandle || '',
                CreatedAt: new Date().toISOString(),
                LastActive: new Date().toISOString(),
            });

        if (insertError) {
            console.error('Error creating student profile:', insertError);
        }

        const mappedStudent: StudentDto = {
            id: 0, // Will be set properly on next fetch
            email: data.email,
            name: data.name,
            phoneNumber: data.phoneNumber,
            instagramHandle: data.instagramHandle,
        };

        await setStorageItem(STORAGE_KEYS.AUTH_TOKEN, authData.session.access_token);
        if (authData.session.refresh_token) {
            await setStorageItem(STORAGE_KEYS.REFRESH_TOKEN, authData.session.refresh_token);
        }

        return {
            token: authData.session.access_token,
            refreshToken: authData.session.refresh_token,
            student: mappedStudent,
        };
    },

    // Logout from Supabase
    logout: async (): Promise<void> => {
        const { error } = await supabase.auth.signOut();
        clearSessionCache(); // Clear cached ID
        await deleteStorageItem(STORAGE_KEYS.AUTH_TOKEN);
        await deleteStorageItem(STORAGE_KEYS.REFRESH_TOKEN);
        if (error) {
            throw new Error(error.message);
        }
    },

    // Forgot password - sends reset email via Supabase
    forgotPassword: async (email: string): Promise<{ message: string }> => {
        const { error } = await supabase.auth.resetPasswordForEmail(email, {
            redirectTo: 'campusmatch://reset-password',
        });

        if (error) {
            throw new Error(error.message);
        }

        return { message: 'Password reset email sent' };
    },

    // Change password for logged-in user
    changePassword: async (data: ChangePasswordRequest): Promise<{ message: string }> => {
        const { error } = await supabase.auth.updateUser({
            password: data.newPassword,
        });

        if (error) {
            throw new Error(error.message);
        }

        return { message: 'Password changed successfully' };
    },

    // Reset password (step 2 of forgot password flow)
    resetPassword: async (data: { email: string; token: string; newPassword: string }): Promise<{ message: string }> => {
        // Try to verify the OTP (token) first which signs the user in
        const { data: sessionData, error: verifyError } = await supabase.auth.verifyOtp({
            email: data.email,
            token: data.token,
            type: 'recovery',
        });

        if (verifyError) {
            throw new Error(verifyError.message);
        }

        if (!sessionData.session) {
            throw new Error('Invalid or expired token');
        }

        // Once signed in, update the password
        const { error: updateError } = await supabase.auth.updateUser({
            password: data.newPassword,
        });

        if (updateError) {
            throw new Error(updateError.message);
        }

        return { message: 'Password reset successfully' };
    },

    // Delete account
    deleteAccount: async (): Promise<void> => {
        // PHASE 3: Use REST API instead of direct database access
        try {
            const { data: { user } } = await supabase.auth.getUser();

            if (user) {
                // Call backend API to delete account (includes business logic & validation)
                const api = (await import('./api')).default;
                await api.delete('/profiles/me');
            }
        } catch (error) {
            console.error('Error deleting account via API:', error);
            // Fallback: still sign out even if API call fails
        }

        // Sign out from Supabase auth
        await supabase.auth.signOut();
        await deleteStorageItem(STORAGE_KEYS.AUTH_TOKEN);
        await deleteStorageItem(STORAGE_KEYS.REFRESH_TOKEN);
    },

    // Get current session
    getCurrentSession: async (): Promise<AuthResponse | null> => {
        const { data: { session } } = await supabase.auth.getSession();
        if (!session) return null;

        const { data: { user } } = await supabase.auth.getUser();
        if (!user) return null;

        const { data: student } = await supabase
            .from('Students')
            .select('*')
            .eq('Email', user.email)
            .single();

        return {
            token: session.access_token,
            refreshToken: session.refresh_token,
            student: student ? mapSupabaseToStudent(student) : {
                id: 0,
                email: user.email || '',
                name: user.user_metadata?.name || 'User',
            },
        };
    },

    // Refresh token
    refreshToken: async (): Promise<AuthResponse> => {
        const { data, error } = await supabase.auth.refreshSession();

        if (error || !data.session) {
            throw new Error(error?.message || 'Failed to refresh session');
        }

        const { data: student } = await supabase
            .from('Students')
            .select('*')
            .eq('Email', data.user?.email)
            .single();

        await setStorageItem(STORAGE_KEYS.AUTH_TOKEN, data.session.access_token);
        if (data.session.refresh_token) {
            await setStorageItem(STORAGE_KEYS.REFRESH_TOKEN, data.session.refresh_token);
        }

        return {
            token: data.session.access_token,
            refreshToken: data.session.refresh_token,
            student: student ? mapSupabaseToStudent(student) : {
                id: 0,
                email: data.user?.email || '',
                name: data.user?.user_metadata?.name || 'User',
            },
        };
    },
};

export default authApi;
