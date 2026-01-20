// Profile Action Buttons Component - Edit, View Profile, Settings buttons
import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';

interface ProfileActionsProps {
    userId: number;
    isEditing: boolean;
    onEditToggle: () => void;
    onFilterPress: () => void;
}

export default function ProfileActions({
    userId,
    isEditing,
    onEditToggle,
    onFilterPress
}: ProfileActionsProps) {
    const router = useRouter();

    return (
        <View style={styles.actionsRow}>
            <TouchableOpacity
                style={styles.viewProfileButton}
                onPress={() => router.push(`/profile/${userId}`)}
            >
                <Ionicons name="eye-outline" size={18} color={Colors.white} />
                <Text style={styles.viewProfileText}>View Profile</Text>
            </TouchableOpacity>

            <TouchableOpacity
                style={styles.editButton}
                onPress={onEditToggle}
            >
                <Text style={styles.editButtonText}>
                    {isEditing ? 'Cancel' : 'Edit Profile'}
                </Text>
            </TouchableOpacity>

            <TouchableOpacity
                style={styles.iconButton}
                onPress={onFilterPress}
            >
                <Ionicons name="options-outline" size={20} color={Colors.white} />
            </TouchableOpacity>

            <TouchableOpacity
                style={styles.iconButton}
                onPress={() => router.push('/settings')}
            >
                <Ionicons name="settings-outline" size={20} color={Colors.white} />
            </TouchableOpacity>
        </View>
    );
}

const styles = StyleSheet.create({
    actionsRow: {
        flexDirection: 'row',
        alignItems: 'center',
        marginTop: 20,
        gap: 8,
    },
    viewProfileButton: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        paddingHorizontal: 14,
        paddingVertical: 12,
        borderRadius: 12,
        gap: 6,
        borderWidth: 1,
        borderColor: '#7C3AED',
    },
    viewProfileText: {
        fontSize: 13,
        fontWeight: '600',
        color: Colors.white,
    },
    editButton: {
        flex: 1,
        backgroundColor: Colors.dark.surface,
        paddingVertical: 12,
        borderRadius: 12,
        alignItems: 'center',
    },
    editButtonText: {
        fontSize: 13,
        fontWeight: '600',
        color: Colors.white,
    },
    iconButton: {
        width: 44,
        height: 44,
        borderRadius: 12,
        backgroundColor: Colors.dark.surface,
        justifyContent: 'center',
        alignItems: 'center',
    },
});
