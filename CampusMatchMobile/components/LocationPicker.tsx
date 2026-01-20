// Location Picker Component - Share current location in chat
import React, { useState, useEffect } from 'react';
import {
    View,
    Text,
    StyleSheet,
    Modal,
    TouchableOpacity,
    ActivityIndicator,
    Image,
    Linking,
    Platform,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import * as Location from 'expo-location';
import Colors from '@/constants/Colors';

interface LocationPickerProps {
    visible: boolean;
    onClose: () => void;
    onSelect: (location: { latitude: number; longitude: number; address?: string }) => void;
}

export default function LocationPicker({ visible, onClose, onSelect }: LocationPickerProps) {
    const [isLoading, setIsLoading] = useState(false);
    const [hasPermission, setHasPermission] = useState<boolean | null>(null);
    const [currentLocation, setCurrentLocation] = useState<{
        latitude: number;
        longitude: number;
        address?: string;
    } | null>(null);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (visible) {
            requestLocationPermission();
        }
    }, [visible]);

    const requestLocationPermission = async () => {
        setIsLoading(true);
        setError(null);

        try {
            const { status } = await Location.requestForegroundPermissionsAsync();

            if (status !== 'granted') {
                setHasPermission(false);
                setError('Location permission denied');
                setIsLoading(false);
                return;
            }

            setHasPermission(true);
            await getCurrentLocation();
        } catch (err) {
            setError('Failed to request location permission');
            setIsLoading(false);
        }
    };

    const getCurrentLocation = async () => {
        try {
            const location = await Location.getCurrentPositionAsync({
                accuracy: Location.Accuracy.Balanced,
            });

            const { latitude, longitude } = location.coords;

            // Get address from coordinates
            let address = '';
            try {
                const [geocoded] = await Location.reverseGeocodeAsync({ latitude, longitude });
                if (geocoded) {
                    const parts = [
                        geocoded.street,
                        geocoded.city,
                        geocoded.region,
                    ].filter(Boolean);
                    address = parts.join(', ');
                }
            } catch {
                // Geocoding failed, use coordinates
            }

            setCurrentLocation({ latitude, longitude, address });
        } catch (err) {
            setError('Failed to get current location');
        }
        setIsLoading(false);
    };

    const handleShareLocation = () => {
        if (currentLocation) {
            onSelect(currentLocation);
            onClose();
        }
    };

    const openInMaps = () => {
        if (!currentLocation) return;

        const { latitude, longitude } = currentLocation;
        const url = Platform.select({
            ios: `maps:0,0?q=${latitude},${longitude}`,
            android: `geo:${latitude},${longitude}?q=${latitude},${longitude}`,
        });

        if (url) {
            Linking.openURL(url).catch(() => {
                // Fallback to Google Maps web
                Linking.openURL(`https://www.google.com/maps?q=${latitude},${longitude}`);
            });
        }
    };

    // Static map image from Google Maps
    const getMapImageUrl = () => {
        if (!currentLocation) return '';
        const { latitude, longitude } = currentLocation;
        // Using OpenStreetMap static tiles (free, no API key needed)
        return `https://staticmap.openstreetmap.de/staticmap.php?center=${latitude},${longitude}&zoom=15&size=400x200&maptype=mapnik&markers=${latitude},${longitude},red`;
    };

    return (
        <Modal
            visible={visible}
            animationType="slide"
            transparent
            onRequestClose={onClose}
        >
            <View style={styles.overlay}>
                <View style={styles.modal}>
                    {/* Header */}
                    <View style={styles.header}>
                        <Text style={styles.title}>Share Location</Text>
                        <TouchableOpacity onPress={onClose} style={styles.closeButton}>
                            <Ionicons name="close" size={24} color={Colors.white} />
                        </TouchableOpacity>
                    </View>

                    {/* Content */}
                    <View style={styles.content}>
                        {isLoading ? (
                            <View style={styles.loadingContainer}>
                                <ActivityIndicator size="large" color="#7C3AED" />
                                <Text style={styles.loadingText}>Getting your location...</Text>
                            </View>
                        ) : error ? (
                            <View style={styles.errorContainer}>
                                <Ionicons name="location-outline" size={48} color={Colors.dark.textMuted} />
                                <Text style={styles.errorText}>{error}</Text>
                                {!hasPermission && (
                                    <TouchableOpacity
                                        style={styles.retryButton}
                                        onPress={() => Linking.openSettings()}
                                    >
                                        <Text style={styles.retryButtonText}>Open Settings</Text>
                                    </TouchableOpacity>
                                )}
                                <TouchableOpacity
                                    style={styles.retryButton}
                                    onPress={requestLocationPermission}
                                >
                                    <Text style={styles.retryButtonText}>Try Again</Text>
                                </TouchableOpacity>
                            </View>
                        ) : currentLocation ? (
                            <View style={styles.locationContainer}>
                                {/* Map Preview */}
                                <TouchableOpacity onPress={openInMaps} style={styles.mapContainer}>
                                    <Image
                                        source={{ uri: getMapImageUrl() }}
                                        style={styles.mapImage}
                                        resizeMode="cover"
                                    />
                                    <View style={styles.mapOverlay}>
                                        <Ionicons name="location" size={32} color="#7C3AED" />
                                    </View>
                                </TouchableOpacity>

                                {/* Location Info */}
                                <View style={styles.locationInfo}>
                                    <Ionicons name="location" size={24} color="#7C3AED" />
                                    <View style={styles.locationText}>
                                        <Text style={styles.addressText}>
                                            {currentLocation.address || 'Current Location'}
                                        </Text>
                                        <Text style={styles.coordsText}>
                                            {currentLocation.latitude.toFixed(6)}, {currentLocation.longitude.toFixed(6)}
                                        </Text>
                                    </View>
                                </View>

                                {/* Actions */}
                                <TouchableOpacity
                                    style={styles.shareButton}
                                    onPress={handleShareLocation}
                                >
                                    <Ionicons name="send" size={20} color={Colors.white} />
                                    <Text style={styles.shareButtonText}>Share This Location</Text>
                                </TouchableOpacity>

                                <TouchableOpacity
                                    style={styles.refreshButton}
                                    onPress={getCurrentLocation}
                                >
                                    <Ionicons name="refresh" size={18} color={Colors.dark.textMuted} />
                                    <Text style={styles.refreshButtonText}>Refresh Location</Text>
                                </TouchableOpacity>
                            </View>
                        ) : null}
                    </View>
                </View>
            </View>
        </Modal>
    );
}

