using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(CampusMatchDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Match>> GetMatchesForStudentAsync(int studentId)
    {
        return await _dbSet
            .Where(m => m.Student1Id == studentId || m.Student2Id == studentId)
            .Include(m => m.Student1)
                .ThenInclude(s => s.Photos.OrderBy(p => p.DisplayOrder))
            .Include(m => m.Student2)
                .ThenInclude(s => s.Photos.OrderBy(p => p.DisplayOrder))
            .Include(m => m.Messages.OrderByDescending(msg => msg.SentAt))
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<Match?> GetMatchBetweenAsync(int student1Id, int student2Id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => 
                (m.Student1Id == student1Id && m.Student2Id == student2Id) ||
                (m.Student1Id == student2Id && m.Student2Id == student1Id));
    }

    public async Task<Match?> GetMatchWithMessagesAsync(int matchId)
    {
        return await _dbSet
            .Include(m => m.Student1)
            .Include(m => m.Student2)
            .Include(m => m.Messages.OrderBy(msg => msg.SentAt))
            .FirstOrDefaultAsync(m => m.Id == matchId);
    }

    public async Task<bool> AreMatchedAsync(int student1Id, int student2Id)
    {
        return await _dbSet
            .AnyAsync(m => 
                (m.Student1Id == student1Id && m.Student2Id == student2Id) ||
                (m.Student1Id == student2Id && m.Student2Id == student1Id));
    }
}
