import React, { useState } from 'react';
import {
    View,
    Text,
    TextInput as RNTextInput,
    TouchableOpacity,
    StyleSheet,
    KeyboardAvoidingView,
    Platform,
    ScrollView,
    ActivityIndicator,
    Alert,
} from 'react-native';
import { Link, useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { useAuthStore } from '@/stores/authStore';
import Colors from '@/constants/Colors';
import Button from '@/components/ui/Button';
import TextInput from '@/components/ui/TextInput';

export default function RegisterScreen() {
    const router = useRouter();
    const { register, isLoading, error, clearError } = useAuthStore();

    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [phoneNumber, setPhoneNumber] = useState('');
    const [instagramHandle, setInstagramHandle] = useState('');
    const [showPassword, setShowPassword] = useState(false);

    const validateForm = () => {
        if (!name.trim()) {
            Alert.alert('Error', 'Please enter your name');
            return false;
        }
        if (name.trim().length < 2) {
            Alert.alert('Error', 'Name must be at least 2 characters');
            return false;
        }
        if (!email.trim()) {
            Alert.alert('Error', 'Please enter your email');
            return false;
        }
        if (!email.includes('@')) {
            Alert.alert('Error', 'Please enter a valid email');
            return false;
        }
        if (!password) {
            Alert.alert('Error', 'Please enter a password');
            return false;
        }
        if (password.length < 6) {
            Alert.alert('Error', 'Password must be at least 6 characters');
            return false;
        }
        if (password !== confirmPassword) {
            Alert.alert('Error', 'Passwords do not match');
            return false;
        }
        if (!phoneNumber.trim()) {
            Alert.alert('Error', 'Please enter your phone number');
            return false;
        }
        return true;
    };

    const handleRegister = async () => {
        if (!validateForm()) return;

        try {
            await register({
                name: name.trim(),
                email: email.trim(),
                password,
                phoneNumber: phoneNumber.trim(),
                instagramHandle: instagramHandle.trim() || undefined,
            });
            router.replace('/(tabs)/discover');
        } catch (err) {
            // Error is handled by store
        }
    };

    return (
        <View style={styles.container}>
            <KeyboardAvoidingView
                behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
                style={styles.keyboardView}
            >
                <ScrollView
                    contentContainerStyle={styles.scrollContent}
                    keyboardShouldPersistTaps="handled"
                    showsVerticalScrollIndicator={false}
                >
                    {/* Header */}
                    <View style={styles.header}>
                        <Link href="/(auth)/login" asChild>
                            <TouchableOpacity style={styles.backButton}>
                                <Ionicons name="arrow-back" size={24} color={Colors.white} />
                            </TouchableOpacity>
                        </Link>
                        <Text style={styles.title}>Create Account</Text>
                        <Text style={styles.subtitle}>Join CampusMatch today</Text>
                    </View>

                    {/* Form */}
                    <View style={styles.formSection}>
                        {error && (
                            <View style={styles.errorContainer}>
                                <Text style={styles.errorText}>{error}</Text>
                                <TouchableOpacity onPress={clearError}>
                                    <Ionicons name="close" size={20} color={Colors.error} />
                                </TouchableOpacity>
                            </View>
                        )}

                        <TextInput
                            label="Full Name"
                            leftIcon="person-outline"
                            placeholder="Full Name"
                            value={name}
                            onChangeText={setName}
                            autoComplete="name"
                        />

                        <TextInput
                            label="Email"
                            leftIcon="mail-outline"
                            placeholder="Email"
                            value={email}
                            onChangeText={setEmail}
                            keyboardType="email-address"
                            autoCapitalize="none"
                            autoComplete="email"
                        />

                        <TextInput
                            label="Phone Number"
                            leftIcon="call-outline"
                            placeholder="Phone Number"
                            value={phoneNumber}
                            onChangeText={setPhoneNumber}
                            keyboardType="phone-pad"
                            autoComplete="tel"
                        />

                        <TextInput
                            label="Instagram Handle (optional)"
                            leftIcon="logo-instagram"
                            placeholder="Instagram Handle"
                            value={instagramHandle}
                            onChangeText={setInstagramHandle}
                            autoCapitalize="none"
                        />

                        <TextInput
                            label="Password"
                            isPassword
                            placeholder="Password"
                            value={password}
                            onChangeText={setPassword}
                        />

                        <TextInput
                            label="Confirm Password"
                            isPassword
                            placeholder="Confirm Password"
                            value={confirmPassword}
                            onChangeText={setConfirmPassword}
                        />

                        <Button
                            variant="primary"
                            size="large"
                            onPress={handleRegister}
                            loading={isLoading}
                            fullWidth
                            accessibilityLabel="Create your account"
                        >
                            Create Account
                        </Button>

                        <View style={styles.loginLink}>
                            <Text style={styles.loginText}>Already have an account? </Text>
                            <Link href="/(auth)/login" asChild>
                                <TouchableOpacity>
                                    <Text style={styles.loginLinkText}>Sign In</Text>
                                </TouchableOpacity>
                            </Link>
                        </View>
                    </View>
                </ScrollView>
            </KeyboardAvoidingView>
        </View>
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
    },
    formSection: {
        width: '100%',
    },
    errorContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        backgroundColor: 'rgba(255, 59, 48, 0.1)',
        borderRadius: 12,
        padding: 12,
        marginBottom: 16,
        borderWidth: 1,
        borderColor: Colors.error,
    },
    errorText: {
        color: Colors.error,
        fontSize: 14,
        flex: 1,
        marginRight: 8,
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
    registerButton: {
        borderRadius: 12,
        overflow: 'hidden',
        marginTop: 8,
        marginBottom: 24,
    },
    registerButtonGradient: {
        paddingVertical: 16,
        alignItems: 'center',
        justifyContent: 'center',
    },
    registerButtonText: {
        color: Colors.white,
        fontSize: 18,
        fontWeight: '600',
    },
    loginLink: {
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center',
    },
    loginText: {
        color: Colors.dark.textSecondary,
        fontSize: 14,
    },
    loginLinkText: {
        color: Colors.primary.main,
        fontSize: 14,
        fontWeight: '600',
    },
});
