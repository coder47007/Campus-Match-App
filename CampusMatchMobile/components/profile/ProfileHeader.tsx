// Profile Header Component - Displays user avatar, name, verification badge
import React from 'react';
import { View, Text, Image, StyleSheet } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';

interface ProfileHeaderProps {
    name: string;
    age?: number;
    photoUrl?: string;
    isVerified?: boolean;
}

export default function ProfileHeader({ name, age, photoUrl, isVerified = true }: ProfileHeaderProps) {
    return (
        <View style={styles.avatarSection}>
            <LinearGradient
                colors={['#7C3AED', '#6D28D9', '#5B21B6']}
                style={styles.avatarRing}
            >
                {photoUrl ? (
                    <Image
                        source={{ uri: photoUrl }}
                        style={styles.avatar}
                    />
                ) : (
                    <View style={[styles.avatar, styles.noAvatar]}>
                        <Ionicons name="person" size={48} color={Colors.white} />
                    </View>
                )}
            </LinearGradient>

            <View style={styles.nameSection}>
                <Text style={styles.userName}>
                    {name}{age ? `, ${age}` : ''}
                </Text>
                <Ionicons name="checkmark-circle" size={22} color="#3B82F6" />
            </View>

            {isVerified && (
                <View style={styles.verifiedBadge}>
                    <Ionicons name="shield-checkmark" size={14} color="#22C55E" />
                    <Text style={styles.verifiedText}>Verified .edu Student</Text>
                </View>
            )}
        </View>
    );
}

const styles = StyleSheet.create({
    avatarSection: {
        alignItems: 'center',
        marginTop: 8,
    },
    avatarRing: {
        width: 120,
        height: 120,
        borderRadius: 60,
        padding: 4,
        justifyContent: 'center',
        alignItems: 'center',
    },
    avatar: {
        width: 112,
        height: 112,
        borderRadius: 56,
        backgroundColor: Colors.dark.surface,
    },
    noAvatar: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    nameSection: {
        flexDirection: 'row',
        alignItems: 'center',
        marginTop: 16,
        gap: 6,
    },
    userName: {
        fontSize: 24,
        fontWeight: '700',
        color: Colors.white,
    },
    verifiedBadge: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(34, 197, 94, 0.1)',
        paddingHorizontal: 12,
        paddingVertical: 6,
        borderRadius: 20,
        marginTop: 8,
        gap: 6,
    },
    verifiedText: {
        fontSize: 12,
        fontWeight: '500',
        color: '#22C55E',
    },
});
