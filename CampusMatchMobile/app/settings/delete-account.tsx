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
import { authApi } from '@/services';
import { useAuthStore } from '@/stores/authStore';
import Colors from '@/constants/Colors';
import Button from '@/components/ui/Button';

export default function DeleteAccountScreen() {
    const router = useRouter();
    const { logout } = useAuthStore();

    const [password, setPassword] = useState('');
    const [confirmText, setConfirmText] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [showPassword, setShowPassword] = useState(false);

    const handleDeleteAccount = () => {
        if (!password.trim()) {
            Alert.alert('Error', 'Please enter your password');
            return;
        }
        if (confirmText !== 'DELETE') {
            Alert.alert('Error', 'Please type DELETE to confirm');
            return;
        }

        Alert.alert(
            'Delete Account',
            'This action cannot be undone. All your data, matches, and messages will be permanently deleted. Are you absolutely sure?',
            [
                { text: 'Cancel', style: 'cancel' },
                {
                    text: 'Delete Forever',
                    style: 'destructive',
                    onPress: performDelete,
                },
            ]
        );
    };

    const performDelete = async () => {
        setIsLoading(true);
        try {
            await authApi.deleteAccount({ password });
            await logout();
            router.replace('/(auth)/login');
        } catch (err: unknown) {
            const errorMessage = (err as { response?: { data?: { message?: string } } })?.response?.data?.message
                || 'Failed to delete account. Please check your password.';
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
                <View style={styles.warningContainer}>
                    <Ionicons name="warning" size={48} color={Colors.error} />
                    <Text style={styles.warningTitle}>Delete Your Account?</Text>
                    <Text style={styles.warningText}>
                        This action is permanent and cannot be undone. You will lose:
                    </Text>
                    <View style={styles.lostItemsList}>
                        <Text style={styles.lostItem}>• All your profile information</Text>
                        <Text style={styles.lostItem}>• All your photos</Text>
                        <Text style={styles.lostItem}>• All your matches</Text>
                        <Text style={styles.lostItem}>• All your messages</Text>
                        <Text style={styles.lostItem}>• Your account history</Text>
                    </View>
                </View>

                <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Enter your password to confirm</Text>
                    <View style={styles.inputContainer}>
                        <TextInput
                            style={styles.input}
                            value={password}
                            onChangeText={setPassword}
                            placeholder="Your password"
                            placeholderTextColor={Colors.dark.textMuted}
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
                </View>

                <View style={styles.inputGroup}>
                    <Text style={styles.inputLabel}>Type DELETE to confirm</Text>
                    <View style={styles.inputContainer}>
                        <TextInput
                            style={styles.input}
                            value={confirmText}
                            onChangeText={setConfirmText}
                            placeholder='Type "DELETE"'
                            placeholderTextColor={Colors.dark.textMuted}
                            autoCapitalize="characters"
                        />
                    </View>
                </View>

                <Button
                    variant="danger"
                    size="large"
                    onPress={handleDeleteAccount}
                    loading={isLoading}
                    disabled={confirmText !== 'DELETE'}
                    leftIcon={<Ionicons name="trash" size={20} color={Colors.white} />}
                    fullWidth
                    accessibilityLabel="Permanently delete your account"
                >
                    Delete My Account
                </Button>
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
    warningContainer: {
        alignItems: 'center',
        backgroundColor: 'rgba(255, 59, 48, 0.1)',
        borderRadius: 16,
        padding: 24,
        marginBottom: 24,
        borderWidth: 1,
        borderColor: 'rgba(255, 59, 48, 0.3)',
    },
    warningTitle: {
        fontSize: 20,
        fontWeight: 'bold',
        color: Colors.error,
        marginTop: 16,
        marginBottom: 12,
    },
    warningText: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
        marginBottom: 16,
    },
    lostItemsList: {
        alignSelf: 'flex-start',
    },
    lostItem: {
        fontSize: 14,
        color: Colors.dark.text,
        marginBottom: 6,
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
    deleteButton: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: Colors.error,
        paddingVertical: 16,
        borderRadius: 12,
        gap: 8,
        marginTop: 8,
    },
    deleteButtonDisabled: {
        opacity: 0.5,
    },
    deleteButtonText: {
        color: Colors.white,
        fontSize: 16,
        fontWeight: '600',
    },
});
