// SignalR client library for real-time communication
using Microsoft.AspNetCore.SignalR.Client;

// Shared DTOs used for messages, matches, etc.
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Client.Services;

/// <summary>
/// SignalRService handles ALL real-time communication
/// between the client app and the backend SignalR hub.
///
/// Responsibilities:
/// - Connect to SignalR ChatHub
/// - Receive real-time messages
/// - Receive new match notifications
/// - Handle typing indicators
/// - Manage connection lifecycle (connect / disconnect / dispose)
/// </summary>
public class SignalRService : IAsyncDisposable
{
    // ===================== FIELDS =====================

    // SignalR hub connection instance
    private HubConnection? _connection;

    // ===================== EVENTS =====================

    // Fired when a new chat message is received
    public event Action<MessageDto>? OnMessageReceived;

    // Fired when a new match occurs
    public event Action<MatchDto>? OnNewMatch;

    // Fired when a typing indicator is received
    // int  -> senderId
    // bool -> isTyping (true = typing, false = stopped)
    public event Action<int, bool>? OnTypingIndicator;

    // ===================== PROPERTIES =====================

    // Indicates whether the SignalR connection is currently active
    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    // ===================== CONNECTION =====================

    /// <summary>
    /// Connects to the SignalR ChatHub using JWT authentication.
    /// This method:
    /// - Builds the connection
    /// - Registers event listeners
    /// - Starts the real-time connection
    /// </summary>
    public async Task ConnectAsync(string token)
    {
        // Create a new SignalR hub connection
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5229/chathub", options =>
            {
                // Attach JWT token to authenticate the user
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            // Automatically reconnect if connection drops
            .WithAutomaticReconnect()
            .Build();

        // ===================== SERVER EVENTS =====================

        // Called when server sends a new chat message
        _connection.On<MessageDto>("ReceiveMessage", message =>
        {
            OnMessageReceived?.Invoke(message);
        });

        // Called when server notifies of a new match
        _connection.On<MatchDto>("NewMatch", match =>
        {
            OnNewMatch?.Invoke(match);
        });

        // Called when server sends typing status
        _connection.On<int, bool>("TypingIndicator", (senderId, isTyping) =>
        {
            OnTypingIndicator?.Invoke(senderId, isTyping);
        });

        // Start the SignalR connection
        await _connection.StartAsync();
    }

    // ===================== TYPING INDICATOR =====================

    /// <summary>
    /// Sends typing status to the server.
    /// Used to notify the recipient that the user is typing or stopped typing.
    /// </summary>
    public async Task SendTypingIndicatorAsync(int recipientId, bool isTyping)
    {
        // Ensure connection exists and is active
        if (_connection != null && IsConnected)
        {
            // Invoke server method
            await _connection.InvokeAsync(
                "SendTypingIndicator",
                recipientId,
                isTyping
            );
        }
    }

    // ===================== DISCONNECTION =====================

    /// <summary>
    /// Gracefully stops the SignalR connection.
    /// </summary>
    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
        }
    }

    // ===================== CLEANUP =====================

    /// <summary>
    /// Disposes the SignalR connection when the service is destroyed.
    /// Required for proper memory cleanup.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
    }
}
