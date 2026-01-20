namespace CampusMatch.Api.Repositories;

/// <summary>
/// Unit of Work pattern - coordinates multiple repository operations as a single transaction
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repositories
    IStudentRepository Students { get; }
    ISwipeRepository Swipes { get; }
    IMatchRepository Matches { get; }
    IMessageRepository Messages { get; }
    IBlockRepository Blocks { get; }
    IRepository<CampusMatch.Api.Models.Photo> Photos { get; }
    IRepository<CampusMatch.Api.Models.Interest> Interests { get; }
    IRepository<CampusMatch.Api.Models.Event> Events { get; }
    IRepository<CampusMatch.Api.Models.Report> Reports { get; }
    
    // Transaction management
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
