// New Match Bubble Component - Circular avatar for new matches
import React from 'react';
import { View, Text, TouchableOpacity, Image, StyleSheet } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';

export interface NewMatchData {
    id: number;
    name: string;
    photoUrl?: string;
    hasLastMessage: boolean;
}

interface NewMatchBubbleProps {
    match: NewMatchData;
    onPress: () => void;
}

export default function NewMatchBubble({ match, onPress }: NewMatchBubbleProps) {
    return (
        <TouchableOpacity
            style={styles.container}
            onPress={onPress}
            activeOpacity={0.8}
        >
            <LinearGradient
                colors={['#7C3AED', '#6D28D9']}
                style={styles.ring}
            >
                {match.photoUrl ? (
                    <Image
                        source={{ uri: match.photoUrl }}
                        style={styles.photo}
                    />
                ) : (
                    <View style={[styles.photo, styles.noPhoto]}>
                        <Ionicons name="person" size={24} color={Colors.white} />
                    </View>
                )}
            </LinearGradient>

            {!match.hasLastMessage && (
                <View style={styles.newBadge}>
                    <Text style={styles.newBadgeText}>New</Text>
                </View>
            )}

            <Text style={styles.name} numberOfLines={1}>
                {match.name?.split(' ')[0]}
            </Text>
        </TouchableOpacity>
    );
}

const styles = StyleSheet.create({
    container: {
        alignItems: 'center',
        marginRight: 16,
        width: 72,
    },
    ring: {
        width: 68,
        height: 68,
        borderRadius: 34,
        padding: 3,
        justifyContent: 'center',
        alignItems: 'center',
    },
    photo: {
        width: 62,
        height: 62,
        borderRadius: 31,
        borderWidth: 2,
        borderColor: Colors.dark.background,
    },
    noPhoto: {
        backgroundColor: Colors.dark.surface,
        justifyContent: 'center',
        alignItems: 'center',
    },
    newBadge: {
        position: 'absolute',
        top: 0,
        right: 0,
        backgroundColor: '#22C55E',
        paddingHorizontal: 6,
        paddingVertical: 2,
        borderRadius: 8,
        borderWidth: 2,
        borderColor: Colors.dark.background,
    },
    newBadgeText: {
        fontSize: 9,
        fontWeight: '700',
        color: Colors.white,
    },
    name: {
        marginTop: 6,
        fontSize: 12,
        color: Colors.white,
        textAlign: 'center',
        fontWeight: '500',
    },
});
