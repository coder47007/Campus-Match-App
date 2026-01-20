// Photos API service using Supabase Storage

import { supabase, getStudentId } from './supabase';
import { PhotoDto } from '../types';

export const photosApi = {
    // Get current user's photos
    getMyPhotos: async (): Promise<PhotoDto[]> => {
        const studentId = await getStudentId();
        if (!studentId) return [];

        const { data, error } = await supabase
            .from('Photos')
            .select('*')
            .eq('StudentId', studentId)
            .order('DisplayOrder', { ascending: true });

        if (error) {
            console.error('Error fetching photos:', error);
            return [];
        }

        return data?.map(p => ({
            id: p.Id,
            url: p.Url,
            isPrimary: p.DisplayOrder === 0,
            displayOrder: p.DisplayOrder
        })) || [];
    },

    // Upload a photo to Supabase Storage
    uploadPhoto: async (uri: string, mimeType: string = 'image/jpeg'): Promise<PhotoDto> => {
        console.log('[Photo Upload] Starting upload...');
        const studentId = await getStudentId();
        console.log('[Photo Upload] Student ID:', studentId);

        if (!studentId) throw new Error('Not authenticated');

        try {
            // Create a unique filename
            const fileExt = mimeType.split('/')[1] || 'jpg';
            const fileName = `${studentId}_${Date.now()}.${fileExt}`;
            const filePath = `profiles/${fileName}`;
            console.log('[Photo Upload] File path:', filePath);

            // Fetch the file as ArrayBuffer (better for React Native)
            console.log('[Photo Upload] Fetching file from URI:', uri);
            const response = await fetch(uri);
            const arrayBuffer = await response.arrayBuffer();
            console.log('[Photo Upload] File size:', arrayBuffer.byteLength, 'bytes');

            // Upload to Supabase Storage using ArrayBuffer
            console.log('[Photo Upload] Uploading to Supabase Storage...');
            const { data: uploadData, error: uploadError } = await supabase.storage
                .from('photos')
                .upload(filePath, arrayBuffer, {
                    contentType: mimeType,
                    upsert: false
                });

            if (uploadError) {
                console.error('[Photo Upload] Upload error:', uploadError);
                console.error('[Photo Upload] Error details:', JSON.stringify(uploadError, null, 2));
                throw uploadError;
            }
            console.log('[Photo Upload] Upload successful!', uploadData);

            // Get public URL
            const { data: { publicUrl } } = supabase.storage
                .from('photos')
                .getPublicUrl(filePath);

            // Get current photo count to set display order
            const { count } = await supabase
                .from('Photos')
                .select('*', { count: 'exact', head: true })
                .eq('StudentId', studentId);

            const isFirstPhoto = (count || 0) === 0;

            // Create database record
            const { data: photoData, error: dbError } = await supabase
                .from('Photos')
                .insert({
                    StudentId: studentId,
                    Url: publicUrl,
                    BlobName: filePath,
                    IsPrimary: isFirstPhoto,
                    DisplayOrder: count || 0,
                    UploadedAt: new Date().toISOString()
                })
                .select()
                .single();

            if (dbError) throw dbError;

            return {
                id: photoData.Id,
                url: photoData.Url,
                isPrimary: photoData.DisplayOrder === 0,
                displayOrder: photoData.DisplayOrder
            };
        } catch (error) {
            console.error('Photo upload error:', error);
            throw error;
        }
    },

    // Set photo as primary
    setPrimaryPhoto: async (photoId: number): Promise<void> => {
        const studentId = await getStudentId();
        if (!studentId) throw new Error('Not authenticated');

        // Set all photos to non-primary
        await supabase
            .from('Photos')
            .update({ DisplayOrder: 99 })
            .eq('StudentId', studentId);

        // Set this photo as primary (DisplayOrder 0)
        const { error } = await supabase
            .from('Photos')
            .update({ DisplayOrder: 0 })
            .eq('Id', photoId);

        if (error) throw error;
    },

    // Delete a photo
    deletePhoto: async (photoId: number): Promise<void> => {
        // Get photo details first
        const { data: photo, error: fetchError } = await supabase
            .from('Photos')
            .select('BlobName')
            .eq('Id', photoId)
            .single();

        if (fetchError) throw fetchError;

        // Delete from storage
        if (photo.BlobName) {
            await supabase.storage
                .from('photos')
                .remove([photo.BlobName]);
        }

        // Delete from database
        const { error } = await supabase
            .from('Photos')
            .delete()
            .eq('Id', photoId);

        if (error) throw error;
    },

    // Reorder photos
    reorderPhotos: async (photoIds: number[]): Promise<void> => {
        // Update each photo's display order
        const updates = photoIds.map((photoId, index) =>
            supabase
                .from('Photos')
                .update({ DisplayOrder: index })
                .eq('Id', photoId)
        );

        await Promise.all(updates);
    },
};

export default photosApi;
