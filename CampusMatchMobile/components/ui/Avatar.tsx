import React from 'react';
import { View, Image, Text, StyleSheet, ViewStyle } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';
import { BorderRadius } from '@/constants/DesignTokens';

type AvatarSize = 'xs' | 'sm' | 'md' | 'lg' | 'xl';
type StatusIndicator = 'online' | 'offline' | 'away' | 'none';

interface AvatarProps {
    /** Image URL */
    imageUrl?: string | null;
    /** User's name - used for initials fallback */
    name?: string;
    /** Size of the avatar */
    size?: AvatarSize;
    /** Status indicator color */
    status?: StatusIndicator;
    /** Show verified badge */
    verified?: boolean;
    /** Custom container style */
    style?: ViewStyle;
}

const SIZE_MAP: Record<AvatarSize, number> = {
    xs: 32,
    sm: 40,
    md: 52,
    lg: 64,
    xl: 80,
};

const STATUS_COLORS: Record<Exclude<StatusIndicator, 'none'>, string> = {
    online: Colors.semantic.success,
    offline: Colors.dark.textMuted,
    away: Colors.semantic.warning,
};

/**
 * Reusable Avatar Component
 * Shows user profile pictures with fallback to initials
 * 
 * Features:
 * - 5 size variants
 * - Status indicator (online/offline/away)
 * - Verified badge
 * - Fallback to initials
 * - Loading state
 * 
 * Usage:
 * ```tsx
 * <Avatar
 *   imageUrl={user.photoUrl}
 *   name={user.name}
 *   size="md"
 *   status="online"
 *   verified
 * />
 * 
 * <Avatar
 *   name="John Doe"
 *   size="sm"
 * />
 * ```
 */
export default function Avatar({
    imageUrl,
    name,
    size = 'md',
    status = 'none',
    verified = false,
    style,
}: AvatarProps) {
    const avatarSize = SIZE_MAP[size];
    const initialsSize = avatarSize * 0.4;
    const statusSize = avatarSize * 0.25;
    const verifiedSize = avatarSize * 0.3;

    // Get initials from name
    const getInitials = (name?: string): string => {
        if (!name) return '?';
        const parts = name.trim().split(' ');
        if (parts.length >= 2) {
            return `${parts[0][0]}${parts[parts.length - 1][0]}`.toUpperCase();
        }
        return name.substring(0, 2).toUpperCase();
    };

    return (
        <View style={[styles.container, { width: avatarSize, height: avatarSize }, style]}>
            {/* Avatar Image or Initials */}
            {imageUrl ? (
                <Image
                    source={{ uri: imageUrl }}
                    style={[styles.image, { width: avatarSize, height: avatarSize, borderRadius: avatarSize / 2 }]}
                    resizeMode="cover"
                />
            ) : (
                <View
                    style={[
                        styles.initialsContainer,
                        { width: avatarSize, height: avatarSize, borderRadius: avatarSize / 2 },
                    ]}
                >
                    <Text style={[styles.initials, { fontSize: initialsSize }]}>
                        {getInitials(name)}
                    </Text>
                </View>
            )}

            {/* Status Indicator */}
            {status !== 'none' && (
                <View
                    style={[
                        styles.statusIndicator,
                        {
                            width: statusSize,
                            height: statusSize,
                            borderRadius: statusSize / 2,
                            backgroundColor: STATUS_COLORS[status],
                            borderWidth: avatarSize > 50 ? 2 : 1.5,
                        },
                    ]}
                />
            )}

            {/* Verified Badge */}
            {verified && (
                <View
                    style={[
                        styles.verifiedBadge,
                        {
                            width: verifiedSize,
                            height: verifiedSize,
                        },
                    ]}
                >
                    <Ionicons name="checkmark-circle" size={verifiedSize} color={Colors.semantic.info} />
                </View>
            )}
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        position: 'relative',
    },
    image: {
        backgroundColor: Colors.dark.surface,
    },
    initialsContainer: {
        backgroundColor: Colors.dark.surfaceElevated,
        justifyContent: 'center',
        alignItems: 'center',
    },
    initials: {
        color: Colors.dark.text,
        fontWeight: '600',
    },
    statusIndicator: {
        position: 'absolute',
        bottom: 0,
        right: 0,
        borderColor: Colors.dark.background,
    },
    verifiedBadge: {
        position: 'absolute',
        bottom: -2,
        right: -2,
    },
});
