import React, { useEffect, useRef, useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ActivityIndicator,
    Dimensions,
    TouchableOpacity,
    SafeAreaView,
    Switch,
    ScrollView,
    Alert,
} from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { useDiscoverStore } from '@/stores/discoverStore';
import SwipeCard from '@/components/SwipeCard';
import MatchPopup from '@/components/MatchPopup';
import FilterModal, { FilterState } from '@/components/FilterModal';
import EmptyState from '@/components/ui/EmptyState';
import Colors from '@/constants/Colors';

const { width: SCREEN_WIDTH } = Dimensions.get('window');

export default function DiscoverScreen() {
    const router = useRouter();
    const [isStudyBuddy, setIsStudyBuddy] = useState(false);
    const [showFilterModal, setShowFilterModal] = useState(false);
    const [hasActiveFilters, setHasActiveFilters] = useState(false);
    const [filters, setFilters] = useState<FilterState | null>(null);
    const [notifyNewStudents, setNotifyNewStudents] = useState(false);

    const {
        profiles,
        currentIndex,
        isLoading,
        error,
        newMatch,
        rewindsRemaining,
        lastSwipedProfile,
        fetchProfiles,
        swipe,
        undoSwipe,
        fetchRewindsRemaining,
        setNewMatch,
    } = useDiscoverStore();

    const cardRef = useRef<{ triggerSwipe: (direction: 'left' | 'right' | 'up') => void } | null>(null);

    useEffect(() => {
        fetchProfiles();
        fetchRewindsRemaining();
    }, []);

    const currentProfile = profiles[currentIndex];

    const handleSwipe = async (direction: 'left' | 'right' | 'up') => {
        if (!currentProfile) return;

        const isLike = direction === 'right';
        const isSuperLike = direction === 'up';

        try {
            await swipe(currentProfile.id, isLike || isSuperLike, isSuperLike);
        } catch (err: any) {
            console.error('Swipe error ignored:', err.message);
            // Do not block UI, just log it. The swipe optimistic update will handle it.
            // If we set 'setError', it shows the "Cloud Offline" screen which blocks usage.
        }
    };

    const handleAction = (action: 'pass' | 'superlike' | 'like') => {
        switch (action) {
            case 'pass':
                cardRef.current?.triggerSwipe('left');
                break;
            case 'superlike':
                cardRef.current?.triggerSwipe('up');
                break;
            case 'like':
                cardRef.current?.triggerSwipe('right');
                break;
        }
    };

    const handleApplyFilters = (newFilters: FilterState) => {
        setFilters(newFilters);
        // Check if any filters are active
        const isActive = newFilters.distance !== 25 ||
            newFilters.ageMin !== 18 ||
            newFilters.ageMax !== 30 ||
            newFilters.showMe !== 'everyone' ||
            newFilters.academicYears.length > 0 ||
            newFilters.majors.length > 0 ||
            newFilters.lookingFor.length > 0;
        setHasActiveFilters(isActive);

        // Fetch profiles with applied filters
        fetchProfiles({
            minAge: newFilters.ageMin,
            maxAge: newFilters.ageMax,
            gender: newFilters.showMe,
            academicYears: newFilters.academicYears.length > 0 ? newFilters.academicYears : undefined,
            majors: newFilters.majors.length > 0 ? newFilters.majors : undefined,
        });
    };

    if (isLoading && profiles.length === 0) {
        return (
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={styles.container}
            >
                <SafeAreaView style={styles.safeArea}>
                    <View style={styles.centerContent}>
                        <ActivityIndicator size="large" color="#7C3AED" />
                        <Text style={styles.loadingText}>Finding matches...</Text>
                    </View>
                </SafeAreaView>
            </LinearGradient>
        );
    }

    if (error) {
        return (
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={styles.container}
            >
                <SafeAreaView style={styles.safeArea}>
                    <View style={styles.centerContent}>
                        <Ionicons name="cloud-offline-outline" size={64} color={Colors.dark.textMuted} />
                        <Text style={styles.errorTitle}>Connection Issue</Text>
                        <Text style={styles.errorText}>{error}</Text>
                        <TouchableOpacity style={styles.retryButton} onPress={fetchProfiles}>
                            <LinearGradient
                                colors={['#7C3AED', '#6D28D9']}
                                style={styles.retryGradient}
                            >
                                <Text style={styles.retryText}>Try Again</Text>
                            </LinearGradient>
                        </TouchableOpacity>
                    </View>
                </SafeAreaView>
            </LinearGradient>
        );
    }

    if (!currentProfile) {
        return (
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={styles.container}
            >
                <SafeAreaView style={styles.safeArea}>
                    <EmptyState
                        icon="heart-dislike-outline"
                        title="No More Profiles"
                        description="We've run out of people to show you. Check back later or adjust your filters."
                        action={{
                            label: "Refresh",
                            onPress: () => fetchProfiles()
                        }}
                        style={styles.centerContent}
                    />

                    {/* Optional: Keep notification toggle if desired */}
                    <View style={styles.notifyContainer}>
                        <View style={styles.notifyRow}>
                            <Text style={styles.notifyLabel}>Notify me when new students join</Text>
                            <Switch
                                value={notifyNewStudents}
                                onValueChange={setNotifyNewStudents}
                                trackColor={{ false: "#767577", true: "#81b0ff" }}
                                thumbColor={notifyNewStudents ? "#660c6c" : "#f4f3f4"}
                                accessible={true}
                                accessibilityLabel="Toggle notifications for new students"
                                accessibilityRole="switch"
                            />
                        </View>
                        <Text style={styles.notifyInfo}>
                            We'll send you a push notification when we find people who match your preferences.
                        </Text>
                    </View>
                </SafeAreaView>
            </LinearGradient>
        );
    }

    return (
        <LinearGradient
            colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
            style={styles.container}
        >
            <SafeAreaView style={styles.safeArea}>
                {/* Header */}
                <View style={styles.header}>
                    <TouchableOpacity
                        style={styles.headerIcon}
                        onPress={() => setShowFilterModal(true)}
                    >
                        <Ionicons name="options-outline" size={22} color={Colors.white} />
                        {hasActiveFilters && <View style={styles.filterDot} />}
                    </TouchableOpacity>
                    <View style={styles.headerTitleContainer}>
                        <Text style={styles.headerTitle}>Discover</Text>
                    </View>
                    <TouchableOpacity
                        style={styles.headerIcon}
                        onPress={() => router.push('/(tabs)/profile')}
                        accessible={true}
                        accessibilityLabel="View profile"
                    >
                        <Ionicons name="person-outline" size={22} color={Colors.white} />
                        <View style={styles.notificationDot} />
                    </TouchableOpacity>

                </View>

                {/* Card Stack */}
                <View style={styles.cardStack}>
                    {profiles.slice(currentIndex, currentIndex + 3).reverse().map((profile, index) => (
                        <SwipeCard
                            key={profile.id}
                            ref={index === profiles.slice(currentIndex, currentIndex + 3).length - 1 ? cardRef : null}
                            profile={profile}
                            onSwipe={handleSwipe}
                            isTop={index === profiles.slice(currentIndex, currentIndex + 3).length - 1}
                        />
                    ))}
                </View>

                {/* Study Buddy Toggle */}
                <View style={styles.toggleContainer}>
                    <Text style={styles.toggleLabel}>Study Buddy?</Text>
                    <Switch
                        value={isStudyBuddy}
                        onValueChange={setIsStudyBuddy}
                        trackColor={{ false: '#3D3D5C', true: '#7C3AED' }}
                        thumbColor={Colors.white}
                    />
                </View>

                {/* Action Buttons */}
                <View style={styles.actionsContainer}>
                    {/* Pass */}
                    <TouchableOpacity
                        style={[styles.actionButton, styles.passButton]}
                        onPress={() => handleAction('pass')}
                        activeOpacity={0.8}
                    >
                        <Ionicons name="close" size={28} color="#EF4444" />
                    </TouchableOpacity>

                    {/* Rewind/Undo */}
                    <TouchableOpacity
                        style={[styles.actionButton, styles.rewindButton]}
                        onPress={async () => {
                            if (!lastSwipedProfile) {
                                Alert.alert('No Recent Swipe', 'There is no swipe to undo.');
                                return;
                            }
                            if (rewindsRemaining <= 0) {
                                Alert.alert('No Rewinds Left', 'You have used all your rewinds for today.');
                                return;
                            }
                            const success = await undoSwipe();
                            if (!success) {
                                Alert.alert('Error', 'Failed to undo swipe.');
                            }
                        }}
                        activeOpacity={0.8}
                        disabled={!lastSwipedProfile || currentIndex === 0}
                    >
                        <Ionicons
                            name="arrow-undo"
                            size={20}
                            color={(!lastSwipedProfile || currentIndex === 0 || rewindsRemaining <= 0) ? "rgba(255,215,0,0.3)" : "#FFD700"}
                        />
                    </TouchableOpacity>

                    {/* Super Like */}
                    <TouchableOpacity
                        style={[styles.actionButton, styles.superButton]}
                        onPress={() => handleAction('superlike')}
                        activeOpacity={0.8}
                    >
                        <Ionicons name="star" size={24} color="#3B82F6" />
                    </TouchableOpacity>

                    {/* Like */}
                    <TouchableOpacity
                        style={[styles.actionButton, styles.likeButton]}
                        onPress={() => handleAction('like')}
                        activeOpacity={0.8}
                    >
                        <Ionicons name="heart" size={28} color="#22C55E" />
                    </TouchableOpacity>
                </View>

                {/* Match Popup */}
                {newMatch && (
                    <MatchPopup
                        visible={true}
                        match={newMatch}
                        onClose={() => setNewMatch(null)}
                        onSendMessage={() => {
                            if (newMatch) {
                                router.push(`/chat/${newMatch.id}`);
                            }
                            setNewMatch(null);
                        }}
                    />
                )}

                {/* Filter Modal */}
                <FilterModal
                    visible={showFilterModal}
                    onClose={() => setShowFilterModal(false)}
                    onApply={handleApplyFilters}
                    initialFilters={filters || undefined}
                />
            </SafeAreaView>
        </LinearGradient >
    );
}

