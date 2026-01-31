import React, { useEffect, useState, useRef } from 'react';
import {
    View,
    Text,
    StyleSheet,
    TouchableOpacity,
    Animated,
    ViewStyle,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';;
import Colors from '@/constants/Colors';
import { useSubscriptionStore } from '@/stores/subscriptionStore';

interface SwipeLimitBannerProps {
    onUpgradePress: () => void;
}

export const SwipeLimitBanner: React.FC<SwipeLimitBannerProps> = ({ onUpgradePress }) => {
    const { swipesRemaining, swipesResetAt, features } = useSubscriptionStore();
    const [timeLeft, setTimeLeft] = useState<string>('');
    const pulseAnim = useRef(new Animated.Value(1)).current;

    // If unlimited swipes, don't show banner
    if (features?.unlimitedSwipes) {
        return null;
    }

    useEffect(() => {
        // Calculate time until reset
        const updateTimeLeft = () => {
            if (!swipesResetAt) {
                setTimeLeft('');
                return;
            }

            const now = new Date();
            const reset = new Date(swipesResetAt);
            const diff = reset.getTime() - now.getTime();

            if (diff <= 0) {
                setTimeLeft('Refreshing...');
                return;
            }

            const hours = Math.floor(diff / (1000 * 60 * 60));
            const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

            if (hours > 0) {
                setTimeLeft(`${hours}h ${minutes}m`);
            } else {
                setTimeLeft(`${minutes}m`);
            }
        };

        updateTimeLeft();
        const interval = setInterval(updateTimeLeft, 60000); // Update every minute

        return () => clearInterval(interval);
    }, [swipesResetAt]);

    // Pulse animation when swipes are low
    useEffect(() => {
        if (swipesRemaining <= 5 && swipesRemaining > 0) {
            Animated.loop(
                Animated.sequence([
                    Animated.timing(pulseAnim, {
                        toValue: 1.05,
                        duration: 500,
                        useNativeDriver: true,
                    }),
                    Animated.timing(pulseAnim, {
                        toValue: 1,
                        duration: 500,
                        useNativeDriver: true,
                    }),
                ])
            ).start();
        }
    }, [swipesRemaining, pulseAnim]);

    // Determine banner style based on remaining swipes
    const isLow = swipesRemaining <= 5;
    const isEmpty = swipesRemaining <= 0;

    if (isEmpty) {
        return (
            <TouchableOpacity onPress={onUpgradePress} activeOpacity={0.9}>
                <LinearGradient
                    colors={['#FF6B6B', '#FF8E53']}
                    start={{ x: 0, y: 0 }}
                    end={{ x: 1, y: 0 satisfies number }}
                    style={styles.emptyBanner}
                >
                    <Ionicons name="lock-closed" size={24} color="white" />
                    <View style={styles.emptyContent}>
                        <Text style={styles.emptyTitle}>Out of Swipes!</Text>
                        <Text style={styles.emptySubtitle}>
                            {timeLeft ? `Resets in ${timeLeft}` : 'Upgrade for unlimited'}
                        </Text>
                    </View>
                    <View style={styles.upgradeChip}>
                        <Text style={styles.upgradeChipText}>Upgrade</Text>
                    </View>
                </LinearGradient>
            </TouchableOpacity>
        );
    }

    return (
        <Animated.View
            style={[
                styles.banner,
                isLow && styles.bannerLow,
                { transform: [{ scale: pulseAnim }] }
            ] as any}
        >
            <View style={styles.swipeCount}>
                <Ionicons
                    name="flame"
                    size={18}
                    color={isLow ? '#FF6B6B' : '#7C3AED'}
                />
                <Text style={[styles.countText, isLow && styles.countTextLow]}>
                    {swipesRemaining}
                </Text>
                <Text style={styles.countLabel}>swipes left</Text>
            </View>

            {timeLeft && (
                <View style={styles.timerContainer}>
                    <Ionicons name="time-outline" size={14} color="#8E8E93" />
                    <Text style={styles.timerText}>{timeLeft}</Text>
                </View>
            )}

            <TouchableOpacity
                style={styles.unlimitedButton}
                onPress={onUpgradePress}
            >
                <Text style={styles.unlimitedText}>Go Unlimited</Text>
                <Ionicons name="arrow-forward" size={14} color="white" />
            </TouchableOpacity>
        </Animated.View>
    );
};

const styles = StyleSheet.create({
    banner: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        paddingHorizontal: 16,
        paddingVertical: 12,
        borderRadius: 16,
        marginHorizontal: 16,
        marginBottom: 12,
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 2 },
        shadowOpacity: 0.1,
        shadowRadius: 4,
        elevation: 3,
    },
    bannerLow: {
        backgroundColor: 'rgba(255, 107, 107, 0.1)',
        borderWidth: 1,
        borderColor: 'rgba(255, 107, 107, 0.3)',
    },
    swipeCount: {
        flexDirection: 'row',
        alignItems: 'center',
        flex: 1,
    },
    countText: {
        fontSize: 20,
        fontWeight: '700',
        color: Colors.white,
        marginLeft: 6,
    },
    countTextLow: {
        color: '#FF6B6B',
    },
    countLabel: {
        fontSize: 13,
        color: Colors.dark.textSecondary,
        marginLeft: 6,
    },
    timerContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        marginRight: 12,
    },
    timerText: {
        fontSize: 12,
        color: Colors.dark.textSecondary,
        marginLeft: 4,
    },
    unlimitedButton: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: '#7C3AED',
        paddingHorizontal: 12,
        paddingVertical: 8,
        borderRadius: 20,
    },
    unlimitedText: {
        fontSize: 12,
        fontWeight: '600',
        color: 'white',
        marginRight: 4,
    },
    // Empty state banner
    emptyBanner: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingHorizontal: 16,
        paddingVertical: 16,
        marginHorizontal: 16,
        marginBottom: 12,
        borderRadius: 16,
    },
    emptyContent: {
        flex: 1,
        marginLeft: 12,
    },
    emptyTitle: {
        fontSize: 16,
        fontWeight: '700',
        color: 'white',
    },
    emptySubtitle: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.8)',
        marginTop: 2,
    },
    upgradeChip: {
        backgroundColor: 'white',
        paddingHorizontal: 16,
        paddingVertical: 8,
        borderRadius: 20,
    },
    upgradeChipText: {
        fontSize: 14,
        fontWeight: '600',
        color: '#FF6B6B',
    },
});

export default SwipeLimitBanner;
