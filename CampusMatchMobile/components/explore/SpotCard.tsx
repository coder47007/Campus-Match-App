// Spot Card Component - Displays a trending campus spot
import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet, ImageBackground } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';

export interface SpotData {
    id: number;
    name: string;
    imageUrl: string;
    studentCount: number;
    isCheckedIn?: boolean;
}

interface SpotCardProps {
    spot: SpotData;
    onPress: () => void;
}

export default function SpotCard({ spot, onPress }: SpotCardProps) {
    return (
        <TouchableOpacity
            style={styles.spotCard}
            onPress={onPress}
            activeOpacity={0.8}
        >
            <ImageBackground
                source={{ uri: spot.imageUrl }}
                style={styles.spotImage}
                imageStyle={styles.spotImageStyle}
            >
                <LinearGradient
                    colors={['transparent', 'rgba(0,0,0,0.8)']}
                    style={styles.spotGradient}
                />
                <View style={styles.spotInfo}>
                    <Text style={styles.spotName} numberOfLines={1}>{spot.name}</Text>
                    <View style={styles.spotStudentsRow}>
                        <Ionicons name="people" size={14} color="#7C3AED" />
                        <Text style={styles.spotStudents}>
                            {spot.studentCount} {spot.studentCount === 1 ? 'student' : 'students'}
                        </Text>
                        {spot.isCheckedIn && (
                            <View style={styles.checkedInBadge}>
                                <Ionicons name="checkmark-circle" size={14} color="#22C55E" />
                            </View>
                        )}
                    </View>
                </View>
            </ImageBackground>
        </TouchableOpacity>
    );
}

const styles = StyleSheet.create({
    spotCard: {
        width: 140,
        height: 180,
        marginRight: 12,
        borderRadius: 16,
        overflow: 'hidden',
    },
    spotImage: {
        flex: 1,
        justifyContent: 'flex-end',
    },
    spotImageStyle: {
        borderRadius: 16,
    },
    spotGradient: {
        ...StyleSheet.absoluteFillObject,
        borderRadius: 16,
    },
    spotInfo: {
        padding: 12,
    },
    spotName: {
        fontSize: 14,
        fontWeight: '700',
        color: Colors.white,
        marginBottom: 4,
    },
    spotStudentsRow: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 4,
    },
    spotStudents: {
        fontSize: 12,
        color: 'rgba(255,255,255,0.8)',
    },
    checkedInBadge: {
        marginLeft: 4,
    },
});
