using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

public class BlockRepository : Repository<Block>, IBlockRepository
{
    public BlockRepository(CampusMatchDbContext context) : base(context)
    {
    }

    public async Task<bool> AreBlockedAsync(int student1Id, int student2Id)
    {
        return await _dbSet
            .AnyAsync(b => 
                (b.BlockerId == student1Id && b.BlockedId == student2Id) ||
                (b.BlockerId == student2Id && b.BlockedId == student1Id));
    }

    public async Task<IEnumerable<int>> GetBlockedIdsAsync(int studentId)
    {
        return await _dbSet
            .Where(b => b.BlockerId == studentId)
            .Select(b => b.BlockedId)
            .ToListAsync();
    }
}
