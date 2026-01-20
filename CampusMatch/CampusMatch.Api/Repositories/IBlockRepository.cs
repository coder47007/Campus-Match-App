using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

/// <summary>
/// Block-specific repository
/// </summary>
public interface IBlockRepository : IRepository<Block>
{
    Task<bool> AreBlockedAsync(int student1Id, int student2Id);
    Task<IEnumerable<int>> GetBlockedIdsAsync(int studentId);
}
