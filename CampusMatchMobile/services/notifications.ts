// Push Notifications Service

import * as Notifications from 'expo-notifications';
import { Platform } from 'react-native';
import Constants from 'expo-constants';

// Configure notification behavior
Notifications.setNotificationHandler({
    handleNotification: async () => ({
        shouldShowAlert: true,
        shouldPlaySound: true,
        shouldSetBadge: true,
    }),
});

export const notificationService = {
    // Request notification permissions
    async requestPermissions(): Promise<boolean> {
        const { status: existingStatus } = await Notifications.getPermissionsAsync();
        let finalStatus = existingStatus;

        if (existingStatus !== 'granted') {
            const { status } = await Notifications.requestPermissionsAsync();
            finalStatus = status;
        }

        return finalStatus === 'granted';
    },

    // Get push token for device
    async getPushToken(): Promise<string | null> {
        try {
            const projectId = Constants.expoConfig?.extra?.eas?.projectId;

            if (!projectId) {
                console.warn('Project ID not found');
                return null;
            }

            const token = await Notifications.getExpoPushTokenAsync({
                projectId,
            });

            return token.data;
        } catch (error) {
            console.error('Error getting push token:', error);
            return null;
        }
    },

    // Register for push notifications
    async registerForPushNotifications(): Promise<string | null> {
        if (Platform.OS === 'android') {
            await Notifications.setNotificationChannelAsync('default', {
                name: 'default',
                importance: Notifications.AndroidImportance.MAX,
                vibrationPattern: [0, 250, 250, 250],
                lightColor: '#FF6B6B',
            });
        }

        const hasPermission = await this.requestPermissions();
        if (!hasPermission) {
            return null;
        }

        return await this.getPushToken();
    },

    // Schedule local notification (for testing)
    async scheduleNotification(title: string, body: string, data?: any) {
        await Notifications.scheduleNotificationAsync({
            content: {
                title,
                body,
                data,
                sound: true,
            },
            trigger: null, // Show immediately
        });
    },

    // Listen for notifications
    addNotificationReceivedListener(callback: (notification: Notifications.Notification) => void) {
        return Notifications.addNotificationReceivedListener(callback);
    },

    // Listen for notification responses (user taps notification)
    addNotificationResponseReceivedListener(
        callback: (response: Notifications.NotificationResponse) => void
    ) {
        return Notifications.addNotificationResponseReceivedListener(callback);
    },

    // Get badge count
    async getBadgeCount(): Promise<number> {
        return await Notifications.getBadgeCountAsync();
    },

    // Set badge count
    async setBadgeCount(count: number) {
        await Notifications.setBadgeCountAsync(count);
    },

    // Clear all notifications
    async clearAll() {
        await Notifications.dismissAllNotificationsAsync();
        await this.setBadgeCount(0);
    },
};

export default notificationService;
