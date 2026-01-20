import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity, ViewStyle } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';
import { Spacing, BorderRadius, Typography } from '@/constants/DesignTokens';

interface EmptyStateProps {
    /** Icon name from Ionicons */
    icon: keyof typeof Ionicons.glyphMap;
    /** Main title */
    title: string;
    /** Description text */
    description?: string;
    /** Action button config */
    action?: {
        label: string;
        onPress: () => void;
    };
    /** Custom container style */
    style?: ViewStyle;
}

/**
 * Empty State Component
 * Shows when there's no content to display
 * 
 * Usage:
 * ```tsx
 * <EmptyState
 *   icon="heart-dislike-outline"
 *   title="No matches yet"
 *   description="Keep swiping to find your perfect match!"
 *   action={{
 *     label: "Discover More",
 *     onPress: () => router.push('/discover')
 *   }}
 * />
 * ```
 */
export default function EmptyState({
    icon,
    title,
    description,
    action,
    style,
}: EmptyStateProps) {
    return (
        <View style={[styles.container, style]}
            accessible={true}
            accessibilityLabel={`${title}. ${description || ''}`}
            accessibilityRole="text">
            <View style={styles.iconContainer}>
                <Ionicons name={icon} size={64} color={Colors.dark.textMuted} />
            </View>

            <Text style={styles.title}>{title}</Text>

            {description && (
                <Text style={styles.description}>{description}</Text>
            )}

            {action && (
                <TouchableOpacity
                    style={styles.actionButton}
                    onPress={action.onPress}
                    accessible={true}
                    accessibilityRole="button"
                    accessibilityLabel={action.label}
                >
                    <Text style={styles.actionText}>{action.label}</Text>
                </TouchableOpacity>
            )}
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        paddingHorizontal: Spacing.xxl,
    },
    iconContainer: {
        width: 120,
        height: 120,
        borderRadius: BorderRadius.round,
        backgroundColor: Colors.dark.overlay.light,
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: Spacing.xl,
    },
    title: {
        ...Typography.h3,
        color: Colors.dark.text,
        textAlign: 'center',
        marginBottom: Spacing.md,
    },
    description: {
        ...Typography.body,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
        marginBottom: Spacing.xxl,
    },
    actionButton: {
        paddingHorizontal: Spacing.xl,
        paddingVertical: Spacing.lg,
        backgroundColor: Colors.dark.overlay.medium,
        borderRadius: BorderRadius.md,
        borderWidth: 1,
        borderColor: Colors.dark.border,
    },
    actionText: {
        ...Typography.button,
        color: Colors.primary.main,
    },
});
