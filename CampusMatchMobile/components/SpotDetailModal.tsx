import React, { useState, useEffect } from 'react';
import {
    View,
    Text,
    StyleSheet,
    Modal,
    TouchableOpacity,
    Image,
    ScrollView,
    ActivityIndicator,
    Alert,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { spotsApi, TrendingSpot, WhosThere } from '@/services/spots';
import Colors from '@/constants/Colors';

interface SpotDetailModalProps {
    visible: boolean;
    spot: TrendingSpot | null;
    onClose: () => void;
    onCheckInChange?: () => void;
}

export default function SpotDetailModal({
    visible,
    spot,
    onClose,
    onCheckInChange
}: SpotDetailModalProps) {
    const [isCheckedIn, setIsCheckedIn] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [whosThere, setWhosThere] = useState<WhosThere[]>([]);
    const [totalCount, setTotalCount] = useState(0);

    useEffect(() => {
        if (visible && spot) {
            setIsCheckedIn(spot.isCheckedIn);
            loadWhosThere();
            // Auto-refresh every 30 seconds while modal is open
            const interval = setInterval(loadWhosThere, 30000);
            return () => clearInterval(interval);
        }
    }, [visible, spot]);

    const loadWhosThere = async () => {
        if (!spot) return;

        try {
            const [matches, count] = await Promise.all([
                spotsApi.getWhosThere(spot.id),
                spotsApi.getTotalCount(spot.id)
            ]);
            setWhosThere(matches);
            setTotalCount(count);
        } catch (error) {
            console.error('Error loading who\'s there:', error);
        }
    };

    const handleCheckIn = async () => {
        if (!spot) return;

        setIsLoading(true);
        try {
            if (isCheckedIn) {
                const success = await spotsApi.checkOut(spot.id);
                if (success) {
                    setIsCheckedIn(false);
                    Alert.alert('Checked Out', `You've checked out from ${spot.name}`);
                    loadWhosThere();
                    onCheckInChange?.();
                }
            } else {
                const success = await spotsApi.checkIn(spot.id);
                if (success) {
                    setIsCheckedIn(true);
                    Alert.alert('Checked In!', `You're now at ${spot.name}. Your matches can see you here for the next 3 hours.`);
                    loadWhosThere();
                    onCheckInChange?.();
                }
            }
        } catch (error) {
            console.error('Check-in error:', error);
            Alert.alert('Error', 'Failed to update check-in status');
        } finally {
            setIsLoading(false);
        }
    };

    if (!spot) return null;

    return (
        <Modal
            visible={visible}
            animationType="slide"
            transparent
            onRequestClose={onClose}
        >
            <View style={styles.overlay}>
                <View style={styles.modalContainer}>
                    {/* Header Image */}
                    <Image
                        source={{ uri: spot.imageUrl }}
                        style={styles.headerImage}
                        resizeMode="cover"
                    />
                    <LinearGradient
                        colors={['transparent', 'rgba(0,0,0,0.9)']}
                        style={styles.imageGradient}
                    />

                    {/* Close Button */}
                    <TouchableOpacity style={styles.closeButton} onPress={onClose}>
                        <Ionicons name="close" size={28} color={Colors.white} />
                    </TouchableOpacity>

                    {/* Content */}
                    <ScrollView style={styles.content} showsVerticalScrollIndicator={false}>
                        {/* Spot Info */}
                        <View style={styles.spotInfo}>
                            <Text style={styles.spotName}>{spot.name}</Text>
                            <Text style={styles.spotType}>{spot.type}</Text>
                            {spot.campus && (
                                <View style={styles.campusBadge}>
                                    <Ionicons name="location" size={14} color="#7C3AED" />
                                    <Text style={styles.campusText}>{spot.campus}</Text>
                                </View>
                            )}
                        </View>

                        {/* Check-In Button */}
                        <TouchableOpacity
                            style={styles.checkInButton}
                            onPress={handleCheckIn}
                            disabled={isLoading}
                        >
                            <LinearGradient
                                colors={isCheckedIn ? ['#EF4444', '#DC2626'] : ['#7C3AED', '#6D28D9']}
                                style={styles.checkInGradient}
                            >
                                {isLoading ? (
                                    <ActivityIndicator color={Colors.white} />
                                ) : (
                                    <>
                                        <Ionicons
                                            name={isCheckedIn ? 'log-out-outline' : 'log-in-outline'}
                                            size={20}
                                            color={Colors.white}
                                        />
                                        <Text style={styles.checkInText}>
                                            {isCheckedIn ? 'Check Out' : 'Check In Here'}
                                        </Text>
                                    </>
                                )}
                            </LinearGradient>
                        </TouchableOpacity>

                        {/* Who's There Section */}
                        <View style={styles.whoSection}>
                            <View style={styles.whoHeader}>
                                <Text style={styles.whoTitle}>Who's There</Text>
                                <View style={styles.countBadge}>
                                    <Text style={styles.countText}>{totalCount} {totalCount === 1 ? 'student' : 'students'}</Text>
                                </View>
                            </View>

                            {whosThere.length > 0 ? (
                                <View style={styles.matchesList}>
                                    {whosThere.map((person) => (
                                        <View key={person.id} style={styles.matchCard}>
                                            <Image
                                                source={{ uri: person.photoUrl || 'https://randomuser.me/api/portraits/lego/1.jpg' }}
                                                style={styles.matchPhoto}
                                            />
                                            <View style={styles.matchInfo}>
                                                <Text style={styles.matchName}>{person.name}</Text>
                                                {person.major && (
                                                    <Text style={styles.matchMajor}>{person.major}</Text>
                                                )}
                                            </View>
                                            <Ionicons name="heart" size={16} color="#EF4444" />
                                        </View>
                                    ))}
                                </View>
                            ) : (
                                <View style={styles.emptyState}>
                                    <Ionicons name="people-outline" size={48} color="rgba(255,255,255,0.3)" />
                                    <Text style={styles.emptyText}>
                                        {totalCount > 0
                                            ? 'None of your matches are here yet'
                                            : 'No one is checked in right now'}
                                    </Text>
                                    <Text style={styles.emptyHint}>
                                        Check in to let your matches know you're here!
                                    </Text>
                                </View>
                            )}
                        </View>

                        {isCheckedIn && (
                            <View style={styles.infoBox}>
                                <Ionicons name="time-outline" size={16} color="#7C3AED" />
                                <Text style={styles.infoText}>
                                    You'll be checked in for 3 hours
                                </Text>
                            </View>
                        )}

                        <View style={{ height: 40 }} />
                    </ScrollView>
                </View>
            </View>
        </Modal>
    );
}

const styles = StyleSheet.create({
    overlay: {
        flex: 1,
        backgroundColor: 'rgba(0,0,0,0.85)',
        justifyContent: 'flex-end',
    },
    modalContainer: {
        backgroundColor: Colors.dark.background,
        borderTopLeftRadius: 24,
        borderTopRightRadius: 24,
        height: '95%',
        overflow: 'hidden',
    },
    headerImage: {
        width: '100%',
        height: 250,
    },
    imageGradient: {
        position: 'absolute',
        top: 0,
        left: 0,
        right: 0,
        height: 250,
    },
    closeButton: {
        position: 'absolute',
        top: 16,
        right: 16,
        width: 40,
        height: 40,
        borderRadius: 20,
        backgroundColor: 'rgba(0,0,0,0.5)',
        justifyContent: 'center',
        alignItems: 'center',
        zIndex: 10,
    },
    content: {
        flex: 1,
    },
    spotInfo: {
        padding: 20,
        paddingTop: 0,
        marginTop: -60,
    },
    spotName: {
        fontSize: 28,
        fontWeight: '700',
        color: Colors.white,
        marginBottom: 4,
    },
    spotType: {
        fontSize: 16,
        color: 'rgba(255,255,255,0.6)',
        marginBottom: 12,
    },
    campusBadge: {
        flexDirection: 'row',
        alignItems: 'center',
        alignSelf: 'flex-start',
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        paddingHorizontal: 12,
        paddingVertical: 6,
        borderRadius: 16,
        gap: 4,
    },
    campusText: {
        fontSize: 12,
        color: '#7C3AED',
        fontWeight: '600',
    },
    checkInButton: {
        marginHorizontal: 20,
        marginBottom: 24,
        borderRadius: 20,
        overflow: 'hidden',
        elevation: 5,
        shadowColor: '#7C3AED',
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.3,
        shadowRadius: 8,
    },
    checkInGradient: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 20,
        gap: 10,
    },
    checkInText: {
        fontSize: 18,
        fontWeight: '700',
        color: Colors.white,
        letterSpacing: 0.5,
    },
    whoSection: {
        paddingHorizontal: 20,
    },
    whoHeader: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginBottom: 16,
    },
    whoTitle: {
        fontSize: 18,
        fontWeight: '700',
        color: Colors.white,
    },
    countBadge: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        paddingHorizontal: 12,
        paddingVertical: 6,
        borderRadius: 12,
    },
    countText: {
        fontSize: 12,
        color: '#7C3AED',
        fontWeight: '600',
    },
    matchesList: {
        gap: 12,
    },
    matchCard: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        padding: 12,
        borderRadius: 16,
        gap: 12,
    },
    matchPhoto: {
        width: 50,
        height: 50,
        borderRadius: 25,
    },
    matchInfo: {
        flex: 1,
    },
    matchName: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    matchMajor: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.6)',
        marginTop: 2,
    },
    emptyState: {
        alignItems: 'center',
        paddingVertical: 40,
    },
    emptyText: {
        fontSize: 16,
        color: 'rgba(255,255,255,0.6)',
        marginTop: 12,
        textAlign: 'center',
    },
    emptyHint: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.4)',
        marginTop: 8,
        textAlign: 'center',
    },
    infoBox: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(124, 58, 237, 0.1)',
        marginHorizontal: 20,
        marginTop: 16,
        padding: 12,
        borderRadius: 12,
        gap: 8,
    },
    infoText: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.8)',
    },
});
