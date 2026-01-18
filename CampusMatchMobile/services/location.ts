// Location Service for geolocation and distance calculations

import * as Location from 'expo-location';

export const locationService = {
    // Request location permissions
    async requestPermissions(): Promise<boolean> {
        const { status } = await Location.requestForegroundPermissionsAsync();
        return status === 'granted';
    },

    // Get current location
    async getCurrentLocation(): Promise<{ latitude: number; longitude: number } | null> {
        try {
            const hasPermission = await this.requestPermissions();
            if (!hasPermission) return null;

            const location = await Location.getCurrentPositionAsync({
                accuracy: Location.Accuracy.Balanced,
            });

            return {
                latitude: location.coords.latitude,
                longitude: location.coords.longitude,
            };
        } catch (error) {
            console.error('Error getting location:', error);
            return null;
        }
    },

    // Calculate distance between two points using Haversine formula
    calculateDistance(
        lat1: number,
        lon1: number,
        lat2: number,
        lon2: number
    ): number {
        const R = 3959; // Earth radius in miles
        const dLat = this.toRad(lat2 - lat1);
        const dLon = this.toRad(lon2 - lon1);

        const a =
            Math.sin(dLat / 2) * Math.sin(dLat / 2) +
            Math.cos(this.toRad(lat1)) *
            Math.cos(this.toRad(lat2)) *
            Math.sin(dLon / 2) *
            Math.sin(dLon / 2);

        const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
        const distance = R * c;

        return Math.round(distance * 10) / 10; // Round to 1 decimal
    },

    toRad(degrees: number): number {
        return (degrees * Math.PI) / 180;
    },

    // Format distance for display
    formatDistance(miles: number): string {
        if (miles < 1) {
            return '< 1 mile away';
        } else if (miles === 1) {
            return '1 mile away';
        } else {
            return `${Math.round(miles)} miles away`;
        }
    },
};

export default locationService;
