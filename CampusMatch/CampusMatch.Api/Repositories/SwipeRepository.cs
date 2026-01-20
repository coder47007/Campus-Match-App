using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

public class SwipeRepository : Repository<Swipe>, ISwipeRepository
{
    public SwipeRepository(CampusMatchDbContext context) : base(context)
    {
    }

    public async Task<Swipe?> GetSwipeAsync(int swiperId, int swipedId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.SwiperId == swiperId && s.SwipedId == swipedId);
    }
    
    public async Task<Swipe?> GetRecentSwipeAsync(int swiperId, DateTime cutoffTime)
    {
        return await _dbSet
            .Where(s => s.SwiperId == swiperId && s.CreatedAt >= cutoffTime)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<Swipe>> GetSwipesForStudentAsync(int studentId)
    {
        return await _dbSet
            .Where(s => s.SwiperId == studentId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetSwipedIdsAsync(int studentId)
    {
        return await _dbSet
            .Where(s => s.SwiperId == studentId)
            .Select(s => s.SwipedId)
            .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetLikedMeIdsAsync(int studentId)
    {
        return await _dbSet
            .Where(s => s.SwipedId == studentId && s.IsLike)
            .Select(s => s.SwiperId)
            .ToListAsync();
    }

    public async Task<Swipe?> GetLastSwipeAsync(int studentId)
    {
        return await _dbSet
            .Where(s => s.SwiperId == studentId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasSwipedAsync(int swiperId, int swipedId)
    {
        return await _dbSet
            .AnyAsync(s => s.SwiperId == swiperId && s.SwipedId == swipedId);
    }
}
