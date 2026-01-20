import React, { useState, useEffect, useCallback } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    TouchableOpacity,
    Image,
    SafeAreaView,
    RefreshControl,
    ActivityIndicator,
    Alert,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';
import { eventsApi, spotsApi } from '@/services';
import { TrendingSpot } from '@/services/spots';
import EventModal from '@/components/EventModal';
import SpotDetailModal from '@/components/SpotDetailModal';

// CampusEvent type
interface CampusEvent {
    Id: number;
    Title: string;
    Location: string;
    StartTime: string;
    CreatorId: number;
    Creator?: { Name: string; PhotoUrl?: string };
}


export default function ExploreScreen() {
    const [events, setEvents] = useState<CampusEvent[]>([]);
    const [spots, setSpots] = useState<TrendingSpot[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [showEventModal, setShowEventModal] = useState(false);
    const [showSpotModal, setShowSpotModal] = useState(false);
    const [selectedSpot, setSelectedSpot] = useState<TrendingSpot | null>(null);
    const [refreshing, setRefreshing] = useState(false);
    const [currentStudentId, setCurrentStudentId] = useState<number | null>(null);

    // Load current user ID to check ownership
    const loadUserId = async (forceRefresh = false) => {
        const { getStudentId, clearSessionCache } = await import('@/services/supabase');

        if (forceRefresh) {
            clearSessionCache();
        }

        const id = await getStudentId();
        console.log('ExploreScreen: Current Student ID is:', id);
        setCurrentStudentId(id);
    };

    useEffect(() => {
        loadUserId();
    }, []);

    const loadEvents = useCallback(async () => {
        try {
            const data = await eventsApi.getEvents();
            setEvents(data);
        } catch (err) {
            console.error(err);
        } finally {
            setIsLoading(false);
            setRefreshing(false);
        }
    }, []);

    const loadSpots = useCallback(async () => {
        try {
            const data = await spotsApi.getSpots();
            setSpots(data);
        } catch (err) {
            console.error('Error loading spots:', err);
        }
    }, []);

    useEffect(() => {
        loadEvents();
        loadSpots();
    }, [loadEvents, loadSpots]);

    const onRefresh = async () => {
        setRefreshing(true);
        // Force fully fresh user ID
        await loadUserId(true);
        loadEvents();
        loadSpots();
    };

    const handleDeleteEvent = async (eventId: number) => {
        Alert.alert(
            "Delete Vibe?",
            "Are you sure you want to remove this vibe?",
            [
                { text: "Cancel", style: "cancel" },
                {
                    text: "Delete",
                    style: "destructive",
                    onPress: async () => {
                        console.log('Deleting event:', eventId);
                        const success = await eventsApi.deleteEvent(eventId);
                        if (success) {
                            setEvents(prev => prev.filter(e => e.Id !== eventId));
                        } else {
                            Alert.alert('Error', 'Could not delete vibe. Please try again.');
                        }
                    }
                }
            ]
        );
    };

    const formatDate = (dateStr: string) => {
        const date = new Date(dateStr);
        const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

        let hours = date.getHours();
        const minutes = date.getMinutes();
        const ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12;
        const time = `${hours}:${minutes < 10 ? '0' + minutes : minutes} ${ampm}`;

        return {
            day: days[date.getDay()],
            date: date.getDate(),
            month: months[date.getMonth()],
            time
        };
    };

    return (
        <LinearGradient
            colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
            style={styles.container}
        >
            <SafeAreaView style={styles.safeArea}>
                {/* Header */}
                <View style={styles.header}>
                    <TouchableOpacity
                        style={styles.headerIcon}
                        onPress={() => Alert.alert('Filter', 'Filter options coming soon!')}
                    >
                        <Ionicons name="options-outline" size={22} color={Colors.white} />
                    </TouchableOpacity>
                    <View style={styles.headerTitleContainer}>
                        <Text style={styles.headerTitle}>Explore</Text>
                    </View>
                    <TouchableOpacity style={styles.createButton} onPress={() => setShowEventModal(true)}>
                        <LinearGradient
                            colors={['#7C3AED', '#6D28D9']}
                            style={styles.createButtonGradient}
                        >
                            <Ionicons name="add" size={24} color={Colors.white} />
                        </LinearGradient>
                    </TouchableOpacity>
                </View>

                <ScrollView
                    showsVerticalScrollIndicator={false}
                    contentContainerStyle={styles.content}
                    refreshControl={
                        <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor={Colors.primary.main} />
                    }
                >
                    {/* Trending Spots Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>Trending Spots</Text>
                        <ScrollView
                            horizontal
                            showsHorizontalScrollIndicator={false}
                            contentContainerStyle={styles.spotsScroll}
                        >
                            {spots.map((spot) => (
                                <TouchableOpacity
                                    key={spot.id}
                                    style={styles.spotCard}
                                    onPress={() => {
                                        setSelectedSpot(spot);
                                        setShowSpotModal(true);
                                    }}
                                >
                                    <Image
                                        source={{ uri: spot.imageUrl }}
                                        style={styles.spotImage}
                                        resizeMode="cover"
                                    />
                                    <LinearGradient
                                        colors={['transparent', 'rgba(0,0,0,0.8)']}
                                        style={styles.spotGradient}
                                    />
                                    <View style={styles.spotInfo}>
                                        <Text style={styles.spotName}>{spot.name}</Text>
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
                                </TouchableOpacity>
                            ))}
                        </ScrollView>
                    </View>

                    {/* Campus Events Section */}
                    <View style={styles.section}>
                        <View style={styles.sectionHeader}>
                            <Text style={styles.sectionTitle}>Campus Vibes</Text>
                            <TouchableOpacity onPress={() => setShowEventModal(true)}>
                                <Text style={styles.seeAll}>+ Add Vibe</Text>
                            </TouchableOpacity>
                        </View>

                        {isLoading && events.length === 0 ? (
                            <ActivityIndicator color={Colors.primary.main} style={{ marginTop: 20 }} />
                        ) : events.length === 0 ? (
                            <View style={styles.emptyState}>
                                <Ionicons name="planet-outline" size={48} color="rgba(255,255,255,0.3)" />
                                <Text style={styles.emptyText}>No vibes yet. Start one!</Text>
                            </View>
                        ) : (
                            events.map((event) => {
                                const dateInfo = formatDate(event.StartTime);
                                const isCreator = currentStudentId === event.CreatorId;

                                // Debug logging
                                if (!isCreator && currentStudentId === 1000) {
                                    // console.log('You are guest (1000), Event Creator:', event.CreatorId);
                                }

                                return (
                                    <TouchableOpacity
                                        key={event.Id}
                                        style={styles.eventCard}
                                        onPress={() => Alert.alert(
                                            event.Title,
                                            `📍 ${event.Location}\n🕐 ${dateInfo.time}\n👤 by ${event.Creator?.Name || 'Student'}`,
                                            [{ text: 'OK' }]
                                        )}
                                    >
                                        <View style={styles.eventDateBadge}>
                                            <Text style={styles.eventDay}>{dateInfo.day}</Text>
                                            <Text style={styles.eventDate}>{dateInfo.date}</Text>
                                            <Text style={styles.eventMonth}>{dateInfo.month}</Text>
                                        </View>

                                        {/* Creator Image or Default */}
                                        <Image
                                            source={{ uri: event.Creator?.PhotoUrl || 'https://images.unsplash.com/photo-1511632765486-a01980e01a18?w=400' }}
                                            style={styles.eventImage}
                                            resizeMode="cover"
                                        />

                                        <View style={styles.eventInfo}>
                                            <Text style={styles.eventName}>{event.Title}</Text>
                                            <Text style={styles.eventLocation}>
                                                📍 {event.Location} • {dateInfo.time}
                                            </Text>
                                            <Text style={styles.creatorName}>
                                                by {event.Creator?.Name || 'Student'}
                                            </Text>
                                        </View>

                                        {/* Delete button - only if creator */}
                                        {isCreator && (
                                            <TouchableOpacity
                                                style={styles.deleteButton}
                                                onPress={() => handleDeleteEvent(event.Id)}
                                            >
                                                <Ionicons name="trash-outline" size={20} color="#EF4444" />
                                            </TouchableOpacity>
                                        )}
                                    </TouchableOpacity>
                                );
                            })
                        )}
                    </View>
                </ScrollView>

                <EventModal
                    visible={showEventModal}
                    onClose={() => setShowEventModal(false)}
                    onEventCreated={loadEvents}
                />

                <SpotDetailModal
                    visible={showSpotModal}
                    spot={selectedSpot}
                    onClose={() => setShowSpotModal(false)}
                    onCheckInChange={() => {
                        loadSpots();
                    }}
                />
            </SafeAreaView>
        </LinearGradient>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    safeArea: {
        flex: 1,
    },
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
        backgroundColor: 'rgba(255,255,255,0.1)',
        paddingHorizontal: 24,
        paddingVertical: 10,
        borderRadius: 20,
    },
    headerTitle: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    createButton: {
        borderRadius: 20,
        overflow: 'hidden',
    },
    createButtonGradient: {
        width: 40,
        height: 40,
        justifyContent: 'center',
        alignItems: 'center',
    },
    content: {
        paddingBottom: 100,
    },
    section: {
        marginTop: 24,
    },
    sectionHeader: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        paddingHorizontal: 16,
        marginBottom: 16,
    },
    sectionTitle: {
        fontSize: 20,
        fontWeight: '700',
        color: Colors.white,
    },
    seeAll: {
        fontSize: 14,
        color: '#7C3AED',
        fontWeight: '600',
    },
    spotsContainer: {
        paddingHorizontal: 16,
        gap: 12,
    },
    spotCard: {
        width: 180,
        height: 140,
        borderRadius: 16,
        overflow: 'hidden',
        backgroundColor: Colors.dark.surface,
    },
    spotImage: {
        width: '100%',
        height: '100%',
        position: 'absolute',
    },
    spotGradient: {
        position: 'absolute',
        bottom: 0,
        left: 0,
        right: 0,
        height: '60%',
    },
    spotInfo: {
        position: 'absolute',
        bottom: 12,
        left: 12,
        right: 12,
    },
    spotName: {
        fontSize: 14,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 4,
    },
    spotStudents: {
        fontSize: 12,
        color: 'rgba(255,255,255,0.7)',
    },
    eventCard: {
        flexDirection: 'row',
        alignItems: 'center',
        marginHorizontal: 16,
        marginBottom: 12,
        backgroundColor: Colors.dark.surface,
        borderRadius: 16,
        overflow: 'hidden',
        padding: 12,
        gap: 12,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.05)',
    },
    eventDateBadge: {
        width: 50,
        backgroundColor: '#2A1F3D',
        borderRadius: 12,
        paddingVertical: 8,
        alignItems: 'center',
    },
    eventDay: {
        fontSize: 10,
        fontWeight: '500',
        color: '#7C3AED',
    },
    eventDate: {
        fontSize: 20,
        fontWeight: '700',
        color: Colors.white,
    },
    eventMonth: {
        fontSize: 10,
        fontWeight: '500',
        color: 'rgba(255,255,255,0.5)',
    },
    eventImage: {
        width: 50,
        height: 50,
        borderRadius: 25,
        borderWidth: 2,
        borderColor: '#7C3AED',
    },
    eventInfo: {
        flex: 1,
    },
    eventName: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 4,
    },
    eventLocation: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
        marginBottom: 2,
    },
    creatorName: {
        fontSize: 11,
        color: '#7C3AED',
    },
    emptyState: {
        alignItems: 'center',
        justifyContent: 'center',
        padding: 40,
        opacity: 0.7,
    },
    emptyText: {
        color: 'rgba(255,255,255,0.5)',
        marginTop: 12,
    },
    deleteButton: {
        padding: 8,
    },
    spotsScroll: {
        paddingRight: 16,
    },
    spotStudentsRow: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 4,
    },
    checkedInBadge: {
        marginLeft: 4,
    },
});
