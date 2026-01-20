using Microsoft.EntityFrameworkCore.Storage;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

/// <summary>
/// Unit of Work implementation - coordinates all repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CampusMatchDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repositories
    private IStudentRepository? _students;
    private ISwipeRepository? _swipes;
    private IMatchRepository? _matches;
    private IMessageRepository? _messages;
    private IRepository<Photo>? _photos;
    private IRepository<Interest>? _interests;
    private IRepository<Event>? _events;
    private IBlockRepository? _blocks;
    private IRepository<Report>? _reports;

    public UnitOfWork(CampusMatchDbContext context)
    {
        _context = context;
    }

    // Lazy-loaded repositories
    public IStudentRepository Students => 
        _students ??= new StudentRepository(_context);

    public ISwipeRepository Swipes => 
        _swipes ??= new SwipeRepository(_context);

    public IMatchRepository Matches => 
        _matches ??= new MatchRepository(_context);

    public IMessageRepository Messages => 
        _messages ??= new MessageRepository(_context);

    public IRepository<Photo> Photos => 
        _photos ??= new Repository<Photo>(_context);

    public IRepository<Interest> Interests => 
        _interests ??= new Repository<Interest>(_context);

    public IRepository<Event> Events => 
        _events ??= new Repository<Event>(_context);

    public IBlockRepository Blocks => 
        _blocks ??= new BlockRepository(_context);

    public IRepository<Report> Reports => 
        _reports ??= new Repository<Report>(_context);

    // Transaction management
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
