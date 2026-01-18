using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private static readonly Dictionary<int, string> _userConnections = new();
    
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            _userConnections[userId.Value] = Context.ConnectionId;
        }
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            _userConnections.Remove(userId.Value);
        }
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendMessage(int recipientId, MessageDto message)
    {
        if (_userConnections.TryGetValue(recipientId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
    }
    
    public async Task NotifyMatch(int userId, MatchDto match)
    {
        if (_userConnections.TryGetValue(userId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("NewMatch", match);
        }
    }
    
    public async Task SendTypingIndicator(int recipientId, bool isTyping)
    {
        if (_userConnections.TryGetValue(recipientId, out var connectionId))
        {
            var senderId = GetUserId();
            await Clients.Client(connectionId).SendAsync("TypingIndicator", senderId, isTyping);
        }
    }
    
    public async Task NotifyMessagesRead(int recipientId, int matchId, List<int> messageIds)
    {
        if (_userConnections.TryGetValue(recipientId, out var connectionId))
        {
            var readerId = GetUserId();
            await Clients.Client(connectionId).SendAsync("MessagesRead", matchId, messageIds, DateTime.UtcNow);
        }
    }
    
    private int? GetUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
    }
    
    public static string? GetConnectionId(int userId)
    {
        return _userConnections.TryGetValue(userId, out var connectionId) ? connectionId : null;
    }
}
