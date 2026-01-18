import React, { forwardRef, useImperativeHandle, useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    Image,
    Dimensions,
    TouchableOpacity,
} from 'react-native';
import { Gesture, GestureDetector } from 'react-native-gesture-handler';
import Animated, {
    useSharedValue,
    useAnimatedStyle,
    withSpring,
    withTiming,
    runOnJS,
    interpolate,
    Extrapolation,
} from 'react-native-reanimated';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import * as Haptics from 'expo-haptics';
import { StudentDto } from '@/types';
import Colors from '@/constants/Colors';


const { width: SCREEN_WIDTH, height: SCREEN_HEIGHT } = Dimensions.get('window');
const CARD_WIDTH = SCREEN_WIDTH - 32;
const CARD_HEIGHT = SCREEN_HEIGHT * 0.55;
const SWIPE_THRESHOLD = SCREEN_WIDTH * 0.3;

// Default campus vibes if profile has no interests
const defaultVibes = ['Student Life', 'Campus Explorer', 'Making Friends'];
const moodOptions = [
    { emoji: '📚', text: 'Midterms... send help' },
    { emoji: '☕', text: 'Coffee is life' },
    { emoji: '🎉', text: 'Ready to party!' },
    { emoji: '💪', text: 'Gym then study' },
    { emoji: '🎮', text: 'Gaming night' },
];

interface SwipeCardProps {
    profile: StudentDto;
    onSwipe: (direction: 'left' | 'right' | 'up') => void;
    isTop?: boolean;
}

export interface SwipeCardRef {
    triggerSwipe: (direction: 'left' | 'right' | 'up') => void;
}

