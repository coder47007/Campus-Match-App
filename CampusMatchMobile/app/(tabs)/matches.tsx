import React, { useEffect, useState, useCallback } from 'react';
import {
    View,
    Text,
    StyleSheet,
    FlatList,
    TouchableOpacity,
    Image,
    ActivityIndicator,
    RefreshControl,
    ScrollView,
    SafeAreaView,
} from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import { useMatchStore } from '@/stores/matchStore';
import { MatchDto } from '@/types';
import Colors from '@/constants/Colors';
import EmptyState from '@/components/ui/EmptyState';

export default function MatchesScreen() {
    const router = useRouter();
    const { matches, isLoading, error, fetchMatches } = useMatchStore();
    const [refreshing, setRefreshing] = useState(false);

    useEffect(() => {
        fetchMatches();
    }, []);

    const onRefresh = useCallback(async () => {
        setRefreshing(true);
        await fetchMatches();
        setRefreshing(false);
    }, [fetchMatches]);

    // Separate new matches (no messages) and conversations (with messages)
    const newMatches = matches.filter(m => !m.lastMessage);
    const conversations = matches.filter(m => m.lastMessage);

    const renderNewMatch = ({ item }: { item: MatchDto }) => (
        <TouchableOpacity
            style={styles.newMatchItem}
            onPress={() => router.push(`/chat/${item.id}`)}
        >
            <LinearGradient
                colors={['#7C3AED', '#6D28D9']}
                style={styles.newMatchRing}
            >
                {item.otherStudentPhotoUrl ? (
                    <Image
                        source={{ uri: item.otherStudentPhotoUrl }}
                        style={styles.newMatchPhoto}
                    />
                ) : (
                    <View style={[styles.newMatchPhoto, styles.noPhoto]}>
                        <Ionicons name="person" size={24} color={Colors.white} />
                    </View>
                )}
            </LinearGradient>
            {!item.lastMessage && (
                <View style={styles.newBadge}>
                    <Text style={styles.newBadgeText}>New</Text>
                </View>
            )}
            <Text style={styles.newMatchName} numberOfLines={1}>
                {item.otherStudentName?.split(' ')[0]}
            </Text>
        </TouchableOpacity>
    );

    const renderConversation = ({ item }: { item: MatchDto }) => {
        const isNew = item.lastMessage && !item.lastMessage.includes('You:');

        return (
            <TouchableOpacity
                style={styles.conversationItem}
                onPress={() => router.push(`/chat/${item.id}`)}
            >
                <View style={styles.avatarContainer}>
                    {item.otherStudentPhotoUrl ? (
                        <Image
                            source={{ uri: item.otherStudentPhotoUrl }}
                            style={styles.conversationPhoto}
                        />
                    ) : (
                        <View style={[styles.conversationPhoto, styles.noPhoto]}>
                            <Ionicons name="person" size={28} color={Colors.white} />
                        </View>
                    )}
                </View>
                <View style={styles.conversationInfo}>
                    <View style={styles.conversationHeader}>
                        <Text style={styles.conversationName}>
                            {item.otherStudentName}
                        </Text>
                        {isNew && (
                            <View style={styles.newMessageBadge}>
                                <Text style={styles.newMessageText}>New</Text>
                            </View>
                        )}
                    </View>
                    <Text style={styles.conversationPreview} numberOfLines={1}>
                        {item.lastMessage || 'New match! Say hello 👋'}
                    </Text>
                </View>
            </TouchableOpacity>
        );
    };

    if (isLoading && matches.length === 0) {
        return (
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={styles.container}
            >
                <SafeAreaView style={styles.safeArea}>
                    <View style={styles.centerContent}>
                        <ActivityIndicator size="large" color="#7C3AED" />
                    </View>
                </SafeAreaView>
            </LinearGradient>
        );
    }

    if (matches.length === 0) {
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
                            onPress={() => router.push('/settings')}
                            accessible={true}
                            accessibilityLabel="Settings"
                        >
                            <Ionicons name="options-outline" size={22} color={Colors.white} />
                        </TouchableOpacity>
                        <View style={styles.headerTitleContainer}>
                            <Text style={styles.headerTitle}>Matches & Messages</Text>
                        </View>
                        <TouchableOpacity
                            style={styles.headerIcon}
                            onPress={() => router.push('/(tabs)/profile')}
                            accessible={true}
                            accessibilityLabel="View profile"
                        >
                            <Ionicons name="person-outline" size={22} color={Colors.white} />
                        </TouchableOpacity>
                    </View>

                    <EmptyState
                        icon="chatbubbles-outline"
                        title="No Matches Yet"
                        description="Keep swiping to find your perfect match!"
                        action={{
                            label: "Start Swiping",
                            onPress: () => router.push('/(tabs)/discover')
                        }}
                    />
                </SafeAreaView>
            </LinearGradient>
        );
    }

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
                        onPress={() => router.push('/settings')}
                        accessible={true}
                        accessibilityLabel="Settings"
                    >
                        <Ionicons name="options-outline" size={22} color={Colors.white} />
                    </TouchableOpacity>
                    <View style={styles.headerTitleContainer}>
                        <Text style={styles.headerTitle}>Matches & Messages</Text>
                    </View>
                    <TouchableOpacity
                        style={styles.headerIcon}
                        onPress={() => router.push('/(tabs)/profile')}
                        accessible={true}
                        accessibilityLabel="View profile"
                    >
                        <Ionicons name="person-outline" size={22} color={Colors.white} />
                    </TouchableOpacity>
                </View>

                <ScrollView
                    showsVerticalScrollIndicator={false}
                    refreshControl={
                        <RefreshControl refreshing={refreshing} onRefresh={onRefresh} tintColor="#7C3AED" />
                    }
                >
                    {/* New Matches Section */}
                    {newMatches.length > 0 && (
                        <View style={styles.section}>
                            <Text style={styles.sectionTitle}>New Matches</Text>
                            <FlatList
                                data={newMatches}
                                renderItem={renderNewMatch}
                                keyExtractor={(item) => item.id.toString()}
                                horizontal
                                showsHorizontalScrollIndicator={false}
                                contentContainerStyle={styles.newMatchesContainer}
                            />
                        </View>
                    )}

                    {/* Messages Section */}
                    <View style={styles.section}>
                        <Text style={styles.sectionTitle}>Messages</Text>
                        {conversations.length > 0 ? (
                            conversations.map((item) => (
                                <View key={item.id}>
                                    {renderConversation({ item })}
                                </View>
                            ))
                        ) : (
                            <View style={styles.noMessages}>
                                <Text style={styles.noMessagesText}>
                                    Start a conversation with your matches!
                                </Text>
                            </View>
                        )}
                    </View>
                </ScrollView>
            </SafeAreaView>
        </LinearGradient >
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    safeArea: {
        flex: 1,
    },
    centerContent: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        padding: 24,
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
        paddingHorizontal: 20,
        paddingVertical: 10,
        borderRadius: 20,
    },
    headerTitle: {
        fontSize: 14,
        fontWeight: '600',
        color: Colors.white,
    },
    section: {
        marginTop: 16,
    },
    sectionTitle: {
        fontSize: 18,
        fontWeight: '700',
        color: Colors.white,
        paddingHorizontal: 16,
        marginBottom: 16,
    },
    newMatchesContainer: {
        paddingHorizontal: 16,
        gap: 16,
    },
    newMatchItem: {
        alignItems: 'center',
        width: 70,
    },
    newMatchRing: {
        width: 64,
        height: 64,
        borderRadius: 32,
        padding: 2,
        justifyContent: 'center',
        alignItems: 'center',
    },
    newMatchPhoto: {
        width: 58,
        height: 58,
        borderRadius: 29,
        backgroundColor: Colors.dark.surface,
    },
    noPhoto: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    newBadge: {
        position: 'absolute',
        top: 44,
        backgroundColor: '#22C55E',
        paddingHorizontal: 6,
        paddingVertical: 2,
        borderRadius: 8,
    },
    newBadgeText: {
        fontSize: 9,
        fontWeight: '700',
        color: Colors.white,
    },
    newMatchName: {
        marginTop: 8,
        fontSize: 12,
        color: Colors.white,
        textAlign: 'center',
    },
    conversationItem: {
        flexDirection: 'row',
        alignItems: 'center',
        paddingHorizontal: 16,
        paddingVertical: 12,
        marginHorizontal: 16,
        marginBottom: 8,
        backgroundColor: Colors.dark.surface,
        borderRadius: 16,
    },
    avatarContainer: {
        position: 'relative',
    },
    conversationPhoto: {
        width: 52,
        height: 52,
        borderRadius: 26,
        backgroundColor: '#2A1F3D',
    },
    conversationInfo: {
        flex: 1,
        marginLeft: 12,
    },
    conversationHeader: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'space-between',
    },
    conversationName: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.white,
    },
    newMessageBadge: {
        backgroundColor: '#7C3AED',
        paddingHorizontal: 8,
        paddingVertical: 3,
        borderRadius: 10,
    },
    newMessageText: {
        fontSize: 10,
        fontWeight: '600',
        color: Colors.white,
    },
    conversationPreview: {
        marginTop: 4,
        fontSize: 14,
        color: 'rgba(255,255,255,0.6)',
    },
    noMessages: {
        paddingHorizontal: 16,
        paddingVertical: 24,
    },
    noMessagesText: {
        fontSize: 14,
        color: 'rgba(255,255,255,0.5)',
        textAlign: 'center',
    },
    emptyIcon: {
        width: 80,
        height: 80,
        borderRadius: 40,
        backgroundColor: 'rgba(124, 58, 237, 0.2)',
        justifyContent: 'center',
        alignItems: 'center',
        marginBottom: 16,
    },
    emptyTitle: {
        fontSize: 24,
        fontWeight: '700',
        color: Colors.white,
    },
    emptySubtitle: {
        fontSize: 14,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
        marginTop: 8,
        lineHeight: 22,
    },
});
