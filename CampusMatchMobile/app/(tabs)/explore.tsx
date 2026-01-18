import React, { useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    ScrollView,
    TouchableOpacity,
    Image,
    SafeAreaView,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import Colors from '@/constants/Colors';

// Mock data for campus spots
const trendingSpots = [
    {
        id: 1,
        name: 'The Daily Grind',
        type: 'Coffee Shop',
        studentCount: 25,
        image: 'https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=400',
    },
    {
        id: 2,
        name: 'Main Library, 4th Floor',
        type: 'Quiet Study',
        studentCount: 42,
        image: 'https://images.unsplash.com/photo-1521587760476-6c12a4b040da?w=400',
    },
    {
        id: 3,
        name: 'Student Union',
        type: 'Social Hub',
        studentCount: 78,
        image: 'https://images.unsplash.com/photo-1523050854058-8df90110c9f1?w=400',
    },
];

// Mock data for campus events
const campusEvents = [
    {
        id: 1,
        name: 'Trivia Night @ The Pub',
        location: 'Deen',
        date: new Date(2024, 11, 23),
        time: '11:30 pm',
        image: 'https://images.unsplash.com/photo-1543269865-cbf427effbad?w=400',
    },
    {
        id: 2,
        name: 'Game Day Tailgate',
        location: 'Deen',
        date: new Date(2025, 0, 25),
        time: '11:30 am',
        image: 'https://images.unsplash.com/photo-1461896836934- voices-28da8dc?w=400',
    },
    {
        id: 3,
        name: 'Study Group Meetup',
        location: 'Library',
        date: new Date(2025, 0, 15),
        time: '2:00 pm',
        image: 'https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=400',
    },
];

export default function ExploreScreen() {
    const formatDate = (date: Date) => {
        const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        return {
            day: days[date.getDay()],
            date: date.getDate(),
            month: months[date.getMonth()],
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
                    <TouchableOpacity style={styles.headerIcon}>
                        <Ionicons name="options-outline" size={22} color={Colors.white} />
                    </TouchableOpacity>
                    <View style={styles.headerTitleContainer}>
                        <Text style={styles.headerTitle}>Explore</Text>
                    </View>
                    <TouchableOpacity style={styles.headerIcon}>
                        <Ionicons name="person-outline" size={22} color={Colors.white} />
                        <View style={styles.notificationDot} />
                    </TouchableOpacity>
                </View>

                <ScrollView showsVerticalScrollIndicator={false} contentContainerStyle={styles.content}>
                    {/* Trending Spots Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>Trending Spots</Text>
                        <ScrollView
                            horizontal
                            showsHorizontalScrollIndicator={false}
                            contentContainerStyle={styles.spotsContainer}
                        >
                            {trendingSpots.map((spot) => (
                                <TouchableOpacity key={spot.id} style={styles.spotCard}>
                                    <Image
                                        source={{ uri: spot.image }}
                                        style={styles.spotImage}
                                        resizeMode="cover"
                                    />
                                    <LinearGradient
                                        colors={['transparent', 'rgba(0,0,0,0.8)']}
                                        style={styles.spotGradient}
                                    />
                                    <View style={styles.spotInfo}>
                                        <Text style={styles.spotName}>{spot.name}</Text>
                                        <Text style={styles.spotStudents}>
                                            • {spot.studentCount} students here
                                        </Text>
                                    </View>
                                </TouchableOpacity>
                            ))}
                        </ScrollView>
                    </View>

                    {/* Campus Events Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>Campus Events</Text>
                        {campusEvents.map((event) => {
                            const dateInfo = formatDate(event.date);
                            return (
                                <TouchableOpacity key={event.id} style={styles.eventCard}>
                                    <View style={styles.eventDateBadge}>
                                        <Text style={styles.eventDay}>{dateInfo.day}</Text>
                                        <Text style={styles.eventDate}>{dateInfo.date}</Text>
                                        <Text style={styles.eventMonth}>{dateInfo.month}</Text>
                                    </View>
                                    <Image
                                        source={{ uri: event.image }}
                                        style={styles.eventImage}
                                        resizeMode="cover"
                                    />
                                    <View style={styles.eventInfo}>
                                        <Text style={styles.eventName}>{event.name}</Text>
                                        <Text style={styles.eventLocation}>
                                            {event.location} • {event.time}
                                        </Text>
                                    </View>
                                </TouchableOpacity>
                            );
                        })}
                    </View>
                </ScrollView>
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
    notificationDot: {
        position: 'absolute',
        top: 8,
        right: 8,
        width: 8,
        height: 8,
        borderRadius: 4,
        backgroundColor: '#EF4444',
    },
    content: {
        paddingBottom: 100,
    },
    section: {
        marginTop: 20,
    },
    sectionTitle: {
        fontSize: 20,
        fontWeight: '700',
        color: Colors.white,
        paddingHorizontal: 16,
        marginBottom: 16,
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
        width: 60,
        height: 60,
        borderRadius: 12,
    },
    eventInfo: {
        flex: 1,
    },
    eventName: {
        fontSize: 15,
        fontWeight: '600',
        color: Colors.white,
        marginBottom: 4,
    },
    eventLocation: {
        fontSize: 13,
        color: 'rgba(255,255,255,0.6)',
    },
});
