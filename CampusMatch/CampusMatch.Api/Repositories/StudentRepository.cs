using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

/// <summary>
/// Student repository implementation with optimized queries
/// </summary>
public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(CampusMatchDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Student?> GetWithPhotosAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Photos.OrderBy(p => p.DisplayOrder))
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetWithInterestsAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Interests)
                .ThenInclude(si => si.Interest)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetCompleteProfileAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Photos.OrderBy(p => p.DisplayOrder))
            .Include(s => s.Interests)
                .ThenInclude(si => si.Interest)
            .Include(s => s.Prompts)
                .ThenInclude(sp => sp.Prompt)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Student>> GetDiscoverCandidatesAsync(
        int studentId,
        int minAge,
        int maxAge,
        string? gender = null,
        List<string>? academicYears = null,
        List<string>? majors = null,
        int skip = 0,
        int take = 20)
    {
        var currentStudent = await GetByIdAsync(studentId);
        if (currentStudent == null) return Enumerable.Empty<Student>();

        // Get already swiped IDs
        var swipedIds = await _context.Swipes
            .Where(s => s.SwiperId == studentId)
            .Select(s => s.SwipedId)
            .ToListAsync();

        // Get blocked IDs
        var blockedIds = await _context.Blocks
            .Where(b => b.BlockerId == studentId || b.BlockedId == studentId)
            .Select(b => b.BlockerId == studentId ? b.BlockedId : b.BlockerId)
            .ToListAsync();

        // Build query with filters
        var query = _dbSet
            .Where(s => s.Id != studentId)
            .Where(s => !swipedIds.Contains(s.Id))
            .Where(s => !blockedIds.Contains(s.Id))
            .Where(s => s.Age >= minAge && s.Age <= maxAge);

        if (!string.IsNullOrEmpty(gender))
        {
            query = query.Where(s => s.Gender == gender);
        }

        if (academicYears != null && academicYears.Any())
        {
            query = query.Where(s => academicYears.Contains(s.Year));
        }

        if (majors != null && majors.Any())
        {
            query = query.Where(s => majors.Contains(s.Major));
        }

        return await query
            .Include(s => s.Photos.OrderBy(p => p.DisplayOrder))
            .Include(s => s.Interests)
                .ThenInclude(si => si.Interest)
            .Include(s => s.Prompts)
                .ThenInclude(sp => sp.Prompt)
            .OrderBy(s => Guid.NewGuid()) // Random order
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(s => s.Email == email);
    }
}
