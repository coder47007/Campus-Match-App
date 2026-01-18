// Events API Controller
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;
using System.Security.Claims;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly CampusMatchDbContext _db;

    public EventsController(CampusMatchDbContext db)
    {
        _db = db;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    // GET: api/events - Get all active events
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(
        [FromQuery] string? category = null,
        [FromQuery] DateTime? startAfter = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _db.Events
            .Where(e => e.IsActive && e.StartTime > DateTime.UtcNow)
            .Include(e => e.Creator)
            .Include(e => e.Attendees)
            .AsQueryable();

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(e => e.Category == category);
        }

        if (startAfter.HasValue)
        {
            query = query.Where(e => e.StartTime >= startAfter.Value);
        }

        var events = await query
            .OrderBy(e => e.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => MapToDto(e, GetUserId()))
            .ToListAsync();

        return Ok(events);
    }

    // GET: api/events/{id} - Get single event
    [HttpGet("{id}")]
    public async Task<ActionResult<EventDto>> GetEvent(int id)
    {
        var eventItem = await _db.Events
            .Include(e => e.Creator)
            .Include(e => e.Attendees)
                .ThenInclude(a => a.Student)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem == null)
        {
            return NotFound(new { error = "Event not found" });
        }

        return Ok(MapToDto(eventItem, GetUserId()));
    }

    // GET: api/events/my-events - Get events created by current user
    [HttpGet("my-events")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetMyEvents()
    {
        var userId = GetUserId();

        var events = await _db.Events
            .Where(e => e.CreatorId == userId)
            .Include(e => e.Creator)
            .Include(e => e.Attendees)
            .OrderByDescending(e => e.StartTime)
            .Select(e => MapToDto(e, userId))
            .ToListAsync();

        return Ok(events);
    }

    // GET: api/events/attending - Get events user is attending
    [HttpGet("attending")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAttendingEvents()
    {
        var userId = GetUserId();

        var events = await _db.EventAttendees
            .Where(a => a.StudentId == userId)
            .Include(a => a.Event)
                .ThenInclude(e => e.Creator)
            .Include(a => a.Event)
                .ThenInclude(e => e.Attendees)
            .Select(a => a.Event)
            .Where(e => e.IsActive && e.StartTime > DateTime.UtcNow)
            .OrderBy(e => e.StartTime)
            .Select(e => MapToDto(e, userId))
            .ToListAsync();

        return Ok(events);
    }

    // POST: api/events - Create a new event
    [HttpPost]
    public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventRequest request)
    {
        var userId = GetUserId();

        var newEvent = new Event
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Location = request.Location ?? string.Empty,
            ImageUrl = request.ImageUrl,
            Category = request.Category ?? "Social",
            CreatorId = userId,
            MaxAttendees = request.MaxAttendees > 0 ? request.MaxAttendees : 100,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Events.Add(newEvent);
        await _db.SaveChangesAsync();

        // Auto-add creator as attendee
        _db.EventAttendees.Add(new EventAttendee
        {
            EventId = newEvent.Id,
            StudentId = userId,
            JoinedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        // Reload with includes
        var created = await _db.Events
            .Include(e => e.Creator)
            .Include(e => e.Attendees)
            .FirstAsync(e => e.Id == newEvent.Id);

        return CreatedAtAction(nameof(GetEvent), new { id = newEvent.Id }, MapToDto(created, userId));
    }

    // PUT: api/events/{id} - Update an event (only by creator)
    [HttpPut("{id}")]
    public async Task<ActionResult<EventDto>> UpdateEvent(int id, [FromBody] UpdateEventRequest request)
    {
        var userId = GetUserId();

        var eventItem = await _db.Events
            .Include(e => e.Creator)
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem == null)
        {
            return NotFound(new { error = "Event not found" });
        }

        if (eventItem.CreatorId != userId)
        {
            return Forbid();
        }

        if (request.Title != null) eventItem.Title = request.Title;
        if (request.Description != null) eventItem.Description = request.Description;
        if (request.StartTime.HasValue) eventItem.StartTime = request.StartTime.Value;
        if (request.EndTime.HasValue) eventItem.EndTime = request.EndTime;
        if (request.Location != null) eventItem.Location = request.Location;
        if (request.ImageUrl != null) eventItem.ImageUrl = request.ImageUrl;
        if (request.Category != null) eventItem.Category = request.Category;
        if (request.MaxAttendees.HasValue) eventItem.MaxAttendees = request.MaxAttendees.Value;

        await _db.SaveChangesAsync();

        return Ok(MapToDto(eventItem, userId));
    }

    // DELETE: api/events/{id} - Delete/cancel an event (only by creator)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var userId = GetUserId();

        var eventItem = await _db.Events.FindAsync(id);

        if (eventItem == null)
        {
            return NotFound(new { error = "Event not found" });
        }

        if (eventItem.CreatorId != userId)
        {
            return Forbid();
        }

        // Soft delete - mark as inactive
        eventItem.IsActive = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/events/{id}/join - Join an event
    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinEvent(int id)
    {
        var userId = GetUserId();

        var eventItem = await _db.Events
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventItem == null)
        {
            return NotFound(new { error = "Event not found" });
        }

        if (!eventItem.IsActive)
        {
            return BadRequest(new { error = "Event is no longer active" });
        }

        if (eventItem.Attendees.Count >= eventItem.MaxAttendees)
        {
            return BadRequest(new { error = "Event is full" });
        }

        var alreadyJoined = eventItem.Attendees.Any(a => a.StudentId == userId);
        if (alreadyJoined)
        {
            return BadRequest(new { error = "Already attending this event" });
        }

        _db.EventAttendees.Add(new EventAttendee
        {
            EventId = id,
            StudentId = userId,
            JoinedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        return Ok(new { message = "Joined event successfully" });
    }

    // POST: api/events/{id}/leave - Leave an event
    [HttpPost("{id}/leave")]
    public async Task<IActionResult> LeaveEvent(int id)
    {
        var userId = GetUserId();

        var attendance = await _db.EventAttendees
            .FirstOrDefaultAsync(a => a.EventId == id && a.StudentId == userId);

        if (attendance == null)
        {
            return NotFound(new { error = "Not attending this event" });
        }

        // Check if user is the creator - creators can't leave
        var eventItem = await _db.Events.FindAsync(id);
        if (eventItem?.CreatorId == userId)
        {
            return BadRequest(new { error = "Creators cannot leave their own event" });
        }

        _db.EventAttendees.Remove(attendance);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Left event successfully" });
    }

    // GET: api/events/{id}/attendees - Get event attendees
    [HttpGet("{id}/attendees")]
    public async Task<ActionResult<IEnumerable<AttendeeDto>>> GetAttendees(int id)
    {
        var eventItem = await _db.Events.FindAsync(id);
        if (eventItem == null)
        {
            return NotFound(new { error = "Event not found" });
        }

        var attendees = await _db.EventAttendees
            .Where(a => a.EventId == id)
            .Include(a => a.Student)
            .OrderBy(a => a.JoinedAt)
            .Select(a => new AttendeeDto
            {
                StudentId = a.StudentId,
                Name = a.Student.Name,
                PhotoUrl = a.Student.PhotoUrl,
                JoinedAt = a.JoinedAt
            })
            .ToListAsync();

        return Ok(attendees);
    }

    // GET: api/events/categories - Get available categories
    [HttpGet("categories")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<string>> GetCategories()
    {
        var categories = new[] { "Social", "Academic", "Sports", "Arts", "Music", "Food", "Games", "Career" };
        return Ok(categories);
    }

    private static EventDto MapToDto(Event e, int currentUserId)
    {
        return new EventDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Location = e.Location,
            ImageUrl = e.ImageUrl,
            Category = e.Category,
            CreatorId = e.CreatorId,
            CreatorName = e.Creator?.Name ?? "Unknown",
            CreatorPhotoUrl = e.Creator?.PhotoUrl,
            MaxAttendees = e.MaxAttendees,
            AttendeeCount = e.Attendees?.Count ?? 0,
            IsAttending = e.Attendees?.Any(a => a.StudentId == currentUserId) ?? false,
            IsCreator = e.CreatorId == currentUserId,
            CreatedAt = e.CreatedAt
        };
    }
}

// DTOs
public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public int CreatorId { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public string? CreatorPhotoUrl { get; set; }
    public int MaxAttendees { get; set; }
    public int AttendeeCount { get; set; }
    public bool IsAttending { get; set; }
    public bool IsCreator { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateEventRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public int MaxAttendees { get; set; } = 100;
}

public class UpdateEventRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
    public string? Category { get; set; }
    public int? MaxAttendees { get; set; }
}

public class AttendeeDto
{
    public int StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public DateTime JoinedAt { get; set; }
}
