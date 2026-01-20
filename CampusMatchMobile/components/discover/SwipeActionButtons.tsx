// Swipe Action Buttons Component - Pass, SuperLike, Like, Rewind buttons for discover
import React from 'react';
import { View, TouchableOpacity, Text, StyleSheet, Alert } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';

interface SwipeActionButtonsProps {
    onPass: () => void;
    onSuperLike: () => void;
    onLike: () => void;
    onRewind: () => void;
    rewindsRemaining?: number;
    disabled?: boolean;
}

export default function SwipeActionButtons({
    onPass,
    onSuperLike,
    onLike,
    onRewind,
    rewindsRemaining = 0,
    disabled = false,
}: SwipeActionButtonsProps) {

    const handleRewind = () => {
        if (rewindsRemaining <= 0) {
            Alert.alert(
                'No Rewinds Left',
                'Upgrade to Premium for more rewinds!',
                [{ text: 'OK' }]
            );
            return;
        }
        onRewind();
    };

    return (
        <View style={styles.container}>
            {/* Rewind */}
            <TouchableOpacity
                style={[styles.actionButton, styles.rewindButton]}
                onPress={handleRewind}
                disabled={disabled}
            >
                <Ionicons
                    name="arrow-undo"
                    size={24}
                    color={rewindsRemaining > 0 ? '#F59E0B' : Colors.dark.textMuted}
                />
                {rewindsRemaining > 0 && (
                    <View style={styles.rewindBadge}>
                        <Text style={styles.rewindCount}>{rewindsRemaining}</Text>
                    </View>
                )}
            </TouchableOpacity>

            {/* Pass */}
            <TouchableOpacity
                style={[styles.actionButton, styles.passButton]}
                onPress={onPass}
                disabled={disabled}
            >
                <Ionicons name="close" size={32} color="#EF4444" />
            </TouchableOpacity>

            {/* Super Like */}
            <TouchableOpacity
                style={[styles.actionButton, styles.superLikeButton]}
                onPress={onSuperLike}
                disabled={disabled}
            >
                <LinearGradient
                    colors={['#3B82F6', '#2563EB']}
                    style={styles.superLikeGradient}
                >
                    <Ionicons name="star" size={24} color={Colors.white} />
                </LinearGradient>
            </TouchableOpacity>

            {/* Like */}
            <TouchableOpacity
                style={[styles.actionButton, styles.likeButton]}
                onPress={onLike}
                disabled={disabled}
            >
                <LinearGradient
                    colors={['#22C55E', '#16A34A']}
                    style={styles.likeGradient}
                >
                    <Ionicons name="heart" size={32} color={Colors.white} />
                </LinearGradient>
            </TouchableOpacity>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flexDirection: 'row',
        justifyContent: 'center',
        alignItems: 'center',
        paddingVertical: 20,
        paddingHorizontal: 16,
        gap: 16,
    },
    actionButton: {
        justifyContent: 'center',
        alignItems: 'center',
        borderRadius: 50,
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.15,
        shadowRadius: 8,
        elevation: 4,
    },
    rewindButton: {
        width: 48,
        height: 48,
        backgroundColor: Colors.dark.surface,
        borderWidth: 2,
        borderColor: 'rgba(245, 158, 11, 0.3)',
    },
    passButton: {
        width: 60,
        height: 60,
        backgroundColor: Colors.dark.surface,
        borderWidth: 3,
        borderColor: '#EF4444',
    },
    superLikeButton: {
        width: 52,
        height: 52,
        overflow: 'hidden',
    },
    superLikeGradient: {
        width: '100%',
        height: '100%',
        justifyContent: 'center',
        alignItems: 'center',
        borderRadius: 26,
    },
    likeButton: {
        width: 64,
        height: 64,
        overflow: 'hidden',
    },
    likeGradient: {
        width: '100%',
        height: '100%',
        justifyContent: 'center',
        alignItems: 'center',
        borderRadius: 32,
    },
    rewindBadge: {
        position: 'absolute',
        top: -4,
        right: -4,
        backgroundColor: '#F59E0B',
        width: 18,
        height: 18,
        borderRadius: 9,
        justifyContent: 'center',
        alignItems: 'center',
    },
    rewindCount: {
        color: Colors.white,
        fontSize: 10,
        fontWeight: '700',
    },
});
