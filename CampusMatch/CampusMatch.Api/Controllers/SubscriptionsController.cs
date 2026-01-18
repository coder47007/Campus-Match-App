// Subscriptions Controller - Mock Payment Implementation
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
public class SubscriptionsController : ControllerBase
{
    private readonly CampusMatchDbContext _db;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(CampusMatchDbContext db, ILogger<SubscriptionsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    // GET: api/subscriptions - Get current user's subscription
    [HttpGet]
    public async Task<ActionResult<SubscriptionDto>> GetSubscription()
    {
        var userId = GetUserId();
        var subscription = await GetOrCreateSubscription(userId);
        
        // Reset daily/monthly limits if needed
        await ResetLimitsIfNeeded(subscription);
        
        return Ok(MapToDto(subscription));
    }

    // GET: api/subscriptions/plans - Get all available plans
    [HttpGet("plans")]
    [AllowAnonymous]
    public ActionResult<Dictionary<string, PlanFeatures>> GetPlans()
    {
        return Ok(SubscriptionPlans.Plans);
    }

    // POST: api/subscriptions/upgrade - Upgrade subscription (mock payment)
    [HttpPost("upgrade")]
    public async Task<ActionResult<SubscriptionDto>> UpgradeSubscription([FromBody] UpgradeRequest request)
    {
        if (!SubscriptionPlans.Plans.ContainsKey(request.Plan) || request.Plan == "free")
        {
            return BadRequest(new { error = "Invalid plan selected" });
        }

        var userId = GetUserId();
        var subscription = await GetOrCreateSubscription(userId);

        // Mock payment processing - in production, integrate with Stripe/RevenueCat
        _logger.LogInformation("Processing mock payment for user {UserId}, plan: {Plan}, token: {Token}", 
            userId, request.Plan, request.PaymentToken ?? "none");

        // Simulate payment success
        var planFeatures = SubscriptionPlans.Plans[request.Plan];
        
        subscription.Plan = request.Plan;
        subscription.StartDate = DateTime.UtcNow;
        subscription.EndDate = DateTime.UtcNow.AddMonths(1); // 1-month subscription
        subscription.SuperLikesRemaining = planFeatures.SuperLikesPerDay == -1 ? 999 : planFeatures.SuperLikesPerDay;
        subscription.RewindsRemaining = planFeatures.RewindsPerDay == -1 ? 999 : planFeatures.RewindsPerDay;
        subscription.BoostsRemaining = planFeatures.BoostsPerMonth;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("User {UserId} upgraded to {Plan}", userId, request.Plan);

        return Ok(MapToDto(subscription));
    }

    // POST: api/subscriptions/cancel - Cancel subscription
    [HttpPost("cancel")]
    public async Task<ActionResult<SubscriptionDto>> CancelSubscription()
    {
        var userId = GetUserId();
        var subscription = await GetOrCreateSubscription(userId);

        if (subscription.Plan == "free")
        {
            return BadRequest(new { error = "No active subscription to cancel" });
        }

        // Don't immediately cancel - let it expire at end of period
        _logger.LogInformation("User {UserId} cancelled subscription, active until {EndDate}", 
            userId, subscription.EndDate);

        return Ok(new { 
            message = "Subscription cancelled. You will retain access until " + subscription.EndDate?.ToString("MMM dd, yyyy"),
            subscription = MapToDto(subscription)
        });
    }

    // POST: api/subscriptions/use-superlike - Use a super like
    [HttpPost("use-superlike")]
    public async Task<ActionResult> UseSuperLike()
    {
        var userId = GetUserId();
        var subscription = await GetOrCreateSubscription(userId);
        await ResetLimitsIfNeeded(subscription);

        var features = SubscriptionPlans.Plans[subscription.Plan];
        
        if (features.SuperLikesPerDay != -1 && subscription.SuperLikesRemaining <= 0)
        {
            return BadRequest(new { 
                error = "No super likes remaining",
                upgradeRequired = subscription.Plan == "free"
            });
        }

        if (features.SuperLikesPerDay != -1)
        {
            subscription.SuperLikesRemaining--;
            await _db.SaveChangesAsync();
        }

        return Ok(new { 
            superLikesRemaining = subscription.SuperLikesRemaining,
            isUnlimited = features.SuperLikesPerDay == -1
        });
    }

    // POST: api/subscriptions/use-rewind - Use a rewind
    [HttpPost("use-rewind")]
    public async Task<ActionResult> UseRewind()
    {
        var userId = GetUserId();
        var subscription = await GetOrCreateSubscription(userId);
        await ResetLimitsIfNeeded(subscription);

        var features = SubscriptionPlans.Plans[subscription.Plan];

        if (features.RewindsPerDay == 0)
        {
            return BadRequest(new { 
                error = "Rewinds are not available on free plan",
                upgradeRequired = true
            });
        }

        if (features.RewindsPerDay != -1 && subscription.RewindsRemaining <= 0)
        {
            return BadRequest(new { 
                error = "No rewinds remaining today",
                resetsAt = subscription.LastRewindReset.AddDays(1)
            });
        }

        if (features.RewindsPerDay != -1)
        {
            subscription.RewindsRemaining--;
            await _db.SaveChangesAsync();
        }

        return Ok(new { 
            rewindsRemaining = subscription.RewindsRemaining,
            isUnlimited = features.RewindsPerDay == -1
        });
    }

    // POST: api/subscriptions/use-boost - Use a profile boost
    [HttpPost("use-boost")]
    public async Task<ActionResult> UseBoost()
    {
        var userId = GetUserId();
        var subscription = await GetOrCreateSubscription(userId);
        await ResetLimitsIfNeeded(subscription);

        if (subscription.BoostsRemaining <= 0)
        {
            return BadRequest(new { 
                error = "No boosts remaining",
                canPurchase = true
            });
        }

        subscription.BoostsRemaining--;
        await _db.SaveChangesAsync();

        // Update user's boost status (boost lasts 30 minutes)
        var user = await _db.Students.FindAsync(userId);
        if (user != null)
        {
            user.IsBoosted = true;
            user.BoostExpiresAt = DateTime.UtcNow.AddMinutes(30);
            await _db.SaveChangesAsync();
        }

        return Ok(new { 
            message = "Boost activated for 30 minutes!",
            boostsRemaining = subscription.BoostsRemaining,
            expiresAt = DateTime.UtcNow.AddMinutes(30)
        });
    }

    // POST: api/subscriptions/purchase-boost - Purchase additional boosts
    [HttpPost("purchase-boost")]
    public async Task<ActionResult> PurchaseBoost([FromBody] PurchaseBoostRequest request)
    {
        var userId = GetUserId();
        var subscription = await GetOrCreateSubscription(userId);

        // Mock payment for boost purchase ($2.99 each)
        _logger.LogInformation("User {UserId} purchased {Quantity} boost(s) (mock)", userId, request.Quantity);

        subscription.BoostsRemaining += request.Quantity;
        await _db.SaveChangesAsync();

        return Ok(new { 
            message = $"Purchased {request.Quantity} boost(s)!",
            boostsRemaining = subscription.BoostsRemaining
        });
    }

    private async Task<Subscription> GetOrCreateSubscription(int userId)
    {
        var subscription = await _db.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.StudentId == userId);

        if (subscription == null)
        {
            subscription = new Subscription
            {
                StudentId = userId,
                Plan = "free",
                SuperLikesRemaining = 1,
                RewindsRemaining = 0,
                BoostsRemaining = 0,
                CreatedAt = DateTime.UtcNow
            };
            _db.Set<Subscription>().Add(subscription);
            await _db.SaveChangesAsync();
        }

        return subscription;
    }

