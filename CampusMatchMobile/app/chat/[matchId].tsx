import React, { useEffect, useState, useRef } from 'react';
import {
    View,
    Text,
    StyleSheet,
    FlatList,
    TextInput,
    TouchableOpacity,
    KeyboardAvoidingView,
    Platform,
    Image,
    ActivityIndicator,
    Alert,
    Animated,
} from 'react-native';
import { useLocalSearchParams, useRouter, Stack } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';
import { LinearGradient } from 'expo-linear-gradient';
import EmojiPicker from 'rn-emoji-keyboard';
import * as ImagePicker from 'expo-image-picker';
import GifPicker from '@/components/GifPicker';
import LocationPicker from '@/components/LocationPicker';
import { useChatStore } from '@/stores/chatStore';
import { useMatchStore } from '@/stores/matchStore';
import { useAuthStore } from '@/stores/authStore';
import { signalRService } from '@/services/signalr';
import { matchesApi } from '@/services/matches';
import { MessageDto } from '@/types';
import Colors from '@/constants/Colors';

export default function ChatScreen() {
    const { matchId } = useLocalSearchParams<{ matchId: string }>();
    const router = useRouter();
    const flatListRef = useRef<FlatList>(null);

    const { user } = useAuthStore();
    const { matches } = useMatchStore();
    const {
        messages,
        typingUsers,
        isLoading,
        isSending,
        fetchMessages,
        sendMessage,
        addMessage,
        setTyping,
    } = useChatStore();

    const [inputText, setInputText] = useState('');
    const [isTyping, setIsTyping] = useState(false);
    const [showEmojiPicker, setShowEmojiPicker] = useState(false);
    const [showGifPicker, setShowGifPicker] = useState(false);
    const [showLocationPicker, setShowLocationPicker] = useState(false);
    const typingTimeoutRef = useRef<NodeJS.Timeout>();
    const sendButtonScale = useRef(new Animated.Value(1)).current;

    const matchIdNum = parseInt(matchId || '0');
    const match = matches.find(m => m.id === matchIdNum);
    const chatMessages = messages[matchIdNum] || [];
    const isOtherTyping = typingUsers[matchIdNum];

    useEffect(() => {
        if (matchIdNum) {
            fetchMessages(matchIdNum);
        }
    }, [matchIdNum]);

    useEffect(() => {
        const unsubMessage = signalRService.onMessage((message: MessageDto) => {
            if (message.senderId === match?.otherStudentId) {
                addMessage(matchIdNum, message);
            }
        });

        const unsubTyping = signalRService.onTyping((userId: number, typing: boolean) => {
            if (userId === match?.otherStudentId) {
                setTyping(matchIdNum, typing);
            }
        });

        return () => {
            unsubMessage();
            unsubTyping();
        };
    }, [matchIdNum, match?.otherStudentId]);

    useEffect(() => {
        if (chatMessages.length > 0) {
            setTimeout(() => {
                flatListRef.current?.scrollToEnd({ animated: true });
            }, 100);
        }
    }, [chatMessages.length]);

    const animateSendButton = () => {
        Animated.sequence([
            Animated.spring(sendButtonScale, {
                toValue: 0.85,
                useNativeDriver: true,
            }),
            Animated.spring(sendButtonScale, {
                toValue: 1,
                useNativeDriver: true,
            }),
        ]).start();
    };

    const handleSend = async () => {
        if (!inputText.trim() || isSending) return;

        animateSendButton();
        const text = inputText.trim();
        setInputText('');

        try {
            await sendMessage(matchIdNum, text);

            if (match?.otherStudentId) {
                signalRService.sendTypingIndicator(match.otherStudentId, false);
            }
        } catch (err) {
            Alert.alert('Error', 'Failed to send message');
            setInputText(text);
        }
    };

    const handleTextChange = (text: string) => {
        setInputText(text);

        if (match?.otherStudentId) {
            if (!isTyping) {
                setIsTyping(true);
                signalRService.sendTypingIndicator(match.otherStudentId, true);
            }

            if (typingTimeoutRef.current) {
                clearTimeout(typingTimeoutRef.current);
            }

            typingTimeoutRef.current = setTimeout(() => {
                setIsTyping(false);
                signalRService.sendTypingIndicator(match.otherStudentId, false);
            }, 2000);
        }
    };

    const formatTime = (dateString: string): string => {
        const date = new Date(dateString);
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    };

    const formatMessageDate = (dateString: string): string => {
        const date = new Date(dateString);
        const now = new Date();
        const diffDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24));

        if (diffDays === 0) return 'Today';
        if (diffDays === 1) return 'Yesterday';
        return date.toLocaleDateString([], { month: 'short', day: 'numeric' });
    };

    const shouldShowDate = (index: number): boolean => {
        if (index === 0) return true;
        const currentDate = new Date(chatMessages[index].sentAt).toDateString();
        const prevDate = new Date(chatMessages[index - 1].sentAt).toDateString();
        return currentDate !== prevDate;
    };

    const renderMessage = ({ item, index }: { item: MessageDto; index: number }) => {
        const isMine = item.senderId === user?.id;
        const showDate = shouldShowDate(index);

        return (
            <>
                {showDate && (
                    <View style={styles.dateContainer}>
                        <View style={styles.dateLine} />
                        <Text style={styles.dateText}>{formatMessageDate(item.sentAt)}</Text>
                        <View style={styles.dateLine} />
                    </View>
                )}
                <View style={[styles.messageRow, isMine && styles.myMessageRow]}>
                    <View style={[styles.messageBubble, isMine ? styles.myBubble : styles.theirBubble]}>
                        {isMine ? (
                            <LinearGradient
                                colors={['#FF6B6B', '#FF8E53']}
                                start={{ x: 0, y: 0 }}
                                end={{ x: 1, y: 1 }}
                                style={styles.myBubbleGradient}
                            >
                                <Text style={styles.myMessageText}>{item.content}</Text>
                            </LinearGradient>
                        ) : (
                            <Text style={styles.theirMessageText}>{item.content}</Text>
                        )}
                    </View>
                    <View style={[styles.messageFooter, isMine && styles.myMessageFooter]}>
                        <Text style={styles.messageTime}>{formatTime(item.sentAt)}</Text>
                        {isMine && (
                            <Ionicons
                                name={item.isRead ? 'checkmark-done' : 'checkmark'}
                                size={14}
                                color={item.isRead ? '#4ADE80' : Colors.dark.textMuted}
                            />
                        )}
                    </View>
                </View>
            </>
        );
    };

    if (!match) {
        return (
            <LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={[styles.container, styles.centerContent]}
            >
                <Text style={styles.errorText}>Match not found</Text>
            </LinearGradient>
        );
    }

    return (
        <>
            <Stack.Screen
                options={{
                    headerTitle: () => (
                        <TouchableOpacity
                            style={styles.headerTitle}
                            onPress={() => router.push(`/profile/${match.otherStudentId}`)}
                        >
                            {match.otherStudentPhotoUrl ? (
                                <Image
                                    source={{ uri: match.otherStudentPhotoUrl }}
                                    style={styles.headerAvatar}
                                />
                            ) : (
                                <View style={[styles.headerAvatar, styles.noAvatar]}>
                                    <Ionicons name="person" size={18} color={Colors.dark.textMuted} />
                                </View>
                            )}
                            <View>
                                <Text style={styles.headerName}>{match.otherStudentName}</Text>
                                {isOtherTyping && (
                                    <Text style={styles.typingText}>typing...</Text>
                                )}
                            </View>
                        </TouchableOpacity>
                    ),
                    headerRight: () => (
                        <TouchableOpacity
                            onPress={() => {
                                Alert.alert(
                                    'Unmatch',
                                    `Are you sure you want to unmatch with ${match.otherStudentName}?`,
                                    [
                                        { text: 'Cancel', style: 'cancel' },
                                        {
                                            text: 'Unmatch',
                                            style: 'destructive',
                                            onPress: async () => {
                                                try {
                                                    await matchesApi.unmatch(matchIdNum);
                                                    router.back();
                                                } catch { Alert.alert('Error', 'Failed to unmatch'); }
                                            },
                                        },
                                    ]
                                );
                            }}
                            style={{ marginRight: 16 }}
                        >
                            <Ionicons name="ellipsis-horizontal" size={24} color={Colors.dark.text} />
                        </TouchableOpacity>
                    ),
                    headerStyle: {
                        backgroundColor: Colors.dark.surface,
                    },
                    headerTintColor: Colors.dark.text,
                    headerShadowVisible: false,
                }
                }
            />
            < LinearGradient
                colors={[Colors.dark.background, '#1a1a2e', Colors.dark.background]}
                style={styles.container}
            >
                <KeyboardAvoidingView
                    style={styles.keyboardAvoiding}
                    behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
                    keyboardVerticalOffset={90}
                >
                    {isLoading ? (
                        <View style={[styles.container, styles.centerContent]}>
                            <ActivityIndicator size="large" color={Colors.primary.main} />
                        </View>
                    ) : (
                        <FlatList
                            ref={flatListRef}
                            data={chatMessages}
                            keyExtractor={(item) => item.id.toString()}
                            renderItem={renderMessage}
                            contentContainerStyle={styles.messagesList}
                            showsVerticalScrollIndicator={false}
                            onContentSizeChange={() =>
                                flatListRef.current?.scrollToEnd({ animated: false })
                            }
                        />
                    )}

                    {/* Modern Input Bar */}
                    <View style={styles.inputContainer}>
                        <View style={styles.inputWrapper}>
                            <TouchableOpacity
                                style={styles.attachButton}
                                onPress={async () => {
                                    const result = await ImagePicker.launchImageLibraryAsync({
                                        mediaTypes: ImagePicker.MediaTypeOptions.Images,
                                        allowsEditing: true,
                                        quality: 0.8,
                                    });
                                    if (!result.canceled && result.assets[0]) {
                                        // For now, insert the image as a message placeholder
                                        // TODO: Upload image to backend and send as image message
                                        Alert.alert(
                                            'Photo Selected!',
                                            'Photo sharing will be available once backend integration is complete.',
                                            [{ text: 'OK' }]
                                        );
                                    }
                                }}
                            >
                                <Ionicons name="image-outline" size={24} color={Colors.dark.textMuted} />
                            </TouchableOpacity>
                            <TouchableOpacity
                                style={styles.attachButton}
                                onPress={() => setShowLocationPicker(true)}
                            >
                                <Ionicons name="location-outline" size={24} color={Colors.dark.textMuted} />
                            </TouchableOpacity>
                            <TextInput
                                style={styles.input}
                                value={inputText}
                                onChangeText={handleTextChange}
                                placeholder="Message..."
                                placeholderTextColor={Colors.dark.textMuted}
                                multiline
                                maxLength={5000}
                            />
                            <TouchableOpacity
                                style={styles.emojiButton}
                                onPress={() => setShowEmojiPicker(true)}
                            >
                                <Ionicons name="happy-outline" size={24} color={Colors.dark.textMuted} />
                            </TouchableOpacity>
                            <TouchableOpacity
                                style={styles.emojiButton}
                                onPress={() => setShowGifPicker(true)}
                            >
                                <Text style={{ fontSize: 13, fontWeight: '700', color: Colors.dark.textMuted }}>GIF</Text>
                            </TouchableOpacity>
                        </View>
                        <Animated.View style={{ transform: [{ scale: sendButtonScale }] }}>
                            <TouchableOpacity
                                style={[
                                    styles.sendButton,
                                    !inputText.trim() && styles.sendButtonDisabled,
                                ]}
                                onPress={handleSend}
                                disabled={!inputText.trim() || isSending}
                            >
                                <LinearGradient
                                    colors={inputText.trim() ? ['#FF6B6B', '#FF8E53'] : ['#2D2D44', '#1E1E2E']}
                                    style={styles.sendButtonGradient}
                                >
                                    {isSending ? (
                                        <ActivityIndicator size="small" color={Colors.white} />
                                    ) : (
                                        <Ionicons
                                            name="arrow-up"
                                            size={22}
                                            color={inputText.trim() ? Colors.white : Colors.dark.textMuted}
                                        />
                                    )}
                                </LinearGradient>
                            </TouchableOpacity>
                        </Animated.View>
                    </View>
                </KeyboardAvoidingView>

                {/* Emoji Picker */}
                <EmojiPicker
                    onEmojiSelected={(emoji: { emoji: string }) => {
                        setInputText(prev => prev + emoji.emoji);
                    }}
                    open={showEmojiPicker}
                    onClose={() => setShowEmojiPicker(false)}
                    theme={{
                        backdrop: '#00000080',
                        knob: '#7C3AED',
                        container: '#1E1E2E',
                        header: '#7C3AED',
                        skinTonesContainer: '#2D2D44',
                        category: {
                            icon: '#FFFFFF',
                            iconActive: '#7C3AED',
                            container: '#2D2D44',
                            containerActive: '#3D3D54',
                        },
                        search: {
                            background: '#2D2D44',
                            text: '#FFFFFF',
                            placeholder: '#888888',
                        },
                        emoji: {
                            selected: '#7C3AED30',
                        },
                    }}
                />

                {/* GIF Picker */}
                <GifPicker
                    visible={showGifPicker}
                    onClose={() => setShowGifPicker(false)}
                    onSelect={(gifUrl: string) => {
                        // For now, insert the GIF URL as a message
                        // TODO: Proper GIF message type in backend
                        setInputText(prev => prev + (prev ? ' ' : '') + gifUrl);
                    }}
                />

                {/* Location Picker */}
                <LocationPicker
                    visible={showLocationPicker}
                    onClose={() => setShowLocationPicker(false)}
                    onSelect={(location) => {
                        // Format location as Google Maps link
                        const mapUrl = `https://maps.google.com/?q=${location.latitude},${location.longitude}`;
                        const locationMessage = location.address
                            ? `📍 ${location.address}\n${mapUrl}`
                            : `📍 My Location\n${mapUrl}`;
                        setInputText(prev => prev + (prev ? '\n' : '') + locationMessage);
                    }}
                />
            </LinearGradient >
        </>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
    },
    keyboardAvoiding: {
        flex: 1,
    },
    centerContent: {
        justifyContent: 'center',
        alignItems: 'center',
    },
    headerTitle: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 12,
    },
    headerAvatar: {
        width: 40,
        height: 40,
        borderRadius: 20,
    },
    noAvatar: {
        backgroundColor: Colors.dark.surface,
        justifyContent: 'center',
        alignItems: 'center',
    },
    headerName: {
        fontSize: 17,
        fontWeight: '600',
        color: Colors.dark.text,
    },
    typingText: {
        fontSize: 12,
        color: '#4ADE80',
        fontStyle: 'italic',
    },
    messagesList: {
        padding: 16,
        paddingBottom: 8,
    },
    dateContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        marginVertical: 20,
        gap: 12,
    },
    dateLine: {
        flex: 1,
        height: 1,
        backgroundColor: 'rgba(255,255,255,0.08)',
    },
    dateText: {
        fontSize: 12,
        color: Colors.dark.textMuted,
        textTransform: 'uppercase',
        letterSpacing: 1,
    },
    messageRow: {
        marginBottom: 8,
        alignItems: 'flex-start',
    },
    myMessageRow: {
        alignItems: 'flex-end',
    },
    messageBubble: {
        maxWidth: '80%',
        borderRadius: 20,
        overflow: 'hidden',
    },
    myBubble: {},
    myBubbleGradient: {
        paddingHorizontal: 16,
        paddingVertical: 12,
    },
    theirBubble: {
        backgroundColor: 'rgba(255,255,255,0.08)',
        paddingHorizontal: 16,
        paddingVertical: 12,
    },
    myMessageText: {
        fontSize: 15,
        color: Colors.white,
        lineHeight: 20,
    },
    theirMessageText: {
        fontSize: 15,
        color: Colors.dark.text,
        lineHeight: 20,
    },
    messageFooter: {
        flexDirection: 'row',
        alignItems: 'center',
        gap: 4,
        marginTop: 4,
        marginLeft: 8,
    },
    myMessageFooter: {
        marginRight: 8,
        marginLeft: 0,
    },
    messageTime: {
        fontSize: 11,
        color: Colors.dark.textMuted,
    },
    readReceiptContainer: {
        marginLeft: 2,
    },
    inputContainer: {
        flexDirection: 'row',
        alignItems: 'flex-end',
        padding: 12,
        paddingBottom: 28,
        gap: 10,
    },
    inputWrapper: {
        flex: 1,
        flexDirection: 'row',
        alignItems: 'flex-end',
        backgroundColor: 'rgba(255,255,255,0.05)',
        borderRadius: 24,
        paddingHorizontal: 6,
        borderWidth: 1,
        borderColor: 'rgba(255,255,255,0.08)',
    },
    attachButton: {
        padding: 8,
    },
    input: {
        flex: 1,
        paddingVertical: 10,
        paddingHorizontal: 8,
        color: Colors.dark.text,
        fontSize: 15,
        maxHeight: 100,
    },
    emojiButton: {
        padding: 8,
    },
    sendButton: {},
    sendButtonDisabled: {},
    sendButtonGradient: {
        width: 44,
        height: 44,
        borderRadius: 22,
        justifyContent: 'center',
        alignItems: 'center',
    },
    errorText: {
        color: Colors.dark.textSecondary,
        fontSize: 16,
    },
});
