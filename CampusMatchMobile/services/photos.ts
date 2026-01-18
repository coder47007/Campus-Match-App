// Photos API service

import { apiService } from './api';
import { PhotoDto } from '../types';

export const photosApi = {
    // Get current user's photos
    getMyPhotos: async (): Promise<PhotoDto[]> => {
        return apiService.get<PhotoDto[]>('/api/photos');
    },

    // Upload a photo
    uploadPhoto: async (uri: string, mimeType: string = 'image/jpeg'): Promise<PhotoDto> => {
        const formData = new FormData();
        const filename = uri.split('/').pop() || 'photo.jpg';

        formData.append('file', {
            uri,
            name: filename,
            type: mimeType,
        } as unknown as Blob);

        return apiService.upload<PhotoDto>('/api/photos/upload', formData);
    },

    // Set photo as primary
    setPrimaryPhoto: async (photoId: number): Promise<void> => {
        return apiService.put(`/api/photos/${photoId}/primary`);
    },

    // Delete a photo
    deletePhoto: async (photoId: number): Promise<void> => {
        return apiService.delete(`/api/photos/${photoId}`);
    },

    // Reorder photos
    reorderPhotos: async (photoIds: number[]): Promise<void> => {
        return apiService.put('/api/photos/reorder', photoIds);
    },
};

export default photosApi;
