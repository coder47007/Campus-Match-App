// Event Card Component - Displays a campus event/vibe
import React from 'react';
import { View, Text, TouchableOpacity, StyleSheet, Image, Alert } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';

export interface EventData {
    id: number;
    title: string;
    location: string;
    startTime: string;
    creatorId: number;
    creator?: {
        name: string;
        photoUrl?: string;
    };
}

interface DateInfo {
    day: string;
    month: string;
    time: string;
    isToday: boolean;
    isTomorrow: boolean;
}

interface EventCardProps {
    event: EventData;
    dateInfo: DateInfo;
    isCreator?: boolean;
    onPress: () => void;
    onDelete?: () => void;
}

export default function EventCard({
    event,
    dateInfo,
    isCreator = false,
    onPress,
    onDelete
}: EventCardProps) {

    const handleDelete = () => {
        Alert.alert(
            'Delete Event',
            'Are you sure you want to delete this event?',
            [
                { text: 'Cancel', style: 'cancel' },
                {
                    text: 'Delete',
                    style: 'destructive',
                    onPress: onDelete
                },
            ]
        );
    };

    return (
        <TouchableOpacity
            style={styles.eventCard}
            onPress={onPress}
            activeOpacity={0.8}
        >
            <LinearGradient
                colors={['#2D2D44', '#1E1E2E']}
                style={styles.eventGradient}
            >
                <View style={styles.eventDate}>
                    <Text style={styles.eventDay}>{dateInfo.day}</Text>
                    <Text style={styles.eventMonth}>{dateInfo.month}</Text>
                    {(dateInfo.isToday || dateInfo.isTomorrow) && (
                        <View style={[
                            styles.todayBadge,
                            dateInfo.isTomorrow && styles.tomorrowBadge
                        ]}>
                            <Text style={styles.todayText}>
                                {dateInfo.isToday ? 'Today' : 'Tomorrow'}
                            </Text>
                        </View>
                    )}
                </View>

                <View style={styles.eventInfo}>
                    <Text style={styles.eventTitle} numberOfLines={2}>{event.title}</Text>
                    <View style={styles.eventMeta}>
                        <Ionicons name="location-outline" size={14} color="rgba(255,255,255,0.6)" />
                        <Text style={styles.eventLocation} numberOfLines={1}>{event.location}</Text>
                    </View>
                    <View style={styles.eventMeta}>
                        <Ionicons name="time-outline" size={14} color="rgba(255,255,255,0.6)" />
                        <Text style={styles.eventTime}>{dateInfo.time}</Text>
                    </View>
                </View>

                <View style={styles.eventActions}>
                    {event.creator?.photoUrl ? (
                        <Image
                            source={{ uri: event.creator.photoUrl }}
                            style={styles.creatorAvatar}
                        />
                    ) : (
                        <View style={[styles.creatorAvatar, styles.creatorAvatarPlaceholder]}>
                            <Ionicons name="person" size={16} color={Colors.white} />
                        </View>
                    )}

                    {isCreator && onDelete && (
                        <TouchableOpacity
                            style={styles.deleteButton}
                            onPress={handleDelete}
                        >
                            <Ionicons name="trash-outline" size={18} color="#EF4444" />
                        </TouchableOpacity>
                    )}
                </View>
            </LinearGradient>
        </TouchableOpacity>
    );
}

const styles = StyleSheet.create({
    eventCard: {
        marginBottom: 12,
        borderRadius: 16,
        overflow: 'hidden',
    },
    eventGradient: {
        flexDirection: 'row',
        padding: 16,
        alignItems: 'center',
        borderRadius: 16,
        borderWidth: 1,
        borderColor: 'rgba(124, 58, 237, 0.2)',
    },
    eventDate: {
        width: 56,
        alignItems: 'center',
        marginRight: 16,
    },
    eventDay: {
        fontSize: 24,
        fontWeight: '800',
        color: Colors.white,
    },
    eventMonth: {
        fontSize: 12,
        color: 'rgba(255,255,255,0.6)',
        textTransform: 'uppercase',
    },
    todayBadge: {
        marginTop: 4,
        backgroundColor: '#22C55E',
        paddingHorizontal: 6,
        paddingVertical: 2,
        borderRadius: 4,
    },
    tomorrowBadge: {
        backgroundColor: '#7C3AED',
    },
    todayText: {
        fontSize: 9,
        fontWeight: '700',
        color: Colors.white,
    },
    eventInfo: {
        flex: 1,
        marginRight: 12,
    },
    eventTitle: {
        fontSize: 16,
        fontWeight: '700',
        color: Colors.white,
        marginBottom: 6,
    },
    eventMeta: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 4,
        marginBottom: 2,
    },
    eventLocation: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
        flex: 1,
    },
    eventTime: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
    },
    eventActions: {
        alignItems: 'center',
        gap: 8,
    },
    creatorAvatar: {
        width: 36,
        height: 36,
        borderRadius: 18,
    },
    creatorAvatarPlaceholder: {
        backgroundColor: Colors.dark.surface,
        justifyContent: 'center',
        alignItems: 'center',
    },
    deleteButton: {
        padding: 4,
    },
});