const SwipeCard = forwardRef<SwipeCardRef, SwipeCardProps>(
    ({ profile, onSwipe, isTop = false }, ref) => {
        const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);
        const translateX = useSharedValue(0);
        const translateY = useSharedValue(0);
        const rotate = useSharedValue(0);
        const scale = useSharedValue(isTop ? 1 : 0.95);

        const photos = profile.photos?.length
            ? profile.photos.sort((a, b) => a.displayOrder - b.displayOrder).map(p => p.url)
            : profile.photoUrl
                ? [profile.photoUrl]
                : [];

        // Campus vibes from profile interests or defaults
        const campusVibes = profile.interests?.length
            ? profile.interests.slice(0, 3).map(i => i.name)
            : defaultVibes;

        // Random mood based on profile id
        const currentMood = moodOptions[(profile.id || 0) % moodOptions.length];

        const triggerSwipe = (direction: 'left' | 'right' | 'up') => {
            // Haptic feedback based on swipe type
            if (direction === 'up') {
                // Super like - heavy impact
                Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Heavy);
            } else if (direction === 'right') {
                // Like - medium impact
                Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Medium);
            } else {
                // Pass/Nope - light impact
                Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
            }

            const targetX = direction === 'left' ? -SCREEN_WIDTH * 1.5 : direction === 'right' ? SCREEN_WIDTH * 1.5 : 0;
            const targetY = direction === 'up' ? -SCREEN_HEIGHT : 0;
            const targetRotation = direction === 'left' ? -30 : direction === 'right' ? 30 : 0;

            translateX.value = withTiming(targetX, { duration: 350 });
            translateY.value = withTiming(targetY, { duration: 350 });
            rotate.value = withTiming(targetRotation, { duration: 350 });

            setTimeout(() => {
                onSwipe(direction);
            }, 300);
        };


        useImperativeHandle(ref, () => ({
            triggerSwipe,
        }));

        const gesture = Gesture.Pan()
            .enabled(isTop)
            .onUpdate((event) => {
                translateX.value = event.translationX;
                translateY.value = event.translationY;
                rotate.value = event.translationX / 20;
            })
            .onEnd((event) => {
                if (event.translationX > SWIPE_THRESHOLD) {
                    runOnJS(triggerSwipe)('right');
                } else if (event.translationX < -SWIPE_THRESHOLD) {
                    runOnJS(triggerSwipe)('left');
                } else if (event.translationY < -SWIPE_THRESHOLD * 0.8) {
                    runOnJS(triggerSwipe)('up');
                } else {
                    translateX.value = withSpring(0);
                    translateY.value = withSpring(0);
                    rotate.value = withSpring(0);
                }
            });

        const cardStyle = useAnimatedStyle(() => ({
            transform: [
                { translateX: translateX.value },
                { translateY: translateY.value },
                { rotate: `${rotate.value}deg` },
                { scale: scale.value },
            ],
        }));

        const likeOpacity = useAnimatedStyle(() => ({
            opacity: interpolate(
                translateX.value,
                [0, SWIPE_THRESHOLD / 2],
                [0, 1],
                Extrapolation.CLAMP
            ),
        }));

        const nopeOpacity = useAnimatedStyle(() => ({
            opacity: interpolate(
                translateX.value,
                [-SWIPE_THRESHOLD / 2, 0],
                [1, 0],
                Extrapolation.CLAMP
            ),
        }));

        const handlePhotoTap = (side: 'left' | 'right') => {
            if (side === 'left' && currentPhotoIndex > 0) {
                setCurrentPhotoIndex(currentPhotoIndex - 1);
            } else if (side === 'right' && currentPhotoIndex < photos.length - 1) {
                setCurrentPhotoIndex(currentPhotoIndex + 1);
            }
        };

        return (
            <GestureDetector gesture={gesture}>
                <Animated.View style={[styles.card, cardStyle]}>
                    {/* Photo */}
                    <View style={styles.photoContainer}>
                        {photos.length > 0 ? (
                            <Image
                                source={{ uri: photos[currentPhotoIndex] }}
                                style={styles.photo}
                                resizeMode="cover"
                            />
                        ) : (
                            <View style={styles.noPhoto}>
                                <Ionicons name="person" size={100} color={Colors.dark.textMuted} />
                            </View>
                        )}

                        {/* Photo Indicators */}
                        {photos.length > 1 && (
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
                        )}

                        {/* Photo Navigation */}
                        <TouchableOpacity
                            style={[styles.photoNav, styles.photoNavLeft]}
                            onPress={() => handlePhotoTap('left')}
                        />
                        <TouchableOpacity
                            style={[styles.photoNav, styles.photoNavRight]}
                            onPress={() => handlePhotoTap('right')}
                        />

                        {/* Swipe Indicators */}
                        <Animated.View style={[styles.swipeIndicator, styles.likeIndicator, likeOpacity]}>
                            <Text style={styles.likeText}>LIKE</Text>
                        </Animated.View>

                        <Animated.View style={[styles.swipeIndicator, styles.nopeIndicator, nopeOpacity]}>
                            <Text style={styles.nopeText}>NOPE</Text>
                        </Animated.View>

                        {/* Gradient Overlay */}
                        <LinearGradient
                            colors={['transparent', 'rgba(15, 10, 26, 0.95)']}
                            style={styles.cardGradient}
                        />
                    </View>

                    {/* Info Section - Glassmorphism */}
                    <View style={styles.infoSection}>
                        <View style={styles.nameRow}>
                            <Text style={styles.name}>{profile.name}</Text>
                            {profile.age && <Text style={styles.age}>, {profile.age}</Text>}
                            <View style={styles.verifiedBadge}>
                                <Ionicons name="checkmark-circle" size={20} color="#3B82F6" />
                            </View>
                        </View>

                        <View style={styles.majorRow}>
                            <Ionicons name="school" size={14} color="rgba(255,255,255,0.7)" />
                            <Text style={styles.majorText}>
                                {profile.major || 'Student'}, {profile.university || 'University'}
                            </Text>
                        </View>

                        {/* Campus Vibes */}
                        <View style={styles.vibesSection}>
                            <Text style={styles.vibesLabel}>Campus Vibes</Text>
                            <View style={styles.vibesTags}>
                                {campusVibes.map((vibe, idx) => (
                                    <View key={idx} style={styles.vibeTag}>
                                        <Text style={styles.vibeTagText}>{vibe}</Text>
                                    </View>
                                ))}
                            </View>
                        </View>

                        {/* Current Mood */}
                        <View style={styles.moodSection}>
                            <Text style={styles.moodLabel}>Current Mood</Text>
                            <View style={styles.moodContent}>
                                <Text style={styles.moodEmoji}>{currentMood.emoji}</Text>
                                <Text style={styles.moodText}>{currentMood.text}</Text>
                            </View>
                        </View>
                    </View>
                </Animated.View>
            </GestureDetector>
        );
    }
);

