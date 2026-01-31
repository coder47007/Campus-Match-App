import React, { useEffect, useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    TouchableOpacity,
    Image,
    ActivityIndicator,
    SafeAreaView,
    Dimensions,
} from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { likesApi } from '@/services';
import { useSubscriptionStore } from '@/stores/subscriptionStore';
import { PremiumPaywall } from '@/components/premium';
import Colors from '@/constants/Colors';

const { width: SCREEN_WIDTH } = Dimensions.get('window');
const CARD_WIDTH = (SCREEN_WIDTH - 48) / 2;

interface LikeCard {
    id: number;
    name: string;
    age?: number;
    major?: string;
    university?: string;
    photoUrl: string;
    isSuperLike?: boolean;
    likedAt?: string;
}

export default function LikesScreen() {
    const router = useRouter();
    const [isLoading, setIsLoading] = useState(true);
    const [likes, setLikes] = useState<LikeCard[]>([]);
    const [canSeeFullProfiles, setCanSeeFullProfiles] = useState(false);
    const [showPaywall, setShowPaywall] = useState(false);

    const { features, fetchSubscription } = useSubscriptionStore();

    useEffect(() => {
        loadLikes();
    }, []);

    const loadLikes = async () => {
        try {
            setIsLoading(true);
            const response = await likesApi.getReceivedLikes();

            setCanSeeFullProfiles(response.canSeeWhoLikedYou || false);

            // Map response to LikeCard format
            const mappedLikes = response.likes?.map((like: any) => ({
                id: like.id || like.swiperId,
                name: like.name || like.initial || '?',
                age: like.age,
                major: like.major,
                university: like.university,
                photoUrl: like.photoUrl || like.photo,
                isSuperLike: like.isSuperLike,
                likedAt: like.likedAt || like.createdAt,
            })) || [];

            setLikes(mappedLikes);
        } catch (error) {
            console.error('Failed to load likes:', error);
        } finally {
            setIsLoading(false);
        }
    };

    const handleUpgrade = () => {
        fetchSubscription();
        loadLikes(); // Reload to show full profiles
    };

    const renderLikeCard = (like: LikeCard, index: number) => {
        // Premium users: show all profiles unblurred
        // Free users: show first profile unblurred, rest blurred
        const isFirst = index === 0;
        const showBlur = !canSeeFullProfiles && !isFirst;

        return (
            <TouchableOpacity
                key={like.id}
                style={styles.likeCard}
                onPress={() => {
                    if (!showBlur) {
                        router.push(`/profile/${like.id}`);
                    } else {
                        setShowPaywall(true);
                    }
                }}
                activeOpacity={showBlur ? 0.9 : 0.8}
            >
                <Image
                    source={{ uri: like.photoUrl }}
                    style={styles.likePhoto}
                    resizeMode="cover"
                    blurRadius={showBlur ? 25 : 0}
                />

                {/* Gradient overlay */}
                <LinearGradient
                    colors={['transparent', 'rgba(0,0,0,0.7)']}
                    style={styles.cardGradient}
                />

                {/* Super Like indicator */}
                {like.isSuperLike && (
                    <View style={styles.superLikeIcon}>
                        <Ionicons name="star" size={14} color="#3B82F6" />
                    </View>
                )}

                {!showBlur && (
                    <>
                        {/* Heart icon */}
                        <View style={styles.heartIcon}>
                            <Ionicons name="heart" size={16} color={Colors.white} />
                        </View>
                        {/* Name and info */}
                        <View style={styles.nameOverlay}>
                            <Text style={styles.likeName}>
                                {like.name}{like.age ? `, ${like.age}` : ''}
                            </Text>
                            {like.university && (
                                <Text style={styles.likeUniversity}>{like.university}</Text>
                            )}
                        </View>
                    </>
                )}

                {showBlur && (
                    <View style={styles.blurOverlay}>
                        <View style={styles.lockIcon}>
                            <Ionicons name="lock-closed" size={24} color="white" />
                        </View>
                        <TouchableOpacity
                            style={styles.upgradeButton}
                            onPress={() => setShowPaywall(true)}
                        >
                            <Text style={styles.upgradeText}>Unlock</Text>
                        </TouchableOpacity>
                    </View>
                )}
            </TouchableOpacity>
        );
    };

    if (isLoading) {
        return (
            <LinearGradient
                colors={Colors.gradients.dark}
                style={styles.container}
            >
                <SafeAreaView style={styles.safeArea}>
                    <View style={styles.centerContent}>
                        <ActivityIndicator size="large" color={Colors.primary.gradient[2]} />
                        <Text style={styles.loadingText}>Loading likes...</Text>
                    </View>
                </SafeAreaView>
            </LinearGradient>
        );
    }

    return (
        <LinearGradient
            colors={Colors.gradients.dark}
            style={styles.container}
        >
            <SafeAreaView style={styles.safeArea}>
                {/* Header */}
                <View style={styles.header}>
                    <TouchableOpacity
                        style={styles.headerIcon}
                        onPress={() => router.back()}
                    >
                        <Ionicons name="chevron-back" size={22} color={Colors.white} />
                    </TouchableOpacity>
                    <View style={styles.headerTitleContainer}>
                        <Text style={styles.headerTitle}>Likes You</Text>
                    </View>
                    <View style={styles.headerIcon} />
                </View>

                <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.content}>
                    {/* Likes Count */}
                    <View style={styles.likesHeader}>
                        <Text style={styles.likesCount}>
                            {likes.length} {likes.length === 1 ? 'person' : 'people'} liked you
                        </Text>
                        {!canSeeFullProfiles && likes.length > 0 && (
                            <View style={styles.premiumHint}>
                                <Ionicons name="diamond" size={14} color="#FFD700" />
                                <Text style={styles.premiumHintText}>Upgrade to see all</Text>
                            </View>
                        )}
                    </View>

                    {/* Empty State */}
                    {likes.length === 0 && (
                        <View style={styles.emptyState}>
                            <View style={styles.emptyIcon}>
                                <Ionicons name="heart-outline" size={48} color={Colors.dark.textMuted} />
                            </View>
                            <Text style={styles.emptyTitle}>No likes yet</Text>
                            <Text style={styles.emptySubtitle}>
                                When someone likes your profile, they'll appear here
                            </Text>
                        </View>
                    )}

                    {/* Likes Grid */}
                    {likes.length > 0 && (
                        <View style={styles.likesGrid}>
                            {likes.map((like, index) => renderLikeCard(like, index))}
                        </View>
                    )}

                    {/* Premium CTA */}
                    {!canSeeFullProfiles && likes.length > 1 && (
                        <TouchableOpacity
                            style={styles.premiumCta}
                            onPress={() => setShowPaywall(true)}
                        >
                            <LinearGradient
                                colors={['#FF6B6B', '#FF8E53']}
                                start={{ x: 0, y: 0 }}
                                end={{ x: 1, y: 0 }}
                                style={styles.premiumGradient}
                            >
                                <Ionicons name="diamond" size={20} color={Colors.white} />
                                <Text style={styles.premiumText}>See All Who Liked You</Text>
                                <Ionicons name="arrow-forward" size={18} color={Colors.white} />
                            </LinearGradient>
                        </TouchableOpacity>
                    )}
                </ScrollView>

                {/* Premium Paywall */}
                <PremiumPaywall
                    visible={showPaywall}
                    onClose={() => setShowPaywall(false)}
                    feature="likes"
                    onUpgrade={handleUpgrade}
                />
            </SafeAreaView>
        </LinearGradient>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    safeArea: {
        flex: 1,
    },
    centerContent: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    loadingText: {
        marginTop: 12,
        fontSize: 14,
        color: Colors.dark.textSecondary,
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
    content: {
        paddingHorizontal: 16,
        paddingBottom: 100,
    },
    likesHeader: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginTop: 16,
        marginBottom: 16,
    },
    likesCount: {
        fontSize: 20,
        fontWeight: '700',
        color: Colors.white,
    },
    premiumHint: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 4,
    },
    premiumHintText: {
        fontSize: 12,
        color: '#FFD700',
        fontWeight: '500',
    },
    likesGrid: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 12,
    },
    likeCard: {
        width: CARD_WIDTH,
        height: CARD_WIDTH * 1.3,
        borderRadius: 16,
        overflow: 'hidden',
        backgroundColor: Colors.dark.surface,
    },
    likePhoto: {
        width: '100%',
        height: '100%',
    },
    cardGradient: {
        position: 'absolute',
        bottom: 0,
        left: 0,
        right: 0,
        height: '50%',
    },
    superLikeIcon: {
        position: 'absolute',
        top: 12,
        right: 12,
        width: 28,
        height: 28,
        borderRadius: 14,
        backgroundColor: 'rgba(59, 130, 246, 0.2)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    heartIcon: {
        position: 'absolute',
        bottom: 12,
        right: 12,
        width: 32,
        height: 32,
        borderRadius: 16,
        backgroundColor: 'rgba(239, 68, 68, 0.8)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    nameOverlay: {
        position: 'absolute',
        bottom: 12,
        left: 12,
        right: 48,
    },
    likeName: {
        fontSize: 16,
        fontWeight: '700',
        color: Colors.white,
    },
    likeUniversity: {
        fontSize: 12,
        color: 'rgba(255,255,255,0.8)',
        marginTop: 2,
    },
    blurOverlay: {
        ...StyleSheet.absoluteFillObject,
        justifyContent: 'center',
        alignItems: 'center',
    },
    lockIcon: {
        width: 48,
        height: 48,
        borderRadius: 24,
        backgroundColor: 'rgba(0,0,0,0.4)',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 8,
    },
    upgradeButton: {
        backgroundColor: 'rgba(255,255,255,0.95)',
        paddingHorizontal: 16,
        paddingVertical: 8,
        borderRadius: 20,
    },
    upgradeText: {
        fontSize: 13,
        fontWeight: '600',
        color: '#FF6B6B',
    },
    emptyState: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        paddingVertical: 60,
    },
    emptyIcon: {
        width: 80,
        height: 80,
        borderRadius: 40,
        backgroundColor: 'rgba(255,255,255,0.1)',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 16,
    },
    emptyTitle: {
        fontSize: 20,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 8,
    },
    emptySubtitle: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
        maxWidth: 280,
    },
    premiumCta: {
        marginTop: 24,
        borderRadius: 16,
        overflow: 'hidden',
    },
    premiumGradient: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        paddingVertical: 16,
        gap: 10,
    },
    premiumText: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
});
