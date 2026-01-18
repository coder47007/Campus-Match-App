import React from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    TouchableOpacity,
    Alert,
} from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { useAuthStore } from '@/stores/authStore';
import Colors from '@/constants/Colors';
import { LinearGradient } from 'expo-linear-gradient';

interface SettingsItemProps {
    icon: keyof typeof Ionicons.glyphMap;
    title: string;
    subtitle?: string;
    onPress: () => void;
    danger?: boolean;
}

function SettingsItem({ icon, title, subtitle, onPress, danger }: SettingsItemProps) {
    return (
        <TouchableOpacity style={styles.settingsItem} onPress={onPress}>
            <View style={[styles.iconContainer, danger && styles.iconContainerDanger]}>
                <Ionicons name={icon} size={22} color={danger ? Colors.error : Colors.primary.main} />
            </View>
            <View style={styles.settingsItemContent}>
                <Text style={[styles.settingsItemTitle, danger && styles.dangerText]}>{title}</Text>
                {subtitle && <Text style={styles.settingsItemSubtitle}>{subtitle}</Text>}
            </View>
            <Ionicons name="chevron-forward" size={20} color={Colors.dark.textMuted} />
        </TouchableOpacity>
    );
}

export default function SettingsScreen() {
    const router = useRouter();
    const { user, logout } = useAuthStore();

    const handleLogout = () => {
        Alert.alert(
            'Logout',
            'Are you sure you want to logout?',
            [
                { text: 'Cancel', style: 'cancel' },
                {
                    text: 'Logout',
                    style: 'destructive',
                    onPress: async () => {
                        await logout();
                        router.replace('/(auth)/login');
                    },
                },
            ]
        );
    };

    return (
        <LinearGradient
            colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
            style={styles.container}
        >
            <ScrollView style={styles.scrollContent} showsVerticalScrollIndicator={false}>
                {/* User Info Header */}
                <View style={styles.header}>
                    <View style={styles.userInfo}>
                        <Text style={styles.userName}>{user?.name}</Text>
                        <Text style={styles.userEmail}>{user?.email}</Text>
                    </View>
                </View>

                {/* Discovery Settings */}
                <View style={styles.section}>
                    <Text style={styles.sectionTitle}>Discovery</Text>
                    <SettingsItem
                        icon="options-outline"
                        title="Preferences"
                        subtitle="Age, distance, and more"
                        onPress={() => router.push('/settings/preferences')}
                    />
                </View>

                {/* Appearance */}
                <View style={styles.section}>
                    <Text style={styles.sectionTitle}>Appearance</Text>
                    <SettingsItem
                        icon="moon-outline"
                        title="Theme"
                        subtitle="Dark, Light, or System"
                        onPress={() => router.push('/settings/appearance')}
                    />
                </View>

                {/* Privacy & Safety */}
                <View style={styles.section}>
                    <Text style={styles.sectionTitle}>Privacy & Safety</Text>
                    <SettingsItem
                        icon="ban-outline"
                        title="Blocked Users"
                        subtitle="Manage blocked profiles"
                        onPress={() => router.push('/settings/blocked')}
                    />
                </View>


                {/* Account */}
                <View style={styles.section}>
                    <Text style={styles.sectionTitle}>Account</Text>
                    <SettingsItem
                        icon="lock-closed-outline"
                        title="Change Password"
                        onPress={() => router.push('/settings/change-password')}
                    />
                    <SettingsItem
                        icon="trash-outline"
                        title="Delete Account"
                        subtitle="Permanently delete your account"
                        onPress={() => router.push('/settings/delete-account')}
                        danger
                    />
                </View>

                {/* Logout */}
                <View style={styles.section}>
                    <TouchableOpacity style={styles.logoutButton} onPress={handleLogout}>
                        <Ionicons name="log-out-outline" size={22} color={Colors.error} />
                        <Text style={styles.logoutText}>Logout</Text>
                    </TouchableOpacity>
                </View>

                {/* Version */}
                <View style={styles.footer}>
                    <Text style={styles.versionText}>CampusMatch v1.0.0</Text>
                </View>
            </ScrollView>
        </LinearGradient>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    scrollContent: {
        flex: 1,
    },
    header: {
        padding: 20,
        borderBottomWidth: 1,
        borderBottomColor: Colors.dark.border,
    },
    userInfo: {
        gap: 4,
    },
    userName: {
        fontSize: 24,
        fontWeight: 'bold',
        color: Colors.dark.text,
    },
    userEmail: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
    },
    section: {
        marginTop: 24,
        paddingHorizontal: 16,
    },
    sectionTitle: {
        fontSize: 13,
        fontWeight: '600',
        color: Colors.dark.textSecondary,
        textTransform: 'uppercase',
        letterSpacing: 1,
        marginBottom: 8,
        marginLeft: 4,
    },
    settingsItem: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        padding: 14,
        borderRadius: 12,
        marginBottom: 8,
    },
    iconContainer: {
        width: 36,
        height: 36,
        borderRadius: 8,
        backgroundColor: 'rgba(255, 107, 107, 0.1)',
        justifyContent: 'center',
        alignItems: 'center',
        marginRight: 12,
    },
    iconContainerDanger: {
        backgroundColor: 'rgba(255, 59, 48, 0.1)',
    },
    settingsItemContent: {
        flex: 1,
    },
    settingsItemTitle: {
        fontSize: 16,
        color: Colors.dark.text,
    },
    settingsItemSubtitle: {
        fontSize: 13,
        color: Colors.dark.textMuted,
        marginTop: 2,
    },
    dangerText: {
        color: Colors.error,
    },
    logoutButton: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: 'rgba(255, 59, 48, 0.1)',
        padding: 16,
        borderRadius: 12,
        gap: 10,
    },
    logoutText: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.error,
    },
    footer: {
        alignItems: 'center',
        padding: 40,
    },
    versionText: {
        fontSize: 12,
        color: Colors.dark.textMuted,
    },
});
