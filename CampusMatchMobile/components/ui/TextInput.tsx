import React, { useState } from 'react';
import {
    View,
    TextInput as RNTextInput,
    Text,
    StyleSheet,
    TextInputProps as RNTextInputProps,
    TouchableOpacity,
    ViewStyle,
    TextStyle,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';
import { Spacing, BorderRadius, Typography } from '@/constants/DesignTokens';

type IconName = keyof typeof Ionicons.glyphMap;

interface TextInputProps extends Omit<RNTextInputProps, 'style'> {
    /** Label text displayed above input */
    label?: string;
    /** Helper text displayed below input */
    helperText?: string;
    /** Error message - shows error state when present */
    error?: string;
    /** Icon to show on the left side */
    leftIcon?: IconName;
    /** Icon to show on the right side */
    rightIcon?: IconName;
    /** Whether this is a password field (adds show/hide toggle) */
    isPassword?: boolean;
    /** Custom container style */
    containerStyle?: ViewStyle;
    /** Custom input style */
    inputStyle?: TextStyle;
    /** Accessibility label for the input */
    accessibilityLabel?: string;
}

/**
 * Reusable TextInput Component
 * Replaces duplicated input implementations across app
 * 
 * Features:
 * - Icon support (left/right)
 * - Password visibility toggle
 * - Validation states (error)
 * - Helper text
 * - Full accessibility
 * 
 * Usage:
 * ```tsx
 * <TextInput
 *   label="Email"
 *   leftIcon="mail-outline"
 *   placeholder="Enter your email"
 *   value={email}
 *   onChangeText={setEmail}
 *   keyboardType="email-address"
 * />
 * 
 * <TextInput
 *   label="Password"
 *   isPassword
 *   value={password}
 *   onChangeText={setPassword}
 *   error={passwordError}
 * />
 * ```
 */
export default function TextInput({
    label,
    helperText,
    error,
    leftIcon,
    rightIcon,
    isPassword = false,
    containerStyle,
    inputStyle,
    accessibilityLabel,
    ...textInputProps
}: TextInputProps) {
    const [showPassword, setShowPassword] = useState(false);
    const [isFocused, setIsFocused] = useState(false);

    const hasError = !!error;
    const showRightIcon = rightIcon || isPassword;

    return (
        <View style={[styles.container, containerStyle]}>
            {/* Label */}
            {label && (
                <Text style={styles.label} accessible={false}>
                    {label}
                </Text>
            )}

            {/* Input Container */}
            <View
                style={[
                    styles.inputContainer,
                    isFocused && styles.inputContainerFocused,
                    hasError && styles.inputContainerError,
                ]}
            >
                {/* Left Icon */}
                {leftIcon && (
                    <Ionicons
                        name={leftIcon}
                        size={20}
                        color={hasError ? Colors.semantic.error : Colors.dark.textMuted}
                        style={styles.leftIcon}
                    />
                )}

                {/* Text Input */}
                <RNTextInput
                    {...textInputProps}
                    style={[styles.input, inputStyle]}
                    placeholderTextColor={Colors.dark.textMuted}
                    secureTextEntry={isPassword && !showPassword}
                    onFocus={(e) => {
                        setIsFocused(true);
                        textInputProps.onFocus?.(e);
                    }}
                    onBlur={(e) => {
                        setIsFocused(false);
                        textInputProps.onBlur?.(e);
                    }}
                    accessible={true}
                    accessibilityLabel={accessibilityLabel || label || textInputProps.placeholder}
                    accessibilityState={{
                        disabled: textInputProps.editable === false,
                    }}
                />

                {/* Right Icon or Password Toggle */}
                {showRightIcon && (
                    <TouchableOpacity
                        style={styles.rightIconButton}
                        onPress={isPassword ? () => setShowPassword(!showPassword) : undefined}
                        disabled={!isPassword}
                        accessible={isPassword}
                        accessibilityRole={isPassword ? 'button' : undefined}
                        accessibilityLabel={isPassword ? (showPassword ? 'Hide password' : 'Show password') : undefined}
                    >
                        <Ionicons
                            name={
                                isPassword
                                    ? showPassword
                                        ? 'eye-off-outline'
                                        : 'eye-outline'
                                    : rightIcon!
                            }
                            size={20}
                            color={hasError ? Colors.semantic.error : Colors.dark.textMuted}
                        />
                    </TouchableOpacity>
                )}
            </View>

            {/* Helper Text or Error */}
            {(helperText || error) && (
                <Text
                    style={[styles.helperText, hasError && styles.errorText]}
                    accessible={true}
                    accessibilityRole="alert"
                >
                    {error || helperText}
                </Text>
            )}
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        marginBottom: Spacing.lg,
    },
    label: {
        ...Typography.bodySmall,
        color: Colors.dark.textSecondary,
        marginBottom: Spacing.sm,
    },
    inputContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        borderRadius: BorderRadius.md,
        borderWidth: 1,
        borderColor: Colors.dark.border,
    },
    inputContainerFocused: {
        borderColor: Colors.primary.main,
    },
    inputContainerError: {
        borderColor: Colors.semantic.error,
    },
    leftIcon: {
        marginLeft: Spacing.lg,
    },
    input: {
        flex: 1,
        paddingVertical: Spacing.lg,
        paddingHorizontal: Spacing.md,
        color: Colors.dark.text,
        ...Typography.body,
    },
    rightIconButton: {
        padding: Spacing.lg,
    },
    helperText: {
        ...Typography.caption,
        color: Colors.dark.textMuted,
        marginTop: Spacing.sm,
        marginLeft: Spacing.xs,
    },
    errorText: {
        color: Colors.semantic.error,
    },
});
