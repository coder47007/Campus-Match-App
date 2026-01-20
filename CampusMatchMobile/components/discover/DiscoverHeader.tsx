// Discover Header Component - Filter button, title, and profile button
import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet } from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';

interface DiscoverHeaderProps {
    onFilterPress: () => void;
    hasActiveFilters?: boolean;
    hasNotification?: boolean;
}

export default function DiscoverHeader({
    onFilterPress,
    hasActiveFilters = false,
    hasNotification = false,
}: DiscoverHeaderProps) {
    const router = useRouter();

    return (
        <View style={styles.header}>
            <TouchableOpacity
                style={styles.headerIcon}
                onPress={onFilterPress}
                accessible={true}
                accessibilityLabel="Open filters"
            >
                <Ionicons name="options-outline" size={22} color={Colors.white} />
                {hasActiveFilters && <View style={styles.filterDot} />}
            </TouchableOpacity>

            <View style={styles.headerTitleContainer}>
                <Text style={styles.headerTitle}>Discover</Text>
            </View>

            <TouchableOpacity
                style={styles.headerIcon}
                onPress={() => router.push('/(tabs)/profile')}
                accessible={true}
                accessibilityLabel="View profile"
            >
                <Ionicons name="person-outline" size={22} color={Colors.white} />
                {hasNotification && <View style={styles.notificationDot} />}
            </TouchableOpacity>
        </View>
    );
}

const styles = StyleSheet.create({
    header: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        paddingHorizontal: 16,
        paddingVertical: 12,
    },
    headerIcon: {
        width: 40,
        height: 40,
        borderRadius: 20,
        backgroundColor: 'rgba(255,255,255,0.1)',
        justifyContent: 'center',
        alignItems: 'center',
    },
    headerTitleContainer: {
        flex: 1,
        alignItems: 'center',
    },
    headerTitle: {
        fontSize: 18,
        fontWeight: '700',
        color: Colors.white,
        letterSpacing: 0.5,
    },
    filterDot: {
        position: 'absolute',
        top: 6,
        right: 6,
        width: 8,
        height: 8,
        borderRadius: 4,
        backgroundColor: '#7C3AED',
    },
    notificationDot: {
        position: 'absolute',
        top: 6,
        right: 6,
        width: 8,
        height: 8,
        borderRadius: 4,
        backgroundColor: '#EF4444',
    },
});
