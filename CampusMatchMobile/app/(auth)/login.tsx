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
    Image,
} from 'react-native';
import { Link, useRouter } from 'expo-router';
import { LinearGradient } from 'expo-linear-gradient';
import { Ionicons } from '@expo/vector-icons';
import { useAuthStore } from '@/stores/authStore';
import Colors from '@/constants/Colors';

export default function LoginScreen() {
    const router = useRouter();
    const { login, isLoading, error, clearError } = useAuthStore();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);

    const handleLogin = async () => {
        if (!email.trim() || !password.trim()) {
            Alert.alert('Error', 'Please enter both email and password');
            return;
        }

        try {
            await login(email.trim(), password);
            router.replace('/(tabs)/discover');
        } catch (err) {
            // Error is handled by store
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
                    {/* Logo Section */}
                    <View style={styles.logoSection}>
                        <View style={styles.logoContainer}>
                            <Image
                                source={require('@/assets/images/campus-match-logo.png')}
                                style={styles.logoImage}
                                resizeMode="contain"
                            />
                        </View>
                        <Text style={styles.appName}>CampusMatch</Text>
                        <Text style={styles.tagline}>Find your perfect match on campus</Text>
                    </View>

                    {/* Form Section */}
                    <View style={styles.formSection}>
                        {error && (
                            <View style={styles.errorContainer}>
                                <Text style={styles.errorText}>{error}</Text>
                                <TouchableOpacity onPress={clearError}>
                                    <Ionicons name="close" size={20} color={Colors.error} />
                                </TouchableOpacity>
                            </View>
                        )}

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

                        <View style={styles.inputContainer}>
                            <Ionicons name="lock-closed-outline" size={20} color={Colors.dark.textMuted} style={styles.inputIcon} />
                            <TextInput
                                style={styles.input}
                                placeholder="Password"
                                placeholderTextColor={Colors.dark.textMuted}
                                value={password}
                                onChangeText={setPassword}
                                secureTextEntry={!showPassword}
                                autoComplete="password"
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

                        <Link href="/(auth)/forgot-password" asChild>
                            <TouchableOpacity style={styles.forgotPassword}>
                                <Text style={styles.forgotPasswordText}>Forgot Password?</Text>
                            </TouchableOpacity>
                        </Link>

                        <TouchableOpacity
                            style={styles.loginButton}
                            onPress={handleLogin}
                            disabled={isLoading}
                        >
                            <LinearGradient
                                colors={Colors.primary.gradient}
                                start={{ x: 0, y: 0 }}
                                end={{ x: 1, y: 0 }}
                                style={styles.loginButtonGradient}
                            >
                                {isLoading ? (
                                    <ActivityIndicator color={Colors.white} />
                                ) : (
                                    <Text style={styles.loginButtonText}>Sign In</Text>
                                )}
                            </LinearGradient>
                        </TouchableOpacity>

                        <View style={styles.divider}>
                            <View style={styles.dividerLine} />
                            <Text style={styles.dividerText}>or</Text>
                            <View style={styles.dividerLine} />
                        </View>

                        <Link href="/(auth)/register" asChild>
                            <TouchableOpacity style={styles.registerButton}>
                                <Text style={styles.registerButtonText}>Create Account</Text>
                            </TouchableOpacity>
                        </Link>
                    </View>

                    {/* Demo Credentials */}
                    <View style={styles.demoSection}>
                        <Text style={styles.demoTitle}>Demo Credentials</Text>
                        <TouchableOpacity
                            onPress={() => {
                                setEmail('emma@mybvc.ca');
                                setPassword('password123');
                            }}
                        >
                            <Text style={styles.demoText}>emma@mybvc.ca / password123</Text>
                        </TouchableOpacity>
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
        justifyContent: 'center',
        padding: 24,
    },
    logoSection: {
        alignItems: 'center',
        marginBottom: 40,
    },
    logoContainer: {
        marginBottom: 16,
    },
    logoImage: {
        width: 150,
        height: 150,
        borderRadius: 75,
    },
    appName: {
        fontSize: 32,
        fontWeight: 'bold',
        color: Colors.white,
        marginBottom: 8,
    },
    tagline: {
        fontSize: 16,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
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
    forgotPassword: {
        alignSelf: 'flex-end',
        marginBottom: 24,
    },
    forgotPasswordText: {
        color: Colors.primary.main,
        fontSize: 14,
    },
    loginButton: {
        borderRadius: 12,
        overflow: 'hidden',
        marginBottom: 20,
    },
    loginButtonGradient: {
        paddingVertical: 16,
        alignItems: 'center',
        justifyContent: 'center',
    },
    loginButtonText: {
        color: Colors.white,
        fontSize: 18,
        fontWeight: '600',
    },
    divider: {
        flexDirection: 'row',
        alignItems: 'center',
        marginBottom: 20,
    },
    dividerLine: {
        flex: 1,
        height: 1,
        backgroundColor: Colors.dark.border,
    },
    dividerText: {
        color: Colors.dark.textMuted,
        marginHorizontal: 16,
        fontSize: 14,
    },
    registerButton: {
        borderRadius: 12,
        borderWidth: 1,
        borderColor: Colors.primary.main,
        paddingVertical: 16,
        alignItems: 'center',
    },
    registerButtonText: {
        color: Colors.primary.main,
        fontSize: 18,
        fontWeight: '600',
    },
    demoSection: {
        marginTop: 40,
        alignItems: 'center',
        opacity: 0.7,
    },
    demoTitle: {
        color: Colors.dark.textMuted,
        fontSize: 12,
        marginBottom: 4,
    },
    demoText: {
        color: Colors.dark.textSecondary,
        fontSize: 12,
    },
});