const styles = StyleSheet.create({
    safeArea: {
        flex: 1,
    },
    container: {
        flex: 1,
    },
    centerContent: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        padding: 24,
    },
    header: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        paddingHorizontal: 16,
        paddingVertical: 12,
    },
    headerIcon: {
        width: 40,
        height: 40,
        borderRadius: 20,
        backgroundColor: 'rgba(255,255,255,0.1)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    headerTitleContainer: {
        backgroundColor: 'rgba(255,255,255,0.1)',
        paddingHorizontal: 24,
        paddingVertical: 10,
        borderRadius: 20,
    },
    headerTitle: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    notificationDot: {
        position: 'absolute',
        top: 8,
        right: 8,
        width: 8,
        height: 8,
        borderRadius: 4,
        backgroundColor: '#EF4444',
    },
    filterDot: {
        position: 'absolute',
        top: 8,
        right: 8,
        width: 8,
        height: 8,
        borderRadius: 4,
        backgroundColor: '#F97316',
    },
    cardStack: {
        height: 500, // Fixed height so buttons show below
        alignItems: 'center',
        justifyContent: 'center',
        marginBottom: 8,
    },
    toggleContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'flex-end',
        paddingHorizontal: 24,
        paddingVertical: 8,
        gap: 12,
    },
    toggleLabel: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.7)',
    },
    actionsContainer: {
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center',
        paddingBottom: 24,
        paddingTop: 16,
        gap: 24,
        zIndex: 10,
        backgroundColor: 'transparent', // Transparent to blend with gradient
    },
    actionButton: {
        justifyContent: 'center',
        alignItems: 'center',
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.3,
        shadowRadius: 8,
        elevation: 6,
    },
    passButton: {
        width: 56,
        height: 56,
        borderRadius: 28,
        backgroundColor: Colors.dark.surface,
        borderWidth: 2,
        borderColor: '#EF4444',
    },
    superButton: {
        width: 52,
        height: 52,
        borderRadius: 16,
        backgroundColor: Colors.dark.surface,
        borderWidth: 2,
        borderColor: '#3B82F6',
    },
    likeButton: {
        width: 56,
        height: 56,
        borderRadius: 28,
        backgroundColor: Colors.dark.surface,
        borderWidth: 2,
        borderColor: '#22C55E',
    },
    rewindButton: {
        width: 44,
        height: 44,
        borderRadius: 22,
        backgroundColor: Colors.dark.surface,
        borderWidth: 2,
        borderColor: '#FFD700',
    },
    loadingText: {
        marginTop: 16,
        fontSize: 16,
        color: Colors.dark.textSecondary,
    },
    errorTitle: {
        fontSize: 24,
        fontWeight: '600',
        color: Colors.white,
        marginTop: 16,
    },
    errorText: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
        marginTop: 8,
    },
    retryButton: {
        marginTop: 24,
        borderRadius: 25,
        overflow: 'hidden',
    },
    retryGradient: {
        paddingHorizontal: 32,
        paddingVertical: 14,
    },
    retryText: {
        color: Colors.white,
        fontSize: 16,
        fontWeight: '600',
    },
    emptyIcon: {
        width: 80,
        height: 80,
        borderRadius: 40,
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 16,
    },
    emptyTitle: {
        fontSize: 24,
        fontWeight: '700',
        color: Colors.white,
    },
    emptySubtitle: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
        marginTop: 8,
        lineHeight: 22,
    },
    refreshButton: {
        marginTop: 20,
        paddingHorizontal: 24,
        paddingVertical: 12,
        borderRadius: 20,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.2)',
    },
    refreshText: {
        color: Colors.white,
        fontSize: 14,
        fontWeight: '500',
    },
    // Empty State Styles
    emptyStateContainer: {
        flexGrow: 1,
        alignItems: 'center',
        paddingHorizontal: 24,
        paddingTop: 40,
        paddingBottom: 100,
    },
    radarContainer: {
        width: 160,
        height: 160,
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 24,
    },
    radarPulse1: {
        position: 'absolute',
        width: 160,
        height: 160,
        borderRadius: 80,
        borderWidth: 2,
        borderColor: 'rgba(124, 58, 237, 0.15)',
    },
    radarPulse2: {
        position: 'absolute',
        width: 120,
        height: 120,
        borderRadius: 60,
        borderWidth: 2,
        borderColor: 'rgba(124, 58, 237, 0.25)',
    },
    radarPulse3: {
        position: 'absolute',
        width: 80,
        height: 80,
        borderRadius: 40,
        borderWidth: 2,
        borderColor: 'rgba(124, 58, 237, 0.4)',
    },
    radarCenter: {
        width: 60,
        height: 60,
        borderRadius: 30,
        backgroundColor: 'rgba(124, 58, 237, 0.3)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    radarEmoji: {
        fontSize: 28,
    },
    primaryButton: {
        width: '100%',
        marginTop: 24,
        borderRadius: 14,
        overflow: 'hidden',
    },
    primaryButtonGradient: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 16,
        gap: 10,
    },
    primaryButtonText: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    secondaryButton: {
        width: '100%',
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        marginTop: 12,
        paddingVertical: 14,
        borderRadius: 14,
        borderWidth: 1,
        borderColor: '#7C3AED',
        gap: 10,
    },
    secondaryButtonText: {
        fontSize: 15,
        fontWeight: '600',
        color: '#7C3AED',
    },
    notifySection: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        width: '100%',
        marginTop: 24,
        paddingHorizontal: 16,
        paddingVertical: 16,
        backgroundColor: Colors.dark.surface,
        borderRadius: 14,
    },
    notifyInfo: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 12,
    },
    notifyTextContainer: {
        gap: 2,
    },
    notifyTitle: {
        fontSize: 15,
        fontWeight: '600',
        color: Colors.white,
    },
    notifySubtext: {
        fontSize: 12,
        color: 'rgba(255,255,255,0.5)',
    },
    rewindSection: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        width: '100%',
        marginTop: 12,
        paddingHorizontal: 16,
        paddingVertical: 16,
        backgroundColor: Colors.dark.surface,
        borderRadius: 14,
    },
    rewindContent: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 12,
    },
    rewindIconContainer: {
        width: 40,
        height: 40,
        borderRadius: 20,
        backgroundColor: 'rgba(255, 215, 0, 0.1)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    rewindTextContainer: {
        gap: 2,
    },
    rewindTitle: {
        fontSize: 14,
        fontWeight: '600',
        color: Colors.white,
    },
    rewindSubtext: {
        fontSize: 12,
        color: 'rgba(255,255,255,0.5)',
    },
    premiumBadge: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(255, 215, 0, 0.1)',
        paddingHorizontal: 10,
        paddingVertical: 6,
        borderRadius: 12,
        gap: 4,
    },
    premiumBadgeText: {
        fontSize: 11,
        fontWeight: '600',
        color: '#FFD700',
    },
    exploreLink: {
        flexDirection: 'row',
        alignItems: 'center',
        marginTop: 28,
        gap: 6,
    },
    exploreLinkText: {
        fontSize: 15,
        color: '#7C3AED',
        fontWeight: '500',
    },
    emptyCircle: {
        width: 80,
        height: 80,
        borderRadius: 40,
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 16,
    },
    notifyContainer: {
        width: '100%',
        marginTop: 24,
        padding: 16,
        backgroundColor: Colors.dark.surface,
        borderRadius: 14,
    },
    notifyRow: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        marginBottom: 8,
    },
    notifyLabel: {
        fontSize: 15,
        fontWeight: '600',
        color: Colors.white,
        flex: 1,
    },
    refreshButtonText: {
        color: Colors.white,
        fontSize: 14,
        fontWeight: '500',
    },
    emptyText: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
        marginTop: 8,
        lineHeight: 22,
    },
});
