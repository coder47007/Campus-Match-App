// Conversation Card Component - Match with message preview
import React from 'react';
import { View, Text, TouchableOpacity, Image, StyleSheet } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Colors from '@/constants/Colors';

export interface ConversationData {
    id: number;
    name: string;
    photoUrl?: string;
    lastMessage?: string;
}

interface ConversationCardProps {
    match: ConversationData;
    onPress: () => void;
}

export default function ConversationCard({ match, onPress }: ConversationCardProps) {
    const isNew = match.lastMessage && !match.lastMessage.includes('You:');

    return (
        <TouchableOpacity
            style={styles.container}
            onPress={onPress}
            activeOpacity={0.8}
        >
            <View style={styles.avatarContainer}>
                {match.photoUrl ? (
                    <Image
                        source={{ uri: match.photoUrl }}
                        style={styles.photo}
                    />
                ) : (
                    <View style={[styles.photo, styles.noPhoto]}>
                        <Ionicons name="person" size={28} color={Colors.white} />
                    </View>
                )}
            </View>

            <View style={styles.info}>
                <View style={styles.header}>
                    <Text style={styles.name}>{match.name}</Text>
                    {isNew && (
                        <View style={styles.newMessageBadge}>
                            <Text style={styles.newMessageText}>New</Text>
                        </View>
                    )}
                </View>
                <Text style={styles.preview} numberOfLines={1}>
                    {match.lastMessage || 'New match! Say hello 👋'}
                </Text>
            </View>
        </TouchableOpacity>
    );
}

const styles = StyleSheet.create({
    container: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: 'rgba(255,255,255,0.05)',
        borderRadius: 16,
        padding: 12,
        marginHorizontal: 16,
        marginBottom: 10,
    },
    avatarContainer: {
        marginRight: 12,
    },
    photo: {
        width: 56,
        height: 56,
        borderRadius: 28,
    },
    noPhoto: {
        backgroundColor: Colors.dark.surface,
        justifyContent: 'center',
        alignItems: 'center',
    },
    info: {
        flex: 1,
    },
    header: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
        marginBottom: 4,
    },
    name: {
        fontSize: 16,
        fontWeight: '700',
        color: Colors.white,
    },
    newMessageBadge: {
        backgroundColor: '#7C3AED',
        paddingHorizontal: 8,
        paddingVertical: 2,
        borderRadius: 8,
    },
    newMessageText: {
        fontSize: 10,
        fontWeight: '700',
        color: Colors.white,
    },
    preview: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.6)',
    },
});
