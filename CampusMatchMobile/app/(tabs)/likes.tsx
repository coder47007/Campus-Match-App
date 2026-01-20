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
    Alert,
} from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { likesApi } from '@/services';
import Colors from '@/constants/Colors';

const { width: SCREEN_WIDTH } = Dimensions.get('window');
const CARD_WIDTH = (SCREEN_WIDTH - 48) / 2;

// Mock data for users who liked you - attractive college students
const mockLikes = [
    { id: 1, name: 'Ben', age: 22, major: 'Business', university: 'NYU', photo: 'https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?w=400', isNew: true },
    { id: 2, name: 'Sophia', age: 21, major: 'Psychology', university: 'UCLA', photo: 'https://images.unsplash.com/photo-1524504388940-b1c1722653e1?w=400', isNew: false },
    { id: 3, name: 'Marcus', age: 23, major: 'Engineering', university: 'MIT', photo: 'https://images.unsplash.com/photo-1492562080023-ab3db95bfbce?w=400', isNew: false },
    { id: 4, name: 'Isabella', age: 20, major: 'Art History', university: 'Columbia', photo: 'https://images.unsplash.com/photo-1517841905240-472988babdf9?w=400', isNew: false },
    { id: 5, name: 'Ethan', age: 22, major: 'Computer Science', university: 'Stanford', photo: 'https://images.unsplash.com/photo-1506794778202-cad84cf45f1d?w=400', isNew: false },
    { id: 6, name: 'Olivia', age: 21, major: 'Biology', university: 'Harvard', photo: 'https://images.unsplash.com/photo-1529626455594-4ff0802cfb7e?w=400', isNew: false },
    { id: 7, name: 'Jake', age: 23, major: 'Finance', university: 'UPenn', photo: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400', isNew: false },
    { id: 8, name: 'Emma', age: 20, major: 'Communications', university: 'USC', photo: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=400', isNew: false },
];

interface LikeCard {
    id: number;
    name: string;
    age: number;
    major?: string;
    university?: string;
    photo: string;
    isNew?: boolean;
}

export default function LikesScreen() {
    const router = useRouter();
    const [isLoading, setIsLoading] = useState(false);
    const [likes, setLikes] = useState<LikeCard[]>(mockLikes);
    const [isPremium, setIsPremium] = useState(false);

    const likesCount = likes.length;

    const renderLikeCard = (like: LikeCard, index: number) => {
        const isFirst = index === 0;
        const showBlur = !isPremium && !isFirst;

        return (
            <TouchableOpacity
                key={like.id}
                style={styles.likeCard}
                onPress={() => {
                    if (!showBlur) {
                        // Navigate to profile
                        router.push(`/profile/${like.id}`);
                    }
                }}
                activeOpacity={showBlur ? 1 : 0.8}
            >
                <Image
                    source={{ uri: like.photo }}
                    style={styles.likePhoto}
                    resizeMode="cover"
                    blurRadius={showBlur ? 15 : 0}
                />

                {/* Gradient overlay */}
                <LinearGradient
                    colors={['transparent', 'rgba(0,0,0,0.6)']}
                    style={styles.cardGradient}
                />

                {isFirst && !showBlur && (
                    <>
                        {/* Heart icon for unblurred */}
                        <View style={styles.heartIcon}>
                            <Ionicons name="heart" size={16} color={Colors.white} />
                        </View>
                        {/* Name overlay */}
                        <View style={styles.nameOverlay}>
                            <Text style={styles.likeName}>{like.name}</Text>
                        </View>
                    </>
                )}

                {showBlur && (
                    <View style={styles.blurOverlay}>
                        <TouchableOpacity
                            style={styles.upgradeButton}
                            onPress={() => {
                                // TODO: Show premium subscription modal
                                Alert.alert(
                                    'Premium Feature',
                                    'Upgrade to Premium to see everyone who liked you!',
                                    [{ text: 'OK' }]
                                );
                            }}
                        >
                            <Text style={styles.upgradeText}>Upgrade to see</Text>
                        </TouchableOpacity>
                    </View>
                )}
            </TouchableOpacity>
        );
    };

    if (isLoading) {
        return (
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={styles.container}
            >
                <SafeAreaView style={styles.safeArea}>
                    <View style={styles.centerContent}>
                        <ActivityIndicator size="large" color="#7C3AED" />
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
                        onPress={() => Alert.alert('Filter', 'Filter options coming soon!')}
                    >
                        <Ionicons name="options-outline" size={22} color={Colors.white} />
                    </TouchableOpacity>
                    <View style={styles.headerTitleContainer}>
                        <Text style={styles.headerTitle}>Activity/Likes You</Text>
                    </View>
                    <TouchableOpacity
                        style={styles.headerIcon}
                        onPress={() => router.push('/profile')}
                    >
                        <Ionicons name="person-outline" size={22} color={Colors.white} />
                    </TouchableOpacity>
                </View>

                <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.content}>
                    {/* Likes Count */}
                    <Text style={styles.likesCount}>Likes You ({likesCount})</Text>

                    {/* Likes Grid */}
                    <View style={styles.likesGrid}>
                        {likes.map((like, index) => renderLikeCard(like, index))}
                    </View>

                    {/* Premium CTA */}
                    {!isPremium && (
                        <TouchableOpacity
                            style={styles.premiumCta}
                            onPress={() => {
                                // TODO: Navigate to premium subscription page
                                Alert.alert(
                                    'CampusMatch Premium',
                                    'Unlock unlimited likes, see who liked you, and more!\n\nPricing:\n• $9.99/month\n• $24.99/3 months\n• $49.99/year',
                                    [
                                        { text: 'Cancel', style: 'cancel' },
                                        { text: 'Subscribe', onPress: () => { } }
                                    ]
                                );
                            }}
                        >
                            <LinearGradient
                                colors={['#7C3AED', '#6D28D9']}
                                style={styles.premiumGradient}
                            >
                                <Ionicons name="diamond" size={20} color={Colors.white} />
                                <Text style={styles.premiumText}>See All Who Liked You</Text>
                            </LinearGradient>
                        </TouchableOpacity>
                    )}
                </ScrollView>
            </SafeAreaView>
        </LinearGradient >
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
        paddingHorizontal: 20,
        paddingVertical: 10,
        borderRadius: 20,
    },
    headerTitle: {
        fontSize: 14,
        fontWeight: '600',
        color: Colors.white,
    },
    content: {
        paddingHorizontal: 16,
        paddingBottom: 100,
    },
    likesCount: {
        fontSize: 20,
        fontWeight: '700',
        color: Colors.white,
        marginTop: 16,
        marginBottom: 16,
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
        height: '40%',
    },
    heartIcon: {
        position: 'absolute',
        bottom: 12,
        right: 12,
        width: 32,
        height: 32,
        borderRadius: 16,
        backgroundColor: 'rgba(255,255,255,0.2)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    nameOverlay: {
        position: 'absolute',
        bottom: 12,
        left: 12,
    },
    likeName: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    blurOverlay: {
        ...StyleSheet.absoluteFillObject,
        justifyContent: 'center',
        alignItems: 'center',
        backgroundColor: 'rgba(15, 10, 26, 0.3)',
    },
    upgradeButton: {
        backgroundColor: 'rgba(255,255,255,0.9)',
        paddingHorizontal: 12,
        paddingVertical: 8,
        borderRadius: 16,
    },
    upgradeText: {
        fontSize: 11,
        fontWeight: '600',
        color: '#1A1025',
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
        gap: 8,
    },
    premiumText: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
});
