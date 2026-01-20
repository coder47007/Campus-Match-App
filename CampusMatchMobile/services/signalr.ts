// SignalR service for real-time chat and notifications

import * as signalR from '@microsoft/signalr';
import { SIGNALR_HUB_URL } from '../constants/config';
import { getAuthToken } from './api';
import { MessageDto, MatchDto } from '../types';

type MessageHandler = (message: MessageDto) => void;
type MatchHandler = (match: MatchDto) => void;
type TypingHandler = (userId: number, isTyping: boolean) => void;
type ReadHandler = (matchId: number, messageIds: number[], readAt: string) => void;

class SignalRService {
    private connection: signalR.HubConnection | null = null;
    private messageHandlers: MessageHandler[] = [];
    private matchHandlers: MatchHandler[] = [];
    private typingHandlers: TypingHandler[] = [];
    private readHandlers: ReadHandler[] = [];
    private isConnecting = false;
    private reconnectAttempts = 0;
    private maxReconnectAttempts = 5;

    // Initialize and start connection
    async start(): Promise<void> {
        if (this.connection?.state === signalR.HubConnectionState.Connected) {
            return;
        }

        if (this.isConnecting) {
            return;
        }

        const token = getAuthToken();
        if (!token) {
            return;
        }

        this.isConnecting = true;

        try {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(SIGNALR_HUB_URL, {
                    accessTokenFactory: () => token,
                })
                .withAutomaticReconnect({
                    nextRetryDelayInMilliseconds: (retryContext) => {
                        if (retryContext.previousRetryCount >= this.maxReconnectAttempts) {
                            return null; // Stop retrying
                        }
                        return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
                    },
                })
                .configureLogging(signalR.LogLevel.Warning)
                .build();

            // Set up event handlers
            this.setupEventHandlers();

            // Start connection
            await this.connection.start();
            this.reconnectAttempts = 0;
        } catch (error) {
            console.error('SignalR connection error:', error);
            this.reconnectAttempts++;

            if (this.reconnectAttempts < this.maxReconnectAttempts) {
                setTimeout(() => this.start(), 5000);
            }
        } finally {
            this.isConnecting = false;
        }
    }

    // Stop connection
    async stop(): Promise<void> {
        if (this.connection) {
            try {
                await this.connection.stop();
            } catch (error) {
                // Connection stop failed silently
            }
            this.connection = null;
        }
    }

    // Set up event handlers
    private setupEventHandlers(): void {
        if (!this.connection) return;

        // Handle incoming messages
        this.connection.on('ReceiveMessage', (message: MessageDto) => {
            this.messageHandlers.forEach(handler => handler(message));
        });

        // Handle new matches
        this.connection.on('NewMatch', (match: MatchDto) => {
            this.matchHandlers.forEach(handler => handler(match));
        });

        // Handle typing indicators
        this.connection.on('TypingIndicator', (userId: number, isTyping: boolean) => {
            this.typingHandlers.forEach(handler => handler(userId, isTyping));
        });

        // Handle read receipts
        this.connection.on('MessagesRead', (matchId: number, messageIds: number[], readAt: string) => {
            this.readHandlers.forEach(handler => handler(matchId, messageIds, readAt));
        });

        // Handle connection events
        this.connection.onreconnecting(() => {
            // Reconnecting
        });

        this.connection.onreconnected(() => {
            // Reconnected
        });

        this.connection.onclose(() => {
            // Connection closed
        });
    }

    // Send typing indicator
    async sendTypingIndicator(recipientId: number, isTyping: boolean): Promise<void> {
        if (this.connection?.state === signalR.HubConnectionState.Connected) {
            try {
                await this.connection.invoke('SendTypingIndicator', recipientId, isTyping);
            } catch (error) {
                console.error('Error sending typing indicator:', error);
            }
        }
    }

    // Subscribe to messages
    onMessage(handler: MessageHandler): () => void {
        this.messageHandlers.push(handler);
        return () => {
            this.messageHandlers = this.messageHandlers.filter(h => h !== handler);
        };
    }

    // Subscribe to matches
    onMatch(handler: MatchHandler): () => void {
        this.matchHandlers.push(handler);
        return () => {
            this.matchHandlers = this.matchHandlers.filter(h => h !== handler);
        };
    }

    // Subscribe to typing indicators
    onTyping(handler: TypingHandler): () => void {
        this.typingHandlers.push(handler);
        return () => {
            this.typingHandlers = this.typingHandlers.filter(h => h !== handler);
        };
    }

    // Subscribe to read receipts
    onRead(handler: ReadHandler): () => void {
        this.readHandlers.push(handler);
        return () => {
            this.readHandlers = this.readHandlers.filter(h => h !== handler);
        };
    }

    // Check if connected
    isConnected(): boolean {
        return this.connection?.state === signalR.HubConnectionState.Connected;
    }
}

// Export singleton instance
export const signalRService = new SignalRService();
export default signalRService;
