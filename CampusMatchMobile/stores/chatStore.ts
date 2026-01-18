// Chat store with Zustand - manages messages for individual chats

import { create } from 'zustand';
import { MessageDto } from '../types';
import { messagesApi } from '../services';
import { signalRService } from '../services/signalr';

interface ChatState {
    // State
    messages: Record<number, MessageDto[]>; // matchId -> messages
    isLoading: boolean;
    isSending: boolean;
    error: string | null;
    typingUsers: Record<number, boolean>; // matchId -> isTyping

    // Actions
    fetchMessages: (matchId: number) => Promise<void>;
    sendMessage: (matchId: number, content: string) => Promise<void>;
    addMessage: (matchId: number, message: MessageDto) => void;
    markMessagesAsRead: (matchId: number, messageIds: number[]) => Promise<void>;
    setTyping: (matchId: number, isTyping: boolean) => void;
    sendTypingIndicator: (recipientId: number, isTyping: boolean) => void;
    clearError: () => void;
    clearChat: (matchId: number) => void;
    initializeRealtime: () => () => void;
}

export const useChatStore = create<ChatState>((set, get) => ({
    // Initial state
    messages: {},
    isLoading: false,
    isSending: false,
    error: null,
    typingUsers: {},

    // Fetch messages for a match
    fetchMessages: async (matchId: number) => {
        set({ isLoading: true, error: null });
        try {
            const messages = await messagesApi.getMessages(matchId);
            set(state => ({
                messages: {
                    ...state.messages,
                    [matchId]: messages,
                },
                isLoading: false,
            }));

            // Mark unread messages as read
            const unreadIds = messages
                .filter(m => !m.isRead)
                .map(m => m.id);

            if (unreadIds.length > 0) {
                get().markMessagesAsRead(matchId, unreadIds);
            }
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to load messages';
            set({ isLoading: false, error: errorMessage });
        }
    },

    // Send a message
    sendMessage: async (matchId: number, content: string) => {
        set({ isSending: true, error: null });
        try {
            const message = await messagesApi.sendMessage({ matchId, content });

            // Add message to local state
            set(state => ({
                messages: {
                    ...state.messages,
                    [matchId]: [...(state.messages[matchId] || []), message],
                },
                isSending: false,
            }));
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to send message';
            set({ isSending: false, error: errorMessage });
            throw error;
        }
    },

    // Add a message (from real-time)
    addMessage: (matchId: number, message: MessageDto) => {
        set(state => {
            const existingMessages = state.messages[matchId] || [];

            // Check if message already exists
            if (existingMessages.some(m => m.id === message.id)) {
                return state;
            }

            return {
                messages: {
                    ...state.messages,
                    [matchId]: [...existingMessages, message],
                },
            };
        });
    },

    // Mark messages as read
    markMessagesAsRead: async (matchId: number, messageIds: number[]) => {
        try {
            await messagesApi.markRead(matchId, messageIds);

            // Update local state
            set(state => {
                const messages = state.messages[matchId];
                if (!messages) return state;

                return {
                    messages: {
                        ...state.messages,
                        [matchId]: messages.map(m =>
                            messageIds.includes(m.id)
                                ? { ...m, isRead: true, readAt: new Date().toISOString() }
                                : m
                        ),
                    },
                };
            });
        } catch (error) {
            // Ignore errors for read receipts
        }
    },

    // Set typing indicator for a match
    setTyping: (matchId: number, isTyping: boolean) => {
        set(state => ({
            typingUsers: {
                ...state.typingUsers,
                [matchId]: isTyping,
            },
        }));

        // Clear typing indicator after 3 seconds
        if (isTyping) {
            setTimeout(() => {
                set(state => ({
                    typingUsers: {
                        ...state.typingUsers,
                        [matchId]: false,
                    },
                }));
            }, 3000);
        }
    },

    // Send typing indicator
    sendTypingIndicator: (recipientId: number, isTyping: boolean) => {
        signalRService.sendTypingIndicator(recipientId, isTyping);
    },

    // Clear error
    clearError: () => set({ error: null }),

    // Clear chat for a specific match
    clearChat: (matchId: number) => {
        set(state => {
            const { [matchId]: _, ...remainingMessages } = state.messages;
            const { [matchId]: __, ...remainingTyping } = state.typingUsers;
            return {
                messages: remainingMessages,
                typingUsers: remainingTyping,
            };
        });
    },

    // Initialize real-time updates
    initializeRealtime: () => {
        // Subscribe to incoming messages
        const unsubMessage = signalRService.onMessage((message) => {
            // Find which match this message belongs to and add it
            // The match store will also update last message
            // We need to figure out the matchId from the sender
            // This is handled in the chat screen where we know the matchId
        });

        // Subscribe to typing indicators
        const unsubTyping = signalRService.onTyping((userId, isTyping) => {
            // We need to map userId to matchId
            // This requires knowing which matches have which users
        });

        // Subscribe to read receipts
        const unsubRead = signalRService.onRead((matchId, messageIds, readAt) => {
            set(state => {
                const messages = state.messages[matchId];
                if (!messages) return state;

                return {
                    messages: {
                        ...state.messages,
                        [matchId]: messages.map(m =>
                            messageIds.includes(m.id)
                                ? { ...m, isRead: true, readAt }
                                : m
                        ),
                    },
                };
            });
        });

        return () => {
            unsubMessage();
            unsubTyping();
            unsubRead();
        };
    },
}));

export default useChatStore;
