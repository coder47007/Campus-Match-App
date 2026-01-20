using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

/// <summary>
/// Match-specific repository
/// </summary>
public interface IMatchRepository : IRepository<Match>
{
    Task<IEnumerable<Match>> GetMatchesForStudentAsync(int studentId);
    Task<Match?> GetMatchBetweenAsync(int student1Id, int student2Id);
    Task<Match?> GetMatchWithMessagesAsync(int matchId);
    Task<bool> AreMatchedAsync(int student1Id, int student2Id);
}
