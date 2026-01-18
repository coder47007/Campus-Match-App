import { Platform } from 'react-native';
import * as SecureStore from 'expo-secure-store';

export const setStorageItem = async (key: string, value: string) => {
    if (Platform.OS === 'web') {
        try {
            if (typeof localStorage !== 'undefined') {
                localStorage.setItem(key, value);
            }
        } catch (e) {
            console.error('Local storage error:', e);
        }
    } else {
        await SecureStore.setItemAsync(key, value);
    }
};

export const getStorageItem = async (key: string) => {
    if (Platform.OS === 'web') {
        try {
            if (typeof localStorage !== 'undefined') {
                return localStorage.getItem(key);
            }
            return null;
        } catch (e) {
            console.error('Local storage error:', e);
            return null;
        }
    } else {
        return await SecureStore.getItemAsync(key);
    }
};

export const deleteStorageItem = async (key: string) => {
    if (Platform.OS === 'web') {
        try {
            if (typeof localStorage !== 'undefined') {
                localStorage.removeItem(key);
            }
        } catch (e) {
            console.error('Local storage error:', e);
        }
    } else {
        await SecureStore.deleteItemAsync(key);
    }
};
