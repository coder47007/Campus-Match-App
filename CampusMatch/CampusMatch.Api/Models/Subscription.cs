// Premium Subscription Models and DTOs
using System.ComponentModel.DataAnnotations;

namespace CampusMatch.Api.Models;

public class Subscription
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    [Required, MaxLength(50)]
    public string Plan { get; set; } = "free"; // free, premium, gold
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public bool IsActive => Plan != "free" && EndDate.HasValue && EndDate > DateTime.UtcNow;
    
    // Premium features
    public int SuperLikesRemaining { get; set; } = 1; // Free: 1/day, Premium: 5/day, Gold: unlimited
    public int RewindsRemaining { get; set; } = 0;    // Free: 0, Premium: 3/day, Gold: unlimited
    public int BoostsRemaining { get; set; } = 0;     // Free: 0, Premium: 1/month, Gold: 3/month
    
    public DateTime LastSuperLikeReset { get; set; } = DateTime.UtcNow;
    public DateTime LastRewindReset { get; set; } = DateTime.UtcNow;
    public DateTime LastBoostReset { get; set; } = DateTime.UtcNow;
    
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
            BoostsPerMonth = 0,
            CanSeeWhoLikedYou = false,
            UnlimitedLikes = false,
            AdvancedFilters = false,
            ReadReceipts = false,
            PriorityMatching = false
        },
        ["premium"] = new PlanFeatures
        {
            Name = "CampusMatch+",
            Price = 9.99m,
            SuperLikesPerDay = 5,
            RewindsPerDay = 3,
            BoostsPerMonth = 1,
            CanSeeWhoLikedYou = true,
            UnlimitedLikes = true,
            AdvancedFilters = true,
            ReadReceipts = true,
            PriorityMatching = false
        },
        ["gold"] = new PlanFeatures
        {
            Name = "CampusMatch Gold",
            Price = 19.99m,
            SuperLikesPerDay = -1, // unlimited
            RewindsPerDay = -1,    // unlimited
            BoostsPerMonth = 3,
            CanSeeWhoLikedYou = true,
            UnlimitedLikes = true,
            AdvancedFilters = true,
            ReadReceipts = true,
            PriorityMatching = true
        }
    };
}

public class PlanFeatures
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int SuperLikesPerDay { get; set; }
    public int RewindsPerDay { get; set; }
    public int BoostsPerMonth { get; set; }
    public bool CanSeeWhoLikedYou { get; set; }
    public bool UnlimitedLikes { get; set; }
    public bool AdvancedFilters { get; set; }
    public bool ReadReceipts { get; set; }
    public bool PriorityMatching { get; set; }
}

// DTOs for subscription endpoints
public class SubscriptionDto
{
    public string Plan { get; set; } = "free";
    public string PlanName { get; set; } = "Free";
    public bool IsActive { get; set; }
    public DateTime? EndDate { get; set; }
    public int SuperLikesRemaining { get; set; }
    public int RewindsRemaining { get; set; }
    public int BoostsRemaining { get; set; }
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
