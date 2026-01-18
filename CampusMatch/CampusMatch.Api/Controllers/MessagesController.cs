using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Api.Hubs;
using CampusMatch.Api.Models;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    private readonly IHubContext<ChatHub> _hubContext;
    
    public MessagesController(CampusMatchDbContext db, IHubContext<ChatHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet("{matchId}")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(int matchId)
    {
        var userId = GetUserId();
        
        // Verify user is part of this match
        var match = await _db.Matches.FindAsync(matchId);
        if (match == null || (match.Student1Id != userId && match.Student2Id != userId))
        {
            return NotFound("Match not found.");
        }
        
        var messages = await _db.Messages
            .Include(m => m.Sender)
            .Where(m => m.MatchId == matchId)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto(
                m.Id, 
                m.SenderId, 
                m.Sender.Name, 
                m.Content, 
                m.SentAt, 
                m.IsRead,
                m.DeliveredAt,
                m.ReadAt
            ))
            .ToListAsync();
        
        // Mark messages as read and set ReadAt timestamp
        var unreadMessages = await _db.Messages
            .Where(m => m.MatchId == matchId && m.SenderId != userId && !m.IsRead)
            .ToListAsync();
        
        var now = DateTime.UtcNow;
        foreach (var msg in unreadMessages)
        {
            msg.IsRead = true;
            msg.ReadAt = now;
            if (msg.DeliveredAt == null) msg.DeliveredAt = now;
        }
        await _db.SaveChangesAsync();
        
        // Notify sender that messages were read via SignalR
        if (unreadMessages.Count > 0)
        {
            var senderId = unreadMessages.First().SenderId;
            var senderConnectionId = ChatHub.GetConnectionId(senderId);
            if (senderConnectionId != null)
            {
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("MessagesRead", new
                {
                    matchId,
                    readAt = now,
                    messageIds = unreadMessages.Select(m => m.Id).ToList()
                });
            }
        }
        
        return Ok(messages);
    }
    
    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage(SendMessageRequest request)
    {
        var userId = GetUserId();
        
        // Verify user is part of this match
        var match = await _db.Matches
            .Include(m => m.Student1)
            .Include(m => m.Student2)
            .FirstOrDefaultAsync(m => m.Id == request.MatchId);
            
        if (match == null || (match.Student1Id != userId && match.Student2Id != userId))
        {
            return NotFound("Match not found.");
        }
        
        var sender = await _db.Students.FindAsync(userId);
        
        var message = new Message
        {
            MatchId = request.MatchId,
            SenderId = userId,
            Content = request.Content
        };
        
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();
        
        var messageDto = new MessageDto(
            message.Id, 
            userId, 
            sender!.Name, 
            message.Content, 
            message.SentAt, 
            false,
            null,
            null
        );
        
        // Send via SignalR to recipient
        var recipientId = match.Student1Id == userId ? match.Student2Id : match.Student1Id;
        var recipientConnectionId = ChatHub.GetConnectionId(recipientId);
        if (recipientConnectionId != null)
        {
            await _hubContext.Clients.Client(recipientConnectionId).SendAsync("ReceiveMessage", messageDto);
            
            // Mark as delivered since recipient is online
            message.DeliveredAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            
            // Notify sender of delivery
            var senderConnectionId = ChatHub.GetConnectionId(userId);
            if (senderConnectionId != null)
            {
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("MessageDelivered", new
                {
                    messageId = message.Id,
                    deliveredAt = message.DeliveredAt
                });
            }
        }
        
        return Ok(messageDto);
    }
    
    // Mark specific messages as delivered
    [HttpPost("{matchId}/delivered")]
    public async Task<IActionResult> MarkDelivered(int matchId, [FromBody] List<int> messageIds)
    {
        var userId = GetUserId();
        
        var match = await _db.Matches.FindAsync(matchId);
        if (match == null || (match.Student1Id != userId && match.Student2Id != userId))
        {
            return NotFound("Match not found.");
        }
        
        var now = DateTime.UtcNow;
        var messages = await _db.Messages
            .Where(m => messageIds.Contains(m.Id) && m.MatchId == matchId && m.SenderId != userId && m.DeliveredAt == null)
            .ToListAsync();
        
        foreach (var msg in messages)
        {
            msg.DeliveredAt = now;
        }
        await _db.SaveChangesAsync();
        
        // Notify senders
        var senderIds = messages.Select(m => m.SenderId).Distinct();
        foreach (var senderId in senderIds)
        {
            var senderConnectionId = ChatHub.GetConnectionId(senderId);
            if (senderConnectionId != null)
            {
                var deliveredMsgIds = messages.Where(m => m.SenderId == senderId).Select(m => m.Id).ToList();
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("MessagesDelivered", new
                {
                    matchId,
                    deliveredAt = now,
                    messageIds = deliveredMsgIds
                });
            }
        }
        
        return Ok();
    }
    
    // Mark specific messages as read
    [HttpPost("{matchId}/read")]
    public async Task<IActionResult> MarkRead(int matchId, [FromBody] List<int> messageIds)
    {
        var userId = GetUserId();
        
        var match = await _db.Matches.FindAsync(matchId);
        if (match == null || (match.Student1Id != userId && match.Student2Id != userId))
        {
            return NotFound("Match not found.");
        }
        
        var now = DateTime.UtcNow;
        var messages = await _db.Messages
            .Where(m => messageIds.Contains(m.Id) && m.MatchId == matchId && m.SenderId != userId && !m.IsRead)
            .ToListAsync();
        
        foreach (var msg in messages)
        {
            msg.IsRead = true;
            msg.ReadAt = now;
            if (msg.DeliveredAt == null) msg.DeliveredAt = now;
        }
        await _db.SaveChangesAsync();
        
        // Notify senders
        var senderIds = messages.Select(m => m.SenderId).Distinct();
        foreach (var senderId in senderIds)
        {
            var senderConnectionId = ChatHub.GetConnectionId(senderId);
            if (senderConnectionId != null)
            {
                var readMsgIds = messages.Where(m => m.SenderId == senderId).Select(m => m.Id).ToList();
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("MessagesRead", new
                {
                    matchId,
                    readAt = now,
                    messageIds = readMsgIds
                });
            }
        }
        
        return Ok();
    }
}