const styles = StyleSheet.create({
    overlay: {
        flex: 1,
        backgroundColor: 'rgba(0, 0, 0, 0.7)',
        justifyContent: 'flex-end',
    },
    modal: {
        backgroundColor: Colors.dark.background,
        borderTopLeftRadius: 24,
        borderTopRightRadius: 24,
        maxHeight: '70%',
        paddingBottom: 30,
    },
    header: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        padding: 16,
        borderBottomWidth: 1,
        borderBottomColor: Colors.dark.surface,
    },
    title: {
        fontSize: 18,
        fontWeight: '700',
        color: Colors.white,
    },
    closeButton: {
        padding: 4,
    },
    content: {
        padding: 16,
    },
    loadingContainer: {
        height: 200,
        justifyContent: 'center',
        alignItems: 'center',
    },
    loadingText: {
        color: Colors.dark.textMuted,
        marginTop: 12,
    },
    errorContainer: {
        height: 200,
        justifyContent: 'center',
        alignItems: 'center',
    },
    errorText: {
        color: Colors.dark.textMuted,
        marginTop: 12,
        textAlign: 'center',
    },
    retryButton: {
        marginTop: 16,
        paddingHorizontal: 20,
        paddingVertical: 10,
        backgroundColor: Colors.dark.surface,
        borderRadius: 20,
    },
    retryButtonText: {
        color: Colors.white,
        fontWeight: '600',
    },
    locationContainer: {
        gap: 16,
    },
    mapContainer: {
        height: 180,
        borderRadius: 16,
        overflow: 'hidden',
        backgroundColor: Colors.dark.surface,
    },
    mapImage: {
        width: '100%',
        height: '100%',
    },
    mapOverlay: {
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: [{ translateX: -16 }, { translateY: -16 }],
    },
    locationInfo: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        padding: 14,
        borderRadius: 12,
        gap: 12,
    },
    locationText: {
        flex: 1,
    },
    addressText: {
        color: Colors.white,
        fontSize: 15,
        fontWeight: '600',
    },
    coordsText: {
        color: Colors.dark.textMuted,
        fontSize: 12,
        marginTop: 2,
    },
    shareButton: {
        flexDirection: 'row',
        backgroundColor: '#7C3AED',
        paddingVertical: 14,
        borderRadius: 12,
        justifyContent: 'center',
        alignItems: 'center',
        gap: 8,
    },
    shareButtonText: {
        color: Colors.white,
        fontSize: 16,
        fontWeight: '600',
    },
    refreshButton: {
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center',
        gap: 6,
        paddingVertical: 8,
    },
    refreshButtonText: {
        color: Colors.dark.textMuted,
        fontSize: 14,
    },
});
