using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;
using CampusMatch.Api.Services;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PhotosController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    private readonly IBlobStorageService _blobStorage;
    private readonly string[] _allowedTypes = { "image/jpeg", "image/png", "image/webp", "image/gif" };
    private const int MaxPhotos = 6;
    private const int MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB
    
    public PhotosController(CampusMatchDbContext db, IBlobStorageService blobStorage)
    {
        _db = db;
        _blobStorage = blobStorage;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    [HttpGet]
    public async Task<ActionResult<List<PhotoDto>>> GetMyPhotos()
    {
        var userId = GetUserId();
        var photos = await _db.Photos
            .Where(p => p.StudentId == userId)
            .OrderBy(p => p.DisplayOrder)
            .Select(p => new PhotoDto(p.Id, p.Url, p.IsPrimary, p.DisplayOrder))
            .ToListAsync();
            
        return Ok(photos);
    }
    
    [HttpPost("upload")]
    public async Task<ActionResult<PhotoDto>> Upload(IFormFile file)
    {
        var userId = GetUserId();
        
        // Validate file
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");
            
        if (file.Length > MaxFileSizeBytes)
            return BadRequest("File too large. Maximum size is 10MB.");
            
        if (!_allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest("Invalid file type. Allowed: JPEG, PNG, WebP, GIF.");
        
        // Check photo limit
        var existingCount = await _db.Photos.CountAsync(p => p.StudentId == userId);
        if (existingCount >= MaxPhotos)
            return BadRequest($"Maximum {MaxPhotos} photos allowed.");
        
        // Upload to storage
        using var stream = file.OpenReadStream();
        var blobName = await _blobStorage.UploadAsync(stream, file.FileName, file.ContentType);
        var url = _blobStorage.GetBlobUrl(blobName);
        
        // Create photo record
        var photo = new Photo
        {
            StudentId = userId,
            Url = url,
            BlobName = blobName,
            IsPrimary = existingCount == 0,  // First photo is primary
            DisplayOrder = existingCount
        };
        
        _db.Photos.Add(photo);
        
        // If primary, update student's main photo URL
        if (photo.IsPrimary)
        {
            var student = await _db.Students.FindAsync(userId);
            if (student != null)
            {
                student.PhotoUrl = url;
            }
        }
        
        await _db.SaveChangesAsync();
        
        return Ok(new PhotoDto(photo.Id, photo.Url, photo.IsPrimary, photo.DisplayOrder));
    }
    
    [HttpPut("{id}/primary")]
    public async Task<IActionResult> SetPrimary(int id)
    {
        var userId = GetUserId();
        
        var photo = await _db.Photos.FirstOrDefaultAsync(p => p.Id == id && p.StudentId == userId);
        if (photo == null)
            return NotFound();
        
        // Remove primary from all other photos
        var allPhotos = await _db.Photos.Where(p => p.StudentId == userId).ToListAsync();
        foreach (var p in allPhotos)
        {
            p.IsPrimary = p.Id == id;
        }
        
        // Update student's main photo
        var student = await _db.Students.FindAsync(userId);
        if (student != null)
        {
            student.PhotoUrl = photo.Url;
        }
        
        await _db.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        
        var photo = await _db.Photos.FirstOrDefaultAsync(p => p.Id == id && p.StudentId == userId);
        if (photo == null)
            return NotFound();
        
        // Delete from storage
        await _blobStorage.DeleteAsync(photo.BlobName);
        
        // Delete from database
        _db.Photos.Remove(photo);
        
        // If was primary, set next photo as primary
        if (photo.IsPrimary)
        {
            var nextPhoto = await _db.Photos
                .Where(p => p.StudentId == userId && p.Id != id)
                .OrderBy(p => p.DisplayOrder)
                .FirstOrDefaultAsync();
                
            if (nextPhoto != null)
            {
                nextPhoto.IsPrimary = true;
                var student = await _db.Students.FindAsync(userId);
                if (student != null)
                {
                    student.PhotoUrl = nextPhoto.Url;
                }
            }
            else
            {
                var student = await _db.Students.FindAsync(userId);
                if (student != null)
                {
                    student.PhotoUrl = null;
                }
            }
        }
        
        await _db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] List<int> photoIds)
    {
        var userId = GetUserId();
        
        var photos = await _db.Photos.Where(p => p.StudentId == userId).ToListAsync();
        
        for (int i = 0; i < photoIds.Count; i++)
        {
            var photo = photos.FirstOrDefault(p => p.Id == photoIds[i]);
            if (photo != null)
            {
                photo.DisplayOrder = i;
            }
        }
        
        await _db.SaveChangesAsync();
        
        return Ok();
    }
}
