import Constants from 'expo-constants';

// API Configuration
// Change this to your API server URL when testing on physical devices
// For Android emulator: use 10.0.2.2 instead of localhost
// For iOS simulator: localhost works fine
// For physical devices: use your computer's IP address on the network
const extra = Constants.expoConfig?.extra;
export const API_BASE_URL = extra?.apiUrl || 'http://10.0.0.56:5229';

// SignalR Hub URL
export const SIGNALR_HUB_URL = `${API_BASE_URL}/chathub`;

// Storage keys
export const STORAGE_KEYS = {
    AUTH_TOKEN: 'auth_token',
    REFRESH_TOKEN: 'refresh_token',
    USER_DATA: 'user_data',
} as const;

// API Timeouts
export const API_TIMEOUT = 30000; // 30 seconds
export const MAX_RETRIES = 3;
export const RETRY_DELAY = 1000; // 1 second

// Photo limits
export const MAX_PHOTOS = 6;
export const MAX_FILE_SIZE_BYTES = 10 * 1024 * 1024; // 10MB

// Prompts limit
export const MAX_PROMPTS = 3;

// Age range
export const MIN_AGE = 18;
export const MAX_AGE = 100;

// Distance range (in miles/km)
export const MIN_DISTANCE = 1;
export const MAX_DISTANCE = 100;
