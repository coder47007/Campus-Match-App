// Event Model
using System.ComponentModel.DataAnnotations;

namespace CampusMatch.Api.Models;

public class Event
{
    public int Id { get; set; }
    
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    
    [MaxLength(300)]
    public string Location { get; set; } = string.Empty;
    
    public string? ImageUrl { get; set; }
    
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty; // Social, Academic, Sports, Arts
    
    public int CreatorId { get; set; }
    public Student Creator { get; set; } = null!;
    
    public int MaxAttendees { get; set; } = 100;
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<EventAttendee> Attendees { get; set; } = new List<EventAttendee>();
}

public class EventAttendee
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int StudentId { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    public Event Event { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
