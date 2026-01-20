namespace CampusMatch.Api.Models;

/// <summary>
/// Interface for entities that support soft delete (IsDeleted, DeletedAt)
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    int? DeletedById { get; set; }
}

/// <summary>
/// Interface for entities with audit trail fields
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    int? CreatedById { get; set; }
    int? UpdatedById { get; set; }
}

/// <summary>
/// Base class combining both soft delete and audit functionality
/// </summary>
public abstract class AuditableEntityBase : IAuditableEntity, ISoftDelete
{
    // Audit fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }
    
    // Soft delete fields
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public int? DeletedById { get; set; }
}
