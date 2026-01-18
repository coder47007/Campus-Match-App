// Enhanced error handling utilities

import { AxiosError } from 'axios';

export class AppError extends Error {
    constructor(
        message: string,
        public code?: string,
        public statusCode?: number
    ) {
        super(message);
        this.name = 'AppError';
    }
}

export function handleApiError(error: unknown): string {
    if (error instanceof AxiosError) {
        if (error.response) {
            // Server responded with error
            const message = error.response.data?.message || error.response.data || 'Server error';
            return typeof message === 'string' ? message : 'An error occurred';
        } else if (error.request) {
            // No response received
            return 'No internet connection. Please check your network.';
        }
    }

    if (error instanceof Error) {
        return error.message;
    }

    return 'An unexpected error occurred';
}

export function isNetworkError(error: unknown): boolean {
    if (error instanceof AxiosError) {
        return !error.response && !!error.request;
    }
    return false;
}

export async function retryWithBackoff<T>(
    fn: () => Promise<T>,
    maxRetries: number = 3,
    baseDelay: number = 1000
): Promise<T> {
    let lastError: unknown;

    for (let i = 0; i < maxRetries; i++) {
        try {
            return await fn();
        } catch (error) {
            lastError = error;

            // Don't retry on 4xx errors (client errors)
            if (error instanceof AxiosError && error.response?.status && error.response.status < 500) {
                throw error;
            }

            if (i < maxRetries - 1) {
                const delay = baseDelay * Math.pow(2, i);
                await new Promise(resolve => setTimeout(resolve, delay));
            }
        }
    }

    throw lastError;
}
