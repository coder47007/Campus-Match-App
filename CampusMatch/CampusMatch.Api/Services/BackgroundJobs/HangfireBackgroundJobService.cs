using System.Linq.Expressions;
using Hangfire;

namespace CampusMatch.Api.Services.BackgroundJobs;

/// <summary>
/// Hangfire-backed background job service implementation
/// </summary>
public class HangfireBackgroundJobService : IBackgroundJobService
{
    private readonly ILogger<HangfireBackgroundJobService> _logger;

    public HangfireBackgroundJobService(ILogger<HangfireBackgroundJobService> logger)
    {
        _logger = logger;
    }

    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        var jobId = BackgroundJob.Enqueue(methodCall);
        _logger.LogInformation("Enqueued background job {JobId} for {Type}", jobId, typeof(T).Name);
        return jobId;
    }

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        var jobId = BackgroundJob.Enqueue(methodCall);
        _logger.LogInformation("Enqueued async background job {JobId} for {Type}", jobId, typeof(T).Name);
        return jobId;
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        var jobId = BackgroundJob.Schedule(methodCall, delay);
        _logger.LogInformation("Scheduled background job {JobId} for {Type} with delay {Delay}", jobId, typeof(T).Name, delay);
        return jobId;
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        var jobId = BackgroundJob.Schedule(methodCall, delay);
        _logger.LogInformation("Scheduled async background job {JobId} for {Type} with delay {Delay}", jobId, typeof(T).Name, delay);
        return jobId;
    }

    public void AddOrUpdateRecurring<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression)
    {
        RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression);
        _logger.LogInformation("Added/Updated recurring job {JobId} for {Type} with cron {Cron}", recurringJobId, typeof(T).Name, cronExpression);
    }

    public void RemoveRecurring(string recurringJobId)
    {
        RecurringJob.RemoveIfExists(recurringJobId);
        _logger.LogInformation("Removed recurring job {JobId}", recurringJobId);
    }
}
