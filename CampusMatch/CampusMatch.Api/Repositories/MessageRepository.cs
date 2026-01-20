using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(CampusMatchDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Message>> GetMessagesForMatchAsync(int matchId, int skip = 0, int take = 50)
    {
        return await _dbSet
            .Where(m => m.MatchId == matchId)
            .Include(m => m.Sender)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int matchId, int studentId)
    {
        return await _dbSet
            .CountAsync(m => m.MatchId == matchId && 
                           m.SenderId != studentId && 
                           !m.IsRead);
    }

    public async Task MarkAsReadAsync(int matchId, int studentId)
    {
        var unreadMessages = await _dbSet
            .Where(m => m.MatchId == matchId && 
                      m.SenderId != studentId && 
                      !m.IsRead)
            .ToListAsync();

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
        }
    }
}
