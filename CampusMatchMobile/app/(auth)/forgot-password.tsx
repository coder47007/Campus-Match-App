import React, { useState } from 'react';
import {
    View,
    Text,
    TextInput,
    TouchableOpacity,
    StyleSheet,
    KeyboardAvoidingView,
    Platform,
    ScrollView,
    ActivityIndicator,
    Alert,
} from 'react-native';
import { Link, useRouter } from 'expo-router';
import { LinearGradient } from 'expo-linear-gradient';
import { Ionicons } from '@expo/vector-icons';
import { authApi } from '@/services';
import Colors from '@/constants/Colors';

export default function ForgotPasswordScreen() {
    const router = useRouter();

    const [step, setStep] = useState<'email' | 'reset'>('email');
    const [email, setEmail] = useState('');
    const [token, setToken] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [showPassword, setShowPassword] = useState(false);

    const handleRequestReset = async () => {
        if (!email.trim()) {
            Alert.alert('Error', 'Please enter your email');
            return;
        }

        setIsLoading(true);
        try {
            await authApi.forgotPassword(email.trim());
            Alert.alert(
                'Check Your Email',
                'If an account exists with this email, you will receive a password reset token. Check the console output in development mode.',
                [{ text: 'OK', onPress: () => setStep('reset') }]
            );
        } catch (error: any) {
            const errorMessage = error.message || 'Failed to request reset. Please try again.';
            Alert.alert('Error', errorMessage);
        } finally {
            setIsLoading(false);
        }
    };

    const handleResetPassword = async () => {
        if (!token.trim()) {
            Alert.alert('Error', 'Please enter the reset token');
            return;
        }
        if (!newPassword) {
            Alert.alert('Error', 'Please enter a new password');
            return;
        }
        if (newPassword.length < 6) {
            Alert.alert('Error', 'Password must be at least 6 characters');
            return;
        }
        if (newPassword !== confirmPassword) {
            Alert.alert('Error', 'Passwords do not match');
            return;
        }

        setIsLoading(true);
        try {
            await authApi.resetPassword({
                email: email.trim(),
                token: token.trim(),
                newPassword,
            });
            Alert.alert(
                'Password Reset',
                'Your password has been reset successfully. Please login with your new password.',
                [{ text: 'OK', onPress: () => router.replace('/(auth)/login') }]
            );
        } catch (error: any) {
            const errorMessage = error.message || 'Failed to reset password. Please try again.';
            Alert.alert('Error', errorMessage);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <LinearGradient
            colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
            style={styles.container}
        >
            <KeyboardAvoidingView
                behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
                style={styles.keyboardView}
            >
                <ScrollView
                    contentContainerStyle={styles.scrollContent}
                    keyboardShouldPersistTaps="handled"
                >
                    {/* Header */}
                    <View style={styles.header}>
                        <Link href="/(auth)/login" asChild>
                            <TouchableOpacity style={styles.backButton}>
                                <Ionicons name="arrow-back" size={24} color={Colors.white} />
                            </TouchableOpacity>
                        </Link>
                        <Text style={styles.title}>
                            {step === 'email' ? 'Forgot Password' : 'Reset Password'}
                        </Text>
                        <Text style={styles.subtitle}>
                            {step === 'email'
                                ? "Enter your email and we'll send you a reset token"
                                : 'Enter the token and your new password'}
                        </Text>
                    </View>

                    {/* Form */}
                    <View style={styles.formSection}>
                        {step === 'email' ? (
                            <>
                                <View style={styles.inputContainer}>
                                    <Ionicons name="mail-outline" size={20} color={Colors.dark.textMuted} style={styles.inputIcon} />
                                    <TextInput
                                        style={styles.input}
                                        placeholder="Email"
                                        placeholderTextColor={Colors.dark.textMuted}
                                        value={email}
                                        onChangeText={setEmail}
                                        keyboardType="email-address"
                                        autoCapitalize="none"
                                        autoComplete="email"
                                    />
                                </View>

                                <TouchableOpacity
                                    style={styles.submitButton}
                                    onPress={handleRequestReset}
                                    disabled={isLoading}
                                >
                                    <LinearGradient
                                        colors={Colors.primary.gradient}
                                        start={{ x: 0, y: 0 }}
                                        end={{ x: 1, y: 0 }}
                                        style={styles.submitButtonGradient}
                                    >
                                        {isLoading ? (
                                            <ActivityIndicator color={Colors.white} />
                                        ) : (
                                            <Text style={styles.submitButtonText}>Send Reset Token</Text>
                                        )}
                                    </LinearGradient>
                                </TouchableOpacity>
                            </>
                        ) : (
                            <>
                                <View style={styles.inputContainer}>
                                    <Ionicons name="key-outline" size={20} color={Colors.dark.textMuted} style={styles.inputIcon} />
                                    <TextInput
                                        style={styles.input}
                                        placeholder="Reset Token"
                                        placeholderTextColor={Colors.dark.textMuted}
                                        value={token}
                                        onChangeText={setToken}
                                        autoCapitalize="none"
                                    />
                                </View>

                                <View style={styles.inputContainer}>
                                    <Ionicons name="lock-closed-outline" size={20} color={Colors.dark.textMuted} style={styles.inputIcon} />
                                    <TextInput
                                        style={styles.input}
                                        placeholder="New Password"
                                        placeholderTextColor={Colors.dark.textMuted}
                                        value={newPassword}
                                        onChangeText={setNewPassword}
                                        secureTextEntry={!showPassword}
                                    />
                                    <TouchableOpacity
                                        onPress={() => setShowPassword(!showPassword)}
                                        style={styles.showPasswordButton}
                                    >
                                        <Ionicons
                                            name={showPassword ? 'eye-off-outline' : 'eye-outline'}
                                            size={20}
                                            color={Colors.dark.textMuted}
                                        />
                                    </TouchableOpacity>
                                </View>

                                <View style={styles.inputContainer}>
                                    <Ionicons name="lock-closed-outline" size={20} color={Colors.dark.textMuted} style={styles.inputIcon} />
                                    <TextInput
                                        style={styles.input}
                                        placeholder="Confirm New Password"
                                        placeholderTextColor={Colors.dark.textMuted}
                                        value={confirmPassword}
                                        onChangeText={setConfirmPassword}
                                        secureTextEntry={!showPassword}
                                    />
                                </View>

                                <TouchableOpacity
                                    style={styles.submitButton}
                                    onPress={handleResetPassword}
                                    disabled={isLoading}
                                >
                                    <LinearGradient
                                        colors={Colors.primary.gradient}
                                        start={{ x: 0, y: 0 }}
                                        end={{ x: 1, y: 0 }}
                                        style={styles.submitButtonGradient}
                                    >
                                        {isLoading ? (
                                            <ActivityIndicator color={Colors.white} />
                                        ) : (
                                            <Text style={styles.submitButtonText}>Reset Password</Text>
                                        )}
                                    </LinearGradient>
                                </TouchableOpacity>

                                <TouchableOpacity
                                    style={styles.backToEmailButton}
                                    onPress={() => setStep('email')}
                                >
                                    <Text style={styles.backToEmailText}>Back to Email</Text>
                                </TouchableOpacity>
                            </>
                        )}
                    </View>
                </ScrollView>
            </KeyboardAvoidingView>
        </LinearGradient>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    keyboardView: {
        flex: 1,
    },
    scrollContent: {
        flexGrow: 1,
        padding: 24,
        paddingTop: 60,
    },
    header: {
        marginBottom: 32,
    },
    backButton: {
        marginBottom: 16,
    },
    title: {
        fontSize: 32,
        fontWeight: 'bold',
        color: Colors.white,
        marginBottom: 8,
    },
    subtitle: {
        fontSize: 16,
        color: Colors.dark.textSecondary,
        lineHeight: 24,
    },
    formSection: {
        width: '100%',
    },
    inputContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        borderRadius: 12,
        marginBottom: 16,
        borderWidth: 1,
        borderColor: Colors.dark.border,
    },
    inputIcon: {
        paddingLeft: 16,
    },
    input: {
        flex: 1,
        paddingVertical: 16,
        paddingHorizontal: 12,
        color: Colors.white,
        fontSize: 16,
    },
    showPasswordButton: {
        padding: 16,
    },
    submitButton: {
        borderRadius: 12,
        overflow: 'hidden',
        marginTop: 8,
    },
    submitButtonGradient: {
        paddingVertical: 16,
        alignItems: 'center',
        justifyContent: 'center',
    },
    submitButtonText: {
        color: Colors.white,
        fontSize: 18,
        fontWeight: '600',
    },
    backToEmailButton: {
        alignItems: 'center',
        marginTop: 16,
        padding: 12,
    },
    backToEmailText: {
        color: Colors.primary.main,
        fontSize: 16,
    },
});