    private async Task ResetLimitsIfNeeded(Subscription subscription)
    {
        var now = DateTime.UtcNow;
        var features = SubscriptionPlans.Plans[subscription.Plan];
        var changed = false;

        // Reset super likes daily
        if ((now - subscription.LastSuperLikeReset).TotalDays >= 1)
        {
            subscription.SuperLikesRemaining = features.SuperLikesPerDay == -1 ? 999 : features.SuperLikesPerDay;
            subscription.LastSuperLikeReset = now;
            changed = true;
        }

        // Reset rewinds daily
        if ((now - subscription.LastRewindReset).TotalDays >= 1)
        {
            subscription.RewindsRemaining = features.RewindsPerDay == -1 ? 999 : features.RewindsPerDay;
            subscription.LastRewindReset = now;
            changed = true;
        }

        // Reset boosts monthly
        if ((now - subscription.LastBoostReset).TotalDays >= 30)
        {
            subscription.BoostsRemaining = features.BoostsPerMonth;
            subscription.LastBoostReset = now;
            changed = true;
        }

        if (changed)
        {
            await _db.SaveChangesAsync();
        }
    }

    private static SubscriptionDto MapToDto(Subscription subscription)
    {
        var features = SubscriptionPlans.Plans.GetValueOrDefault(subscription.Plan) 
            ?? SubscriptionPlans.Plans["free"];

        return new SubscriptionDto
        {
            Plan = subscription.Plan,
            PlanName = features.Name,
            IsActive = subscription.IsActive,
            EndDate = subscription.EndDate,
            SuperLikesRemaining = subscription.SuperLikesRemaining,
            RewindsRemaining = subscription.RewindsRemaining,
            BoostsRemaining = subscription.BoostsRemaining,
            Features = features
        };
    }
}
