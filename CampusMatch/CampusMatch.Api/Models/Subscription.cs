// Premium Subscription Models and DTOs
using System.ComponentModel.DataAnnotations;

namespace CampusMatch.Api.Models;

public class Subscription
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    [Required, MaxLength(50)]
    public string Plan { get; set; } = "free"; // free, premium, gold (legacy)
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public bool IsActive => Plan != "free" && EndDate.HasValue && EndDate > DateTime.UtcNow;
    
    // Premium features tracking
    public int SuperLikesRemaining { get; set; } = 1;   // Free: 1/day, Premium: 5/day, Gold: unlimited
    public int RewindsRemaining { get; set; } = 0;      // Free: 0, Premium: 3/day, Gold: unlimited
    public int BoostsRemaining { get; set; } = 0;       // Free: 1/10 days, Premium: unlimited, Gold: unlimited
    public int SwipesRemaining { get; set; } = 20;      // Free: 20/16hr, Premium/Gold: unlimited
    
    public DateTime LastSuperLikeReset { get; set; } = DateTime.UtcNow;
    public DateTime LastRewindReset { get; set; } = DateTime.UtcNow;
    public DateTime LastBoostReset { get; set; } = DateTime.UtcNow;
    public DateTime LastSwipeReset { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

// Subscription Plans with features
public static class SubscriptionPlans
{
    public static readonly Dictionary<string, PlanFeatures> Plans = new()
    {
        ["free"] = new PlanFeatures
        {
            Name = "Free",
            Price = 0,
            SuperLikesPerDay = 1,
            RewindsPerDay = 0,
            BoostsPerPeriod = 1,
            BoostPeriodDays = 10,           // 1 boost per 10 days
            SwipesPerPeriod = 20,
            SwipePeriodHours = 16,          // 20 swipes per 16 hours
            MaxDistanceKm = 0,              // Same campus only (0 = no cross-campus)
            CrossCampusMatching = false,    // Cannot match with other universities
            CanSeeWhoLikedYou = false,
            UnlimitedSwipes = false,
            AdvancedFilters = false,
            ReadReceipts = false,
            TypingIndicators = false,
            HighlightedBadge = false,
            NoAds = false,
            PriorityMatching = false
        },
        ["premium"] = new PlanFeatures
        {
            Name = "CampusMatch+",
            Price = 14.99m,
            SuperLikesPerDay = 5,
            RewindsPerDay = 5,
            BoostsPerPeriod = -1,           // Unlimited boosts
            BoostPeriodDays = 0,
            SwipesPerPeriod = -1,           // Unlimited swipes
            SwipePeriodHours = 0,
            MaxDistanceKm = 200,            // Can filter up to 200km
            CrossCampusMatching = true,     // Can match with other universities
            CanSeeWhoLikedYou = true,
            UnlimitedSwipes = true,
            AdvancedFilters = true,
            ReadReceipts = true,
            TypingIndicators = true,
            HighlightedBadge = true,
            NoAds = true,
            PriorityMatching = false
        },
        ["gold"] = new PlanFeatures
        {
            Name = "CampusMatch Gold",
            Price = 24.99m,
            SuperLikesPerDay = -1,          // Unlimited
            RewindsPerDay = -1,             // Unlimited
            BoostsPerPeriod = -1,           // Unlimited
            BoostPeriodDays = 0,
            SwipesPerPeriod = -1,           // Unlimited
            SwipePeriodHours = 0,
            MaxDistanceKm = 200,            // Can filter up to 200km
            CrossCampusMatching = true,     // Can match with other universities
            CanSeeWhoLikedYou = true,
            UnlimitedSwipes = true,
            AdvancedFilters = true,
            ReadReceipts = true,
            TypingIndicators = true,
            HighlightedBadge = true,
            NoAds = true,
            PriorityMatching = true         // Gold exclusive - appears first in discovery
        }
    };
}

public class PlanFeatures
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    // Feature limits (-1 = unlimited)
    public int SuperLikesPerDay { get; set; }
    public int RewindsPerDay { get; set; }
    public int BoostsPerPeriod { get; set; }
    public int BoostPeriodDays { get; set; }
    public int SwipesPerPeriod { get; set; }
    public int SwipePeriodHours { get; set; }
    
    // Distance & campus matching
    public int MaxDistanceKm { get; set; }              // 0 = same campus only, max 200km
    public bool CrossCampusMatching { get; set; }       // Can match outside own university
    
    // Feature flags
    public bool CanSeeWhoLikedYou { get; set; }
    public bool UnlimitedSwipes { get; set; }
    public bool AdvancedFilters { get; set; }           // Major, Year, Interests, Distance
    public bool ReadReceipts { get; set; }
    public bool TypingIndicators { get; set; }
    public bool HighlightedBadge { get; set; }
    public bool NoAds { get; set; }
    public bool PriorityMatching { get; set; }
}

// DTOs for subscription endpoints
public class SubscriptionDto
{
    public string Plan { get; set; } = "free";
    public string PlanName { get; set; } = "Free";
    public bool IsActive { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Remaining counts
    public int SuperLikesRemaining { get; set; }
    public int RewindsRemaining { get; set; }
    public int BoostsRemaining { get; set; }
    public int SwipesRemaining { get; set; }
    
    // Reset times (for countdown display)
    public DateTime? SwipesResetAt { get; set; }
    public DateTime? BoostsResetAt { get; set; }
    
    public PlanFeatures Features { get; set; } = new();
}

public class UpgradeRequest
{
    [Required]
    public string Plan { get; set; } = string.Empty;
    
    // Mock payment token (in production, this would be Stripe token etc.)
    public string? PaymentToken { get; set; }
}

public class PurchaseBoostRequest
{
    public int Quantity { get; set; } = 1;
}

// Swipe status response
public class SwipeStatusResponse
{
    public int SwipesRemaining { get; set; }
    public bool IsUnlimited { get; set; }
    public DateTime? ResetsAt { get; set; }
    public bool CanSwipe => IsUnlimited || SwipesRemaining > 0;
}
