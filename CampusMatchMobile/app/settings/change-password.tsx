import React, { useState } from 'react';
import {
    View,
    Text,
    TextInput,
    StyleSheet,
    TouchableOpacity,
    Alert,
    ActivityIndicator,
    KeyboardAvoidingView,
    Platform,
} from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { authApi } from '@/services';
import Colors from '@/constants/Colors';

export default function ChangePasswordScreen() {
    const router = useRouter();

    const [currentPassword, setCurrentPassword] = useState('');
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [showPasswords, setShowPasswords] = useState(false);

    const handleSubmit = async () => {
        if (!currentPassword.trim()) {
            Alert.alert('Error', 'Please enter your current password');
            return;
        }
        if (!newPassword) {
            Alert.alert('Error', 'Please enter a new password');
            return;
        }
        if (newPassword.length < 6) {
            Alert.alert('Error', 'New password must be at least 6 characters');
            return;
        }
        if (newPassword !== confirmPassword) {
            Alert.alert('Error', 'New passwords do not match');
            return;
        }

        setIsLoading(true);
        try {
            await authApi.changePassword({
                currentPassword,
                newPassword,
            });
            Alert.alert(
                'Success',
                'Your password has been changed successfully',
                [{ text: 'OK', onPress: () => router.back() }]
            );
        } catch (err: unknown) {
            const errorMessage = (err as { response?: { data?: { message?: string } } })?.response?.data?.message
                || 'Failed to change password. Please check your current password.';
            Alert.alert('Error', errorMessage);
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <KeyboardAvoidingView
            style={styles.container}
            behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
        >
            <View style={styles.content}>
                <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Current Password</Text>
                    <View style={styles.inputContainer}>
                        <TextInput
                            style={styles.input}
                            value={currentPassword}
                            onChangeText={setCurrentPassword}
                            placeholder="Enter current password"
                            placeholderTextColor={Colors.dark.textMuted}
                            secureTextEntry={!showPasswords}
                        />
                        <TouchableOpacity
                            onPress={() => setShowPasswords(!showPasswords)}
                            style={styles.showPasswordButton}
                        >
                            <Ionicons
                                name={showPasswords ? 'eye-off-outline' : 'eye-outline'}
                                size={20}
                                color={Colors.dark.textMuted}
                            />
                        </TouchableOpacity>
                    </View>
                </View>

                <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>New Password</Text>
                    <View style={styles.inputContainer}>
                        <TextInput
                            style={styles.input}
                            value={newPassword}
                            onChangeText={setNewPassword}
                            placeholder="Enter new password"
                            placeholderTextColor={Colors.dark.textMuted}
                            secureTextEntry={!showPasswords}
                        />
                    </View>
                </View>

                <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Confirm New Password</Text>
                    <View style={styles.inputContainer}>
                        <TextInput
                            style={styles.input}
                            value={confirmPassword}
                            onChangeText={setConfirmPassword}
                            placeholder="Confirm new password"
                            placeholderTextColor={Colors.dark.textMuted}
                            secureTextEntry={!showPasswords}
                        />
                    </View>
                </View>

                <Text style={styles.hint}>
                    Password must be at least 6 characters long
                </Text>

                <TouchableOpacity
                    style={styles.submitButton}
                    onPress={handleSubmit}
                    disabled={isLoading}
                >
                    <LinearGradient
                        colors={Colors.primary.gradient}
                        style={styles.submitButtonGradient}
                    >
                        {isLoading ? (
                            <ActivityIndicator color={Colors.white} />
                        ) : (
                            <Text style={styles.submitButtonText}>Change Password</Text>
                        )}
                    </LinearGradient>
                </TouchableOpacity>
            </View>
        </KeyboardAvoidingView>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: Colors.dark.background,
    },
    content: {
        padding: 20,
    },
    inputGroup: {
        marginBottom: 20,
    },
    inputLabel: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
        marginBottom: 8,
    },
    inputContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        borderRadius: 12,
        borderWidth: 1,
        borderColor: Colors.dark.border,
    },
    input: {
        flex: 1,
        padding: 16,
        color: Colors.dark.text,
        fontSize: 16,
    },
    showPasswordButton: {
        padding: 16,
    },
    hint: {
        fontSize: 13,
        color: Colors.dark.textMuted,
        marginBottom: 24,
    },
    submitButton: {
        borderRadius: 12,
        overflow: 'hidden',
    },
    submitButtonGradient: {
        paddingVertical: 16,
        alignItems: 'center',
    },
    submitButtonText: {
        color: Colors.white,
        fontSize: 16,
        fontWeight: '600',
    },
});
