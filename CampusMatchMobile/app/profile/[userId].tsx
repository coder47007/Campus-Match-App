import React, { useEffect, useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    Image,
    TouchableOpacity,
    Dimensions,
    ActivityIndicator,
    Alert,
} from 'react-native';
import { useLocalSearchParams, useRouter, Stack } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { profilesApi, reportsApi } from '@/services';
import { useDiscoverStore } from '@/stores/discoverStore';
import { useAuthStore } from '@/stores/authStore';
import MatchPopup from '@/components/MatchPopup';
import { StudentDto } from '@/types';
import Colors from '@/constants/Colors';

const { width: SCREEN_WIDTH } = Dimensions.get('window');

export default function ViewProfileScreen() {
    const { userId } = useLocalSearchParams<{ userId: string }>();
    const router = useRouter();
    const { swipe, profiles, currentIndex, setNewMatch, newMatch } = useDiscoverStore();
    const { user: currentUser } = useAuthStore();

    const [profile, setProfile] = useState<StudentDto | null>(null);
    const [isLoading, setIsLoading] = useState(true);
    const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);

    useEffect(() => {
        fetchProfile();
    }, [userId]);

    const fetchProfile = async () => {
        try {
            const data = await profilesApi.getProfile(parseInt(userId || '0'));
            setProfile(data);
        } catch (err) {
            Alert.alert('Error', 'Failed to load profile');
            router.back();
        } finally {
            setIsLoading(false);
        }
    };

    const handleReport = () => {
        Alert.alert(
            'Report User',
            'Why are you reporting this user?',
            [
                { text: 'Cancel', style: 'cancel' },
                { text: 'Inappropriate content', onPress: () => submitReport('Inappropriate content') },
                { text: 'Spam', onPress: () => submitReport('Spam') },
                { text: 'Harassment', onPress: () => submitReport('Harassment') },
                { text: 'Fake profile', onPress: () => submitReport('Fake profile') },
            ]
        );
    };

    const submitReport = async (reason: string) => {
        try {
            await reportsApi.reportUser({
                reportedId: parseInt(userId || '0'),
                reason,
                source: 'discover',
            });
            Alert.alert('Report Submitted', 'Thank you for helping keep our community safe.');
        } catch (err) {
            Alert.alert('Error', 'Failed to submit report');
        }
    };

    const handleBlock = () => {
        Alert.alert(
            'Block User',
            'Are you sure you want to block this user? They will no longer be able to see your profile or message you.',
            [
                { text: 'Cancel', style: 'cancel' },
                {
                    text: 'Block',
                    style: 'destructive',
                    onPress: async () => {
                        try {
                            await reportsApi.blockUser(parseInt(userId || '0'));
                            Alert.alert('User Blocked', 'This user has been blocked.');
                            router.back();
                        } catch (err) {
                            Alert.alert('Error', 'Failed to block user');
                        }
                    },
                },
            ]
        );
    };

    const handleSwipeAction = async (action: 'pass' | 'superlike' | 'like') => {
        if (!profile) return;

        const isLike = action === 'like';
        const isSuperLike = action === 'superlike';
        const currentDiscoverProfile = profiles[currentIndex];

        // Check if we are viewing the current discover profile
        // If so, use the store swipe to update the feed state
        if (currentDiscoverProfile && currentDiscoverProfile.id === profile.id) {
            try {
                await swipe(profile.id, isLike, isSuperLike);
                router.back();
            } catch (error) {
                // Error handled in store
            }
        } else {
            // We are viewing a profile not in current feed context (e.g. from Likes tab)
            // Just perform the swipe API call?
            // For now, let's just go back, or handle specific logic.
            // But the user request is likely for the Discover flow.
            // If we are deep linking, we might not want to disturb the discover queue blindly.
            // We'll proceed with the store swipe if IDs match, otherwise just log or simple alert for now.
            Alert.alert('Notice', 'You can only swipe on the active card in your feed.');
        }
    };

    const photos = profile?.photos?.length
        ? profile.photos.sort((a, b) => a.displayOrder - b.displayOrder).map(p => p.url)
        : profile?.photoUrl
            ? [profile.photoUrl]
            : [];

    if (isLoading) {
        return (
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={[styles.container, styles.centerContent]}
            >
                <ActivityIndicator size="large" color={Colors.primary.main} />
            </LinearGradient>
        );
    }

    if (!profile) {
        return (
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={[styles.container, styles.centerContent]}
            >
                <Text style={styles.errorText}>Profile not found</Text>
            </LinearGradient>
        );
    }

    return (
        <>
            <Stack.Screen
                options={{
                    title: profile.name,
                    headerRight: () => (
                        <TouchableOpacity onPress={handleReport}>
                            <Ionicons name="flag-outline" size={24} color={Colors.dark.text} />
                        </TouchableOpacity>
                    ),
                }}
            />
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={styles.container}
            >
                <ScrollView style={styles.scrollTop} showsVerticalScrollIndicator={false}>
                    {/* Photo Carousel */}
                    <View style={styles.photoSection}>
                        {photos.length > 0 ? (
                            <>
                                <Image
                                    source={{ uri: photos[currentPhotoIndex] }}
                                    style={styles.mainPhoto}
                                    resizeMode="cover"
                                />
                                {photos.length > 1 && (
                                    <>
                                        <View style={styles.photoIndicators}>
                                            {photos.map((_, idx) => (
                                                <View
                                                    key={idx}
                                                    style={[
                                                        styles.photoIndicator,
                                                        idx === currentPhotoIndex && styles.photoIndicatorActive,
                                                    ]}
                                                />
                                            ))}
                                        </View>
                                        <TouchableOpacity
                                            style={[styles.photoNavButton, styles.photoNavLeft]}
                                            onPress={() => currentPhotoIndex > 0 && setCurrentPhotoIndex(currentPhotoIndex - 1)}
                                        />
                                        <TouchableOpacity
                                            style={[styles.photoNavButton, styles.photoNavRight]}
                                            onPress={() => currentPhotoIndex < photos.length - 1 && setCurrentPhotoIndex(currentPhotoIndex + 1)}
                                        />
                                    </>
                                )}
                                <LinearGradient
                                    colors={['transparent', 'rgba(0,0,0,0.7)']}
                                    style={styles.photoGradient}
                                />
                            </>
                        ) : (
                            <View style={styles.noPhotoContainer}>
                                <Ionicons name="person" size={80} color={Colors.dark.textMuted} />
                            </View>
                        )}
                    </View>

                    {/* Basic Info */}
                    <View style={styles.infoSection}>
                        <View style={styles.nameRow}>
                            <Text style={styles.name}>{profile.name}</Text>
                            {profile.age && <Text style={styles.age}>{profile.age}</Text>}
                        </View>

                        {(profile.major || profile.year) && (
                            <View style={styles.infoRow}>
                                <Ionicons name="school-outline" size={18} color={Colors.dark.textSecondary} />
                                <Text style={styles.infoText}>
                                    {[profile.major, profile.year].filter(Boolean).join(' • ')}
                                </Text>
                            </View>
                        )}

                        {profile.university && (
                            <View style={styles.infoRow}>
                                <Ionicons name="location-outline" size={18} color={Colors.dark.textSecondary} />
                                <Text style={styles.infoText}>{profile.university}</Text>
                            </View>
                        )}
                    </View>

                    {/* Bio */}
                    {profile.bio && (
                        <View style={styles.section}>
                            <Text style={styles.sectionTitle}>About</Text>
                            <Text style={styles.bioText}>{profile.bio}</Text>
                        </View>
                    )}

                    {/* Prompts */}
                    {profile.prompts && profile.prompts.length > 0 && (
                        <View style={styles.section}>
                            <Text style={styles.sectionTitle}>Prompts</Text>
                            {profile.prompts.map((prompt) => (
                                <View key={prompt.id} style={styles.promptCard}>
                                    <Text style={styles.promptQuestion}>{prompt.question}</Text>
                                    <Text style={styles.promptAnswer}>{prompt.answer}</Text>
                                </View>
                            ))}
                        </View>
                    )}

                    {/* Interests */}
                    {profile.interests && profile.interests.length > 0 && (
                        <View style={styles.section}>
                            <Text style={styles.sectionTitle}>Interests</Text>
                            <View style={styles.interestsGrid}>
                                {profile.interests.map((interest) => (
                                    <View key={interest.id} style={styles.interestChip}>
                                        <Text style={styles.interestEmoji}>{interest.emoji}</Text>
                                        <Text style={styles.interestText}>{interest.name}</Text>
                                    </View>
                                ))}
                            </View>
                        </View>
                    )}

                    {/* Actions - Only show if not viewing own profile */}
                    {currentUser && profile && currentUser.id !== profile.id && (
                        <View style={styles.actionsSection}>
                            <TouchableOpacity style={styles.blockButton} onPress={handleBlock}>
                                <Ionicons name="ban-outline" size={20} color={Colors.error} />
                                <Text style={styles.blockButtonText}>Block User</Text>
                            </TouchableOpacity>
                        </View>
                    )}



                    <View style={styles.bottomPadding} />
                </ScrollView>

                {/* Floating Swipe Actions (Fixed at bottom) */}
                <View style={styles.floatingActions}>
                    {/* Pass */}
                    <TouchableOpacity
                        style={[styles.actionButton, styles.passButton]}
                        onPress={() => handleSwipeAction('pass')}
                        activeOpacity={0.8}
                    >
                        <Ionicons name="close" size={30} color="#EF4444" />
                    </TouchableOpacity>

                    {/* Super Like */}
                    <TouchableOpacity
                        style={[styles.actionButton, styles.superButton]}
                        onPress={() => handleSwipeAction('superlike')}
                        activeOpacity={0.8}
                    >
                        <Ionicons name="star" size={22} color="#3B82F6" />
                    </TouchableOpacity>

                    {/* Like */}
                    <TouchableOpacity
                        style={[styles.actionButton, styles.likeButton]}
                        onPress={() => handleSwipeAction('like')}
                        activeOpacity={0.8}
                    >
                        <Ionicons name="heart" size={30} color="#22C55E" />
                    </TouchableOpacity>
                </View>

                {/* Match Popup handling in case match happens here */}
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
            </LinearGradient>
        </>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    scrollTop: {
        flex: 1,
    },
    centerContent: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    photoSection: {
        width: SCREEN_WIDTH,
        height: SCREEN_WIDTH * 1.2,
        position: 'relative',
    },
    mainPhoto: {
        width: '100%',
        height: '100%',
    },
    noPhotoContainer: {
        width: '100%',
        height: '100%',
        backgroundColor: Colors.dark.surface,
        justifyContent: 'center',
        alignItems: 'center',
    },
    photoIndicators: {
        position: 'absolute',
        top: 12,
        left: 12,
        right: 12,
        flexDirection: 'row',
        gap: 4,
    },
    photoIndicator: {
        flex: 1,
        height: 3,
        backgroundColor: 'rgba(255,255,255,0.4)',
        borderRadius: 2,
    },
    photoIndicatorActive: {
        backgroundColor: Colors.white,
    },
    photoNavButton: {
        position: 'absolute',
        top: 0,
        bottom: 0,
        width: '50%',
    },
    photoNavLeft: {
        left: 0,
    },
    photoNavRight: {
        right: 0,
    },
    photoGradient: {
        position: 'absolute',
        bottom: 0,
        left: 0,
        right: 0,
        height: 100,
    },
    infoSection: {
        padding: 20,
        marginTop: -40,
    },
    nameRow: {
        flexDirection: 'row',
        alignItems: 'baseline',
        gap: 10,
    },
    name: {
        fontSize: 32,
        fontWeight: 'bold',
        color: Colors.dark.text,
    },
    age: {
        fontSize: 28,
        color: Colors.dark.text,
    },
    infoRow: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 8,
        marginTop: 12,
    },
    infoText: {
        fontSize: 16,
        color: Colors.dark.textSecondary,
    },
    section: {
        padding: 20,
        paddingTop: 0,
    },
    sectionTitle: {
        fontSize: 18,
        fontWeight: '600',
        color: Colors.dark.text,
        marginBottom: 12,
    },
    bioText: {
        fontSize: 16,
        color: Colors.dark.textSecondary,
        lineHeight: 24,
    },
    promptCard: {
        backgroundColor: Colors.dark.surface,
        borderRadius: 16,
        padding: 16,
        marginBottom: 12,
    },
    promptQuestion: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
        marginBottom: 8,
    },
    promptAnswer: {
        fontSize: 16,
        color: Colors.dark.text,
        lineHeight: 22,
    },
    interestsGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 10,
    },
    interestChip: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        paddingHorizontal: 14,
        paddingVertical: 10,
        borderRadius: 25,
        gap: 6,
    },
    interestEmoji: {
        fontSize: 16,
    },
    interestText: {
        fontSize: 14,
        color: Colors.dark.text,
    },
    actionsSection: {
        padding: 20,
    },
    blockButton: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'rgba(255, 59, 48, 0.1)',
        paddingVertical: 14,
        borderRadius: 12,
        gap: 8,
    },
    blockButtonText: {
        color: Colors.error,
        fontSize: 16,
        fontWeight: '600',
    },
    bottomPadding: {
        height: 120,
    },
    errorText: {
        color: Colors.dark.textSecondary,
        fontSize: 16,
    },
    floatingActions: {
        position: 'absolute',
        bottom: 30,
        left: 0,
        right: 0,
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center',
        gap: 20,
        paddingVertical: 10,
        zIndex: 100,
    },
    actionButton: {
        justifyContent: 'center',
        alignItems: 'center',
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.3,
        shadowRadius: 8,
        elevation: 6,
        backgroundColor: Colors.dark.surface,
    },
    passButton: {
        width: 60,
        height: 60,
        borderRadius: 30,
        borderWidth: 2,
        borderColor: '#EF4444',
    },
    superButton: {
        width: 44,
        height: 44,
        borderRadius: 22,
        borderWidth: 2,
        borderColor: '#3B82F6',
        marginTop: 0,
    },
    likeButton: {
        width: 60,
        height: 60,
        borderRadius: 30,
        borderWidth: 2,
        borderColor: '#22C55E',
    },
});
