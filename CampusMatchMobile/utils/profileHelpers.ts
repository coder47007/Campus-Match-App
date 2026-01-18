// Profile completion calculator

import { StudentDto } from '@/types';

export function calculateProfileCompletion(user: StudentDto | null): number {
    if (!user) return 0;

    let completed = 0;
    const total = 10;

    // Basic info (4 points)
    if (user.name) completed++;
    if (user.age) completed++;
    if (user.major) completed++;
    if (user.bio && user.bio.length >= 20) completed++;

    // Photos (2 points)
    if (user.photoUrl) completed++;
    if (user.photos && user.photos.length >= 2) completed++;

    // Interests & Prompts (2 points)
    if (user.interests && user.interests.length >= 3) completed++;
    if (user.prompts && user.prompts.length >= 1) completed++;

    // Additional info (2 points)
    if (user.university) completed++;
    if (user.year) completed++;

    return Math.round((completed / total) * 100);
}

export function getCompletionSuggestions(user: StudentDto | null): string[] {
    if (!user) return [];

    const suggestions: string[] = [];

    if (!user.bio || user.bio.length < 20) {
        suggestions.push('Add a bio (at least 20 characters)');
    }
    if (!user.photoUrl || (user.photos && user.photos.length < 3)) {
        suggestions.push('Upload at least 3 photos');
    }
    if (!user.interests || user.interests.length < 5) {
        suggestions.push('Select at least 5 interests');
    }
    if (!user.prompts || user.prompts.length < 2) {
        suggestions.push('Answer at least 2 prompts');
    }
    if (!user.major) {
        suggestions.push('Add your major');
    }
    if (!user.year) {
        suggestions.push('Add your academic year');
    }

    return suggestions;
}
