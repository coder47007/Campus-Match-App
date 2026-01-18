// Match store with Zustand - manages matches and real-time updates

import { create } from 'zustand';
import { MatchDto, MessageDto } from '../types';
import { matchesApi, messagesApi } from '../services';
import { signalRService } from '../services/signalr';

interface MatchState {
    // State
    matches: MatchDto[];
    isLoading: boolean;
    error: string | null;
    lastMessages: Record<number, MessageDto | null>; // matchId -> last message

    // Actions
    fetchMatches: () => Promise<void>;
    addMatch: (match: MatchDto) => void;
    removeMatch: (matchId: number) => void;
    unmatch: (matchId: number) => Promise<void>;
    updateLastMessage: (matchId: number, message: MessageDto) => void;
    clearError: () => void;
    reset: () => void;
    initializeRealtime: () => () => void;
}

export const useMatchStore = create<MatchState>((set, get) => ({
    // Initial state
    matches: [],
    isLoading: false,
    error: null,
    lastMessages: {},

    // Fetch all matches
    fetchMatches: async () => {
        set({ isLoading: true, error: null });
        try {
            const matches = await matchesApi.getMatches();
            set({
                matches,
                isLoading: false,
            });

            // Fetch last message for each match
            for (const match of matches) {
                try {
                    const messages = await messagesApi.getMessages(match.id);
                    if (messages.length > 0) {
                        set(state => ({
                            lastMessages: {
                                ...state.lastMessages,
                                [match.id]: messages[messages.length - 1],
                            },
                        }));
                    }
                } catch (error) {
                    // Ignore individual errors
                }
            }
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to load matches';
            set({ isLoading: false, error: errorMessage });
        }
    },

    // Add a new match (from real-time notification)
    addMatch: (match: MatchDto) => {
        set(state => ({
            matches: [match, ...state.matches],
        }));
    },

    // Remove a match from the list
    removeMatch: (matchId: number) => {
        set(state => ({
            matches: state.matches.filter(m => m.id !== matchId),
            lastMessages: Object.fromEntries(
                Object.entries(state.lastMessages).filter(([id]) => parseInt(id) !== matchId)
            ),
        }));
    },

    // Unmatch (delete match)
    unmatch: async (matchId: number) => {
        try {
            await matchesApi.unmatch(matchId);
            get().removeMatch(matchId);
        } catch (error: unknown) {
            const errorMessage = error instanceof Error
                ? error.message
                : 'Failed to unmatch';
            set({ error: errorMessage });
            throw error;
        }
    },

    // Update last message for a match
    updateLastMessage: (matchId: number, message: MessageDto) => {
        set(state => ({
            lastMessages: {
                ...state.lastMessages,
                [matchId]: message,
            },
        }));
    },

    // Clear error
    clearError: () => set({ error: null }),

    // Reset store
    reset: () => set({
        matches: [],
        isLoading: false,
        error: null,
        lastMessages: {},
    }),

    // Initialize real-time updates
    initializeRealtime: () => {
        // Subscribe to new matches
        const unsubMatch = signalRService.onMatch((match) => {
            get().addMatch(match);
        });

        // Subscribe to new messages (for updating last message in list)
        const unsubMessage = signalRService.onMessage((message) => {
            // Find which match this message belongs to
            const { matches } = get();
            const match = matches.find(m =>
                m.otherStudentId === message.senderId
            );
            if (match) {
                get().updateLastMessage(match.id, message);
            }
        });

        // Return cleanup function
        return () => {
            unsubMatch();
            unsubMessage();
        };
    },
}));

export default useMatchStore;
