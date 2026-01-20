using CampusMatch.Api.Models;

namespace CampusMatch.Api.Repositories;

/// <summary>
/// Student-specific repository with custom queries
/// </summary>
public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByEmailAsync(string email);
    Task<Student?> GetWithPhotosAsync(int id);
    Task<Student?> GetWithInterestsAsync(int id);
    Task<Student?> GetCompleteProfileAsync(int id);
    Task<IEnumerable<Student>> GetDiscoverCandidatesAsync(
        int studentId, 
        int minAge, 
        int maxAge, 
        string? gender = null,
        List<string>? academicYears = null,
        List<string>? majors = null,
        int skip = 0,
        int take = 20
    );
    Task<bool> EmailExistsAsync(string email);
}
