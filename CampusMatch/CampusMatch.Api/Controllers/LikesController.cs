using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CampusMatch.Api.Data;
using CampusMatch.Api.Models;
using CampusMatch.Shared.DTOs;

namespace CampusMatch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LikesController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    
    public LikesController(CampusMatchDbContext db)
    {
        _db = db;
    }
    
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
    // Get who liked you - PREMIUM/GOLD ONLY for full profiles
    // Free users get blurred preview with count only
    [HttpGet("received")]
    public async Task<ActionResult<object>> GetReceivedLikes()
    {
        var userId = GetUserId();
        
        // Check subscription for "see who liked you" feature
        var subscription = await _db.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.StudentId == userId);
        var plan = subscription?.Plan ?? "free";
        var planFeatures = SubscriptionPlans.Plans.GetValueOrDefault(plan) ?? SubscriptionPlans.Plans["free"];
        
        // Get IDs of users you've already swiped on
        var swipedIds = await _db.Swipes
            .Where(s => s.SwiperId == userId)
            .Select(s => s.SwipedId)
            .ToListAsync();
        
        // Get IDs of blocked users (both directions)
        var blockedIds = await _db.Blocks
            .Where(b => b.BlockerId == userId || b.BlockedId == userId)
            .Select(b => b.BlockerId == userId ? b.BlockedId : b.BlockerId)
            .ToListAsync();
        
        var excludedIds = swipedIds.Concat(blockedIds).Distinct().ToHashSet();
        
        // Get users who liked you but you haven't swiped on
        var likesQuery = _db.Swipes
            .Include(s => s.Swiper)
                .ThenInclude(st => st.Photos)
            .Where(s => s.SwipedId == userId && s.IsLike && !excludedIds.Contains(s.SwiperId))
            .OrderByDescending(s => s.IsSuperLike)
            .ThenByDescending(s => s.CreatedAt)
            .Take(20);
        
        // Premium/Gold users: Return full profile info
        if (planFeatures.CanSeeWhoLikedYou)
        {
            var fullLikes = await likesQuery
                .Select(s => new {
                    id = s.SwiperId,
                    name = s.Swiper.Name,
                    age = s.Swiper.Age,
                    photoUrl = s.Swiper.PhotoUrl ?? s.Swiper.Photos.OrderBy(p => p.DisplayOrder).Select(p => p.Url).FirstOrDefault(),
                    major = s.Swiper.Major,
                    university = s.Swiper.University,
                    isSuperLike = s.IsSuperLike,
                    likedAt = s.CreatedAt,
                    isBlurred = false
                })
                .ToListAsync();
            
            return Ok(new {
                canSeeWhoLikedYou = true,
                likes = fullLikes
            });
        }
        
        // Free users: Return blurred previews only
        var blurredLikes = await likesQuery
            .Select(s => new LikePreviewDto(
                s.SwiperId,
                s.Swiper.PhotoUrl ?? s.Swiper.Photos.OrderBy(p => p.DisplayOrder).Select(p => p.Url).FirstOrDefault(),  // Will be blurred on client
                s.Swiper.Name.Length > 0 ? s.Swiper.Name[0].ToString() : "?",
                s.IsSuperLike,
                s.CreatedAt
            ))
            .ToListAsync();
        
        return Ok(new {
            canSeeWhoLikedYou = false,
            upgradeRequired = true,
            likesCount = blurredLikes.Count,
            likes = blurredLikes  // Photos will be blurred by client
        });
    }
    
    // Get count of likes received (available to all users)
    [HttpGet("count")]
    public async Task<ActionResult<object>> GetLikesCount()
    {
        var userId = GetUserId();
        
        // Get IDs of users you've already swiped on or matched with
        var swipedIds = await _db.Swipes
            .Where(s => s.SwiperId == userId)
            .Select(s => s.SwipedId)
            .ToListAsync();
        
        var blockedIds = await _db.Blocks
            .Where(b => b.BlockerId == userId || b.BlockedId == userId)
            .Select(b => b.BlockerId == userId ? b.BlockedId : b.BlockerId)
            .ToListAsync();
        
        var excludedIds = swipedIds.Concat(blockedIds).Distinct().ToHashSet();
        
        var count = await _db.Swipes
            .CountAsync(s => s.SwipedId == userId && s.IsLike && !excludedIds.Contains(s.SwiperId));
        
        var superLikeCount = await _db.Swipes
            .CountAsync(s => s.SwipedId == userId && s.IsSuperLike && !excludedIds.Contains(s.SwiperId));
        
        // Check if user has premium to see who liked them
        var subscription = await _db.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.StudentId == userId);
        var plan = subscription?.Plan ?? "free";
        var planFeatures = SubscriptionPlans.Plans.GetValueOrDefault(plan) ?? SubscriptionPlans.Plans["free"];
        
        return Ok(new { 
            total = count, 
            superLikes = superLikeCount,
            hasNew = count > 0,
            canSeeWhoLikedYou = planFeatures.CanSeeWhoLikedYou
        });
    }
}
