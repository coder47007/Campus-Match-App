using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

/// <summary>
/// Message-specific repository
/// </summary>
public interface IMessageRepository : IRepository<Message>
{
    Task<IEnumerable<Message>> GetMessagesForMatchAsync(int matchId, int skip = 0, int take = 50);
    Task<int> GetUnreadCountAsync(int matchId, int studentId);
    Task MarkAsReadAsync(int matchId, int studentId);
}
