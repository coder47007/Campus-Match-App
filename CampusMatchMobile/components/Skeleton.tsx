// Skeleton loader components for loading states
import React, { useEffect, useRef } from 'react';
import { View, StyleSheet, Animated, Dimensions, ViewStyle } from 'react-native';
import Colors from '@/constants/Colors';

const { width: SCREEN_WIDTH } = Dimensions.get('window');

interface SkeletonProps {
    width?: number | string;
    height?: number;
    borderRadius?: number;
    style?: ViewStyle;
}

// Basic skeleton component with shimmer animation
export function Skeleton({ width = '100%', height = 20, borderRadius = 8, style }: SkeletonProps) {
    const shimmerAnim = useRef(new Animated.Value(0)).current;

    useEffect(() => {
        const animation = Animated.loop(
            Animated.timing(shimmerAnim, {
                toValue: 1,
                duration: 1500,
                useNativeDriver: true,
            })
        );
        animation.start();
        return () => animation.stop();
    }, [shimmerAnim]);

    const translateX = shimmerAnim.interpolate({
        inputRange: [0, 1],
        outputRange: [-SCREEN_WIDTH, SCREEN_WIDTH],
    });

    return (
        <View
            style={[
                styles.skeleton,
                {
                    width: typeof width === 'number' ? width : undefined,
                    height,
                    borderRadius,
                },
                typeof width === 'string' && { width: width as any },
                style,
            ]}
        >
            <Animated.View
                style={[
                    styles.shimmer,
                    {
                        transform: [{ translateX }],
                    },
                ]}
            />
        </View>
    );
}

// Skeleton for profile cards in discover screen
export function SkeletonCard() {
    return (
        <View style={styles.card}>
            <Skeleton width="100%" height={400} borderRadius={20} />
            <View style={styles.cardContent}>
                <Skeleton width="60%" height={24} style={{ marginBottom: 8 }} />
                <Skeleton width="40%" height={16} style={{ marginBottom: 8 }} />
                <Skeleton width="80%" height={14} />
            </View>
        </View>
    );
}

// Skeleton for list items (matches, likes, etc.)
export function SkeletonListItem() {
    return (
        <View style={styles.listItem}>
            <Skeleton width={56} height={56} borderRadius={28} />
            <View style={styles.listItemContent}>
                <Skeleton width="50%" height={16} style={{ marginBottom: 6 }} />
                <Skeleton width="70%" height={12} />
            </View>
        </View>
    );
}

// Skeleton for chat messages
export function SkeletonMessage({ isOwn = false }: { isOwn?: boolean }) {
    return (
        <View style={[styles.message, isOwn && styles.messageOwn]}>
            <Skeleton
                width={isOwn ? 180 : 220}
                height={40}
                borderRadius={16}
                style={isOwn ? styles.messageBubbleOwn : styles.messageBubble}
            />
        </View>
    );
}

// Skeleton for profile header
export function SkeletonProfileHeader() {
    return (
        <View style={styles.profileHeader}>
            <Skeleton width={100} height={100} borderRadius={50} />
            <View style={styles.profileInfo}>
                <Skeleton width="60%" height={24} style={{ marginBottom: 8 }} />
                <Skeleton width="40%" height={16} style={{ marginBottom: 8 }} />
                <Skeleton width="80%" height={14} />
            </View>
        </View>
    );
}

// Skeleton for interest chips
export function SkeletonInterests() {
    return (
        <View style={styles.interests}>
            <Skeleton width={80} height={32} borderRadius={16} />
            <Skeleton width={100} height={32} borderRadius={16} />
            <Skeleton width={70} height={32} borderRadius={16} />
            <Skeleton width={90} height={32} borderRadius={16} />
        </View>
    );
}

const styles = StyleSheet.create({
    skeleton: {
        backgroundColor: Colors.dark.surface,
        overflow: 'hidden',
    },
    shimmer: {
        position: 'absolute',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: 'rgba(255, 255, 255, 0.05)',
        transform: [{ skewX: '-20deg' }],
    },
    card: {
        borderRadius: 20,
        overflow: 'hidden',
        backgroundColor: Colors.dark.card,
    },
    cardContent: {
        padding: 16,
    },
    listItem: {
        flexDirection: 'row',
        alignItems: 'center',
        padding: 12,
        backgroundColor: Colors.dark.surface,
        borderRadius: 12,
        marginBottom: 8,
    },
    listItemContent: {
        flex: 1,
        marginLeft: 12,
    },
    message: {
        marginVertical: 4,
        paddingHorizontal: 16,
    },
    messageOwn: {
        alignItems: 'flex-end',
    },
    messageBubble: {
        backgroundColor: Colors.dark.surface,
    },
    messageBubbleOwn: {
        backgroundColor: Colors.primary.main,
    },
    profileHeader: {
        alignItems: 'center',
        padding: 20,
    },
    profileInfo: {
        alignItems: 'center',
        marginTop: 16,
        width: '100%',
    },
    interests: {
        flexDirection: 'row',
        flexWrap: 'wrap',
        gap: 8,
        padding: 16,
    },
});

export default Skeleton;
