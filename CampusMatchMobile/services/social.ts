// Social sharing utilities

import { Share, Linking } from 'react-native';
import * as Sharing from 'expo-sharing';

export const socialService = {
    // Open Instagram profile
    async openInstagram(handle: string) {
        if (!handle) return;

        const cleanHandle = handle.replace('@', '');
        const instagramUrl = `instagram://user?username=${cleanHandle}`;
        const webUrl = `https://instagram.com/${cleanHandle}`;

        try {
            const canOpen = await Linking.canOpenURL(instagramUrl);
            if (canOpen) {
                await Linking.openURL(instagramUrl);
            } else {
                await Linking.openURL(webUrl);
            }
        } catch (error) {
            await Linking.openURL(webUrl);
        }
    },

    // Share profile
    async shareProfile(userId: number, userName: string) {
        try {
            const deepLink = `campusmatch://profile/${userId}`;
            const message = `Check out ${userName}'s profile on CampusMatch! ${deepLink}`;

            await Share.share({
                message,
                title: `${userName} on CampusMatch`,
            });
        } catch (error) {
            console.error('Error sharing:', error);
        }
    },

    // Share content
    async shareContent(title: string, message: string, url?: string) {
        try {
            await Share.share({
                title,
                message: url ? `${message}\n${url}` : message,
            });
        } catch (error) {
            console.error('Error sharing:', error);
        }
    },

    // Share file
    async shareFile(fileUri: string) {
        try {
            const isAvailable = await Sharing.isAvailableAsync();
            if (isAvailable) {
                await Sharing.shareAsync(fileUri);
            }
        } catch (error) {
            console.error('Error sharing file:', error);
        }
    },
};

export default socialService;
