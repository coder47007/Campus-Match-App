import React from 'react';
import {
    TouchableOpacity,
    Text,
    StyleSheet,
    ActivityIndicator,
    TouchableOpacityProps,
    ViewStyle,
    TextStyle,
} from 'react-native';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';
import { Spacing, BorderRadius, Typography } from '@/constants/DesignTokens';

type ButtonVariant = 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
type ButtonSize = 'small' | 'medium' | 'large';

interface ButtonProps extends Omit<TouchableOpacityProps, 'style'> {
    /** Button text */
    children: string;
    /** Visual style variant */
    variant?: ButtonVariant;
    /** Button size */
    size?: ButtonSize;
    /** Show loading spinner */
    loading?: boolean;
    /** Full width button */
    fullWidth?: boolean;
    /** Icon to show on left */
    leftIcon?: React.ReactNode;
    /** Icon to show on right */
    rightIcon?: React.ReactNode;
    /** Custom container style */
    style?: ViewStyle;
    /** Custom text style */
    textStyle?: TextStyle;
    /** Accessibility label */
    accessibilityLabel?: string;
}

/**
 * Reusable Button Component
 * Replaces duplicated button code across auth screens and throughout app
 * 
 * Usage:
 * ```tsx
 * <Button variant="primary" onPress={handleLogin} loading={isLoading}>
 *   Sign In
 * </Button>
 * ```
 */
export default function Button({
    children,
    variant = 'primary',
    size = 'medium',
    loading = false,
    fullWidth = false,
    leftIcon,
    rightIcon,
    disabled,
    style,
    textStyle,
    accessibilityLabel,
    ...props
}: ButtonProps) {
    const isPrimary = variant === 'primary';
    const isDanger = variant === 'danger';
    const isOutline = variant === 'outline';
    const isGhost = variant === 'ghost';

    const sizeStyles = {
        small: {
            paddingVertical: Spacing.md,
            paddingHorizontal: Spacing.lg,
            ...Typography.button,
        },
        medium: {
            paddingVertical: Spacing.lg,
            paddingHorizontal: Spacing.xl,
            ...Typography.button,
        },
        large: {
            paddingVertical: Spacing.xl,
            paddingHorizontal: Spacing.xxl,
            ...Typography.buttonLarge,
        },
    };

    const buttonContent = (
        <>
            {leftIcon && <>{leftIcon}</>}
            {loading ? (
                <ActivityIndicator
                    size="small"
                    color={isOutline || isGhost ? Colors.primary.main : Colors.white}
                />
            ) : (
                <Text
                    style={[
                        styles.text,
                        sizeStyles[size],
                        isPrimary && styles.primaryText,
                        isDanger && styles.dangerText,
                        isOutline && styles.outlineText,
                        isGhost && styles.ghostText,
                        disabled && styles.disabledText,
                        textStyle,
                    ]}
                >
                    {children}
                </Text>
            )}
            {rightIcon && <>{rightIcon}</>}
        </>
    );

    const containerStyle = [
        styles.button,
        fullWidth && styles.fullWidth,
        disabled && styles.disabled,
        style,
    ];

    // Primary and danger use gradients
    if ((isPrimary || isDanger) && !disabled) {
        return (
            <TouchableOpacity
                {...props}
                disabled={disabled || loading}
                style={[containerStyle, styles.gradientButton]}
                accessible={true}
                accessibilityRole="button"
                accessibilityLabel={accessibilityLabel || children}
                accessibilityState={{ disabled: disabled || loading }}
            >
                <LinearGradient
                    colors={isDanger ? [Colors.semantic.error, Colors.semantic.errorLight] : Colors.gradients.primary}
                    start={{ x: 0, y: 0 }}
                    end={{ x: 1, y: 0 }}
                    style={[styles.gradient, sizeStyles[size]]}
                >
                    {buttonContent}
                </LinearGradient>
            </TouchableOpacity>
        );
    }

    // Outline, secondary, and ghost use solid backgrounds
    return (
        <TouchableOpacity
            {...props}
            disabled={disabled || loading}
            style={[
                containerStyle,
                sizeStyles[size],
                variant === 'secondary' && styles.secondary,
                isOutline && styles.outline,
                isGhost && styles.ghost,
            ]}
            accessible={true}
            accessibilityRole="button"
            accessibilityLabel={accessibilityLabel || children}
            accessibilityState={{ disabled: disabled || loading }}
        >
            {buttonContent}
        </TouchableOpacity>
    );
}

const styles = StyleSheet.create({
    button: {
        borderRadius: BorderRadius.md,
        overflow: 'hidden',
        alignItems: 'center',
        justifyContent: 'center',
        flexDirection: 'row',
        gap: Spacing.sm,
    },
    fullWidth: {
        width: '100%',
    },
    gradientButton: {
        padding: 0,
    },
    gradient: {
        width: '100%',
        alignItems: 'center',
        justifyContent: 'center',
        flexDirection: 'row',
        gap: Spacing.sm,
    },
    secondary: {
        backgroundColor: Colors.dark.surfaceElevated,
        borderWidth: 1,
        borderColor: Colors.dark.border,
    },
    outline: {
        backgroundColor: 'transparent',
        borderWidth: 1,
        borderColor: Colors.primary.main,
    },
    ghost: {
        backgroundColor: 'transparent',
    },
    disabled: {
        opacity: 0.5,
    },
    text: {
        textAlign: 'center',
    },
    primaryText: {
        color: Colors.white,
    },
    dangerText: {
        color: Colors.white,
    },
    outlineText: {
        color: Colors.primary.main,
    },
    ghostText: {
        color: Colors.primary.main,
    },
    disabledText: {
        color: Colors.dark.textMuted,
    },
});