SwipeCard.displayName = 'SwipeCard';

const styles = StyleSheet.create({
    card: {
        position: 'absolute',
        width: CARD_WIDTH,
        height: CARD_HEIGHT,
        borderRadius: 24,
        backgroundColor: '#1A1025',
        overflow: 'hidden',
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 12 },
        shadowOpacity: 0.4,
        shadowRadius: 24,
        elevation: 12,
        borderWidth: 1,
        borderColor: 'rgba(124, 58, 237, 0.2)',
    },
    photoContainer: {
        flex: 0.6,
        position: 'relative',
    },
    photo: {
        width: '100%',
        height: '100%',
    },
    noPhoto: {
        width: '100%',
        height: '100%',
        backgroundColor: '#2D2D44',
        justifyContent: 'center',
        alignItems: 'center',
    },
    photoIndicators: {
        position: 'absolute',
        top: 12,
        left: 16,
        right: 16,
        flexDirection: 'row',
        gap: 4,
    },
    photoIndicator: {
        flex: 1,
        height: 3,
        backgroundColor: 'rgba(255,255,255,0.3)',
        borderRadius: 2,
    },
    photoIndicatorActive: {
        backgroundColor: Colors.white,
    },
    photoNav: {
        position: 'absolute',
        top: 0,
        bottom: '30%',
        width: '50%',
    },
    photoNavLeft: {
        left: 0,
    },
    photoNavRight: {
        right: 0,
    },
    cardGradient: {
        position: 'absolute',
        bottom: 0,
        left: 0,
        right: 0,
        height: '50%',
    },
    swipeIndicator: {
        position: 'absolute',
        top: 60,
        paddingHorizontal: 16,
        paddingVertical: 8,
        borderWidth: 3,
        borderRadius: 8,
    },
    likeIndicator: {
        right: 20,
        borderColor: '#22C55E',
        transform: [{ rotate: '15deg' }],
    },
    nopeIndicator: {
        left: 20,
        borderColor: '#EF4444',
        transform: [{ rotate: '-15deg' }],
    },
    likeText: {
        color: '#22C55E',
        fontWeight: '800',
        fontSize: 24,
    },
    nopeText: {
        color: '#EF4444',
        fontWeight: '800',
        fontSize: 24,
    },
    infoSection: {
        flex: 0.4,
        paddingHorizontal: 16,
        paddingVertical: 12,
    },
    nameRow: {
        flexDirection: 'row',
        alignItems: 'center',
    },
    name: {
        fontSize: 24,
        fontWeight: '700',
        color: Colors.white,
    },
    age: {
        fontSize: 24,
        fontWeight: '400',
        color: Colors.white,
    },
    verifiedBadge: {
        marginLeft: 6,
    },
    majorRow: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 6,
        marginTop: 4,
    },
    majorText: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.7)',
    },
    vibesSection: {
        marginTop: 12,
    },
    vibesLabel: {
        fontSize: 13,
        fontWeight: '600',
        color: 'rgba(255,255,255,0.5)',
        marginBottom: 8,
    },
    vibesTags: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
    },
    vibeTag: {
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        paddingHorizontal: 12,
        paddingVertical: 6,
        borderRadius: 16,
        borderWidth: 1,
        borderColor: 'rgba(124, 58, 237, 0.3)',
    },
    vibeTagText: {
        fontSize: 12,
        color: Colors.white,
    },
    moodSection: {
        marginTop: 12,
    },
    moodLabel: {
        fontSize: 13,
        fontWeight: '600',
        color: 'rgba(255,255,255,0.5)',
        marginBottom: 8,
    },
    moodContent: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(255,255,255,0.05)',
        paddingHorizontal: 12,
        paddingVertical: 8,
        borderRadius: 12,
        gap: 8,
    },
    moodEmoji: {
        fontSize: 18,
    },
    moodText: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.8)',
    },
});

export default SwipeCard;
