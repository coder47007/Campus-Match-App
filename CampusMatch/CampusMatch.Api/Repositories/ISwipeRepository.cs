using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

/// <summary>
/// Swipe-specific repository
/// </summary>
public interface ISwipeRepository : IRepository<Swipe>
{
    Task<Swipe?> GetSwipeAsync(int swiperId, int swipedId);
    Task<Swipe?> GetRecentSwipeAsync(int swiperId, DateTime cutoffTime);
    Task<IEnumerable<Swipe>> GetSwipesForStudentAsync(int studentId);
    Task<IEnumerable<int>> GetLikedMeIdsAsync(int studentId);
    Task<Swipe?> GetLastSwipeAsync(int studentId);
    Task<bool> HasSwipedAsync(int swiperId, int swipedId);
}
