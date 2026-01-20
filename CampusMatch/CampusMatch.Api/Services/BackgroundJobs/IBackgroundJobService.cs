namespace CampusMatch.Api.Services.BackgroundJobs;

/// <summary>
/// Interface for background job service
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Enqueue a job to be executed immediately in the background
    /// </summary>
    string Enqueue<T>(System.Linq.Expressions.Expression<Action<T>> methodCall);
    
    /// <summary>
    /// Enqueue an async job to be executed immediately in the background
    /// </summary>
    string Enqueue<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall);
    
    /// <summary>
    /// Schedule a job to be executed at a specific time
    /// </summary>
    string Schedule<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, TimeSpan delay);
    
    /// <summary>
    /// Schedule an async job to be executed at a specific time
    /// </summary>
    string Schedule<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall, TimeSpan delay);
    
    /// <summary>
    /// Add or update a recurring job
    /// </summary>
    void AddOrUpdateRecurring<T>(string recurringJobId, System.Linq.Expressions.Expression<Func<T, Task>> methodCall, string cronExpression);
    
    /// <summary>
    /// Remove a recurring job
    /// </summary>
    void RemoveRecurring(string recurringJobId);
}
