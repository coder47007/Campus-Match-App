import React, { useEffect, useState } from 'react';
import {
    View,
    Text,
    StyleSheet,
    FlatList,
    TouchableOpacity,
    Image,
    ActivityIndicator,
    Alert,
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import { reportsApi } from '@/services';
import { BlockedUserDto } from '@/types';
import Colors from '@/constants/Colors';

export default function BlockedUsersScreen() {
    const [blockedUsers, setBlockedUsers] = useState<BlockedUserDto[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        fetchBlockedUsers();
    }, []);

    const fetchBlockedUsers = async () => {
        setIsLoading(true);
        setError(null);
        try {
            const users = await reportsApi.getBlockedUsers();
            setBlockedUsers(users);
        } catch (err: unknown) {
            const errorMessage = err instanceof Error ? err.message : 'Failed to load blocked users';
            setError(errorMessage);
        } finally {
            setIsLoading(false);
        }
    };

    const handleUnblock = (userId: number, userName: string) => {
        Alert.alert(
            'Unblock User',
            `Are you sure you want to unblock ${userName}?`,
            [
                { text: 'Cancel', style: 'cancel' },
                {
                    text: 'Unblock',
                    onPress: async () => {
                        try {
                            await reportsApi.unblockUser(userId);
                            setBlockedUsers(blockedUsers.filter(u => u.id !== userId));
                            Alert.alert('Success', `${userName} has been unblocked`);
                        } catch (err) {
                            Alert.alert('Error', 'Failed to unblock user');
                        }
                    },
                },
            ]
        );
    };

    const formatDate = (dateString: string): string => {
        const date = new Date(dateString);
        return date.toLocaleDateString([], { month: 'short', day: 'numeric', year: 'numeric' });
    };

    if (isLoading) {
        return (
            <View style={[styles.container, styles.centerContent]}>
                <ActivityIndicator size="large" color={Colors.primary.main} />
            </View>
        );
    }

    if (error) {
        return (
            <View style={[styles.container, styles.centerContent]}>
                <Text style={styles.errorText}>{error}</Text>
                <TouchableOpacity style={styles.retryButton} onPress={fetchBlockedUsers}>
                    <Text style={styles.retryButtonText}>Try Again</Text>
                </TouchableOpacity>
            </View>
        );
    }

    if (blockedUsers.length === 0) {
        return (
            <View style={[styles.container, styles.centerContent]}>
                <Ionicons name="ban-outline" size={64} color={Colors.dark.textMuted} />
                <Text style={styles.emptyTitle}>No Blocked Users</Text>
                <Text style={styles.emptySubtitle}>
                    Users you block will appear here
                </Text>
            </View>
        );
    }

    return (
        <View style={styles.container}>
            <FlatList
                data={blockedUsers}
                keyExtractor={(item) => item.id.toString()}
                contentContainerStyle={styles.list}
                renderItem={({ item }) => (
                    <View style={styles.userItem}>
                        {item.photoUrl ? (
                            <Image
                                source={{ uri: item.photoUrl }}
                                style={styles.avatar}
                                resizeMode="cover"
                            />
                        ) : (
                            <View style={[styles.avatar, styles.noAvatar]}>
                                <Ionicons name="person" size={24} color={Colors.dark.textMuted} />
                            </View>
                        )}

                        <View style={styles.userInfo}>
                            <Text style={styles.userName}>{item.name}</Text>
                            <Text style={styles.blockedDate}>
                                Blocked on {formatDate(item.blockedAt)}
                            </Text>
                        </View>

                        <TouchableOpacity
                            style={styles.unblockButton}
                            onPress={() => handleUnblock(item.id, item.name)}
                        >
                            <Text style={styles.unblockButtonText}>Unblock</Text>
                        </TouchableOpacity>
                    </View>
                )}
            />
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: Colors.dark.background,
    },
    centerContent: {
        justifyContent: 'center',
        alignItems: 'center',
        padding: 40,
    },
    list: {
        padding: 16,
    },
    userItem: {
        flexDirection: 'row',
        alignItems: 'center',
        backgroundColor: Colors.dark.surface,
        padding: 16,
        borderRadius: 12,
        marginBottom: 12,
    },
    avatar: {
        width: 50,
        height: 50,
        borderRadius: 25,
    },
    noAvatar: {
        backgroundColor: Colors.dark.card,
        justifyContent: 'center',
        alignItems: 'center',
    },
    userInfo: {
        flex: 1,
        marginLeft: 12,
    },
    userName: {
        fontSize: 16,
        fontWeight: '600',
        color: Colors.dark.text,
    },
    blockedDate: {
        fontSize: 13,
        color: Colors.dark.textMuted,
        marginTop: 2,
    },
    unblockButton: {
        paddingHorizontal: 14,
        paddingVertical: 8,
        backgroundColor: 'rgba(255, 107, 107, 0.1)',
        borderRadius: 8,
    },
    unblockButtonText: {
        color: Colors.primary.main,
        fontSize: 14,
        fontWeight: '600',
    },
    emptyTitle: {
        marginTop: 24,
        fontSize: 20,
        fontWeight: 'bold',
        color: Colors.dark.text,
    },
    emptySubtitle: {
        marginTop: 8,
        fontSize: 14,
        color: Colors.dark.textSecondary,
        textAlign: 'center',
    },
    errorText: {
        fontSize: 16,
        color: Colors.error,
        textAlign: 'center',
    },
    retryButton: {
        marginTop: 16,
        paddingHorizontal: 24,
        paddingVertical: 12,
        backgroundColor: Colors.primary.main,
        borderRadius: 8,
    },
    retryButtonText: {
        color: Colors.white,
        fontSize: 16,
        fontWeight: '600',
    },
});
