using Microsoft.EntityFrameworkCore;
using CampusMatch.Api.Models;

namespace CampusMatch.Api.Data;

public class CampusMatchDbContext : DbContext
{
    public CampusMatchDbContext(DbContextOptions<CampusMatchDbContext> options) : base(options) { }
    
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<StudentInterest> StudentInterests => Set<StudentInterest>();
    public DbSet<Swipe> Swipes => Set<Swipe>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Block> Blocks => Set<Block>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Prompt> Prompts => Set<Prompt>();
    public DbSet<StudentPrompt> StudentPrompts => Set<StudentPrompt>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventAttendee> EventAttendees => Set<EventAttendee>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Student
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();
        
        // Photo relationships
        modelBuilder.Entity<Photo>()
            .HasOne(p => p.Student)
            .WithMany(s => s.Photos)
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // StudentInterest (many-to-many join table)
        modelBuilder.Entity<StudentInterest>()
            .HasKey(si => new { si.StudentId, si.InterestId });
            
        modelBuilder.Entity<StudentInterest>()
            .HasOne(si => si.Student)
            .WithMany(s => s.Interests)
            .HasForeignKey(si => si.StudentId);
            
        modelBuilder.Entity<StudentInterest>()
            .HasOne(si => si.Interest)
            .WithMany(i => i.Students)
            .HasForeignKey(si => si.InterestId);
        
        // Swipe relationships
        modelBuilder.Entity<Swipe>()
            .HasOne(s => s.Swiper)
            .WithMany(st => st.SwipesMade)
            .HasForeignKey(s => s.SwiperId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Swipe>()
            .HasOne(s => s.Swiped)
            .WithMany(st => st.SwipesReceived)
            .HasForeignKey(s => s.SwipedId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Swipe>()
            .HasIndex(s => new { s.SwiperId, s.SwipedId })
            .IsUnique();
        
        // Match relationships
        modelBuilder.Entity<Match>()
            .HasOne(m => m.Student1)
            .WithMany()
            .HasForeignKey(m => m.Student1Id)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Match>()
            .HasOne(m => m.Student2)
            .WithMany()
            .HasForeignKey(m => m.Student2Id)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Message relationships
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Match)
            .WithMany(mt => mt.Messages)
            .HasForeignKey(m => m.MatchId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Report relationships
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Reporter)
            .WithMany()
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Report>()
            .HasOne(r => r.Reported)
            .WithMany()
            .HasForeignKey(r => r.ReportedId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Block relationships
        modelBuilder.Entity<Block>()
            .HasOne(b => b.Blocker)
            .WithMany(s => s.BlockedUsers)
            .HasForeignKey(b => b.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Block>()
            .HasOne(b => b.Blocked)
            .WithMany(s => s.BlockedByUsers)
            .HasForeignKey(b => b.BlockedId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<Block>()
            .HasIndex(b => new { b.BlockerId, b.BlockedId })
            .IsUnique();
        
        // Session relationships
        modelBuilder.Entity<Session>()
            .HasOne(s => s.Student)
            .WithMany(st => st.Sessions)
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // StudentPrompt relationships
        modelBuilder.Entity<StudentPrompt>()
            .HasOne(sp => sp.Student)
            .WithMany(s => s.Prompts)
            .HasForeignKey(sp => sp.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<StudentPrompt>()
            .HasOne(sp => sp.Prompt)
            .WithMany(p => p.StudentPrompts)
            .HasForeignKey(sp => sp.PromptId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // ActivityLog relationships - Use NoAction to avoid SQL Server cascade path issues
        modelBuilder.Entity<ActivityLog>()
            .HasOne(a => a.Admin)
            .WithMany()
            .HasForeignKey(a => a.AdminId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<ActivityLog>()
            .HasOne(a => a.TargetUser)
            .WithMany()
            .HasForeignKey(a => a.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        // Performance indexes for frequently queried columns
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.LastActiveAt);
        
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.University);
        
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.IsBanned);
        
        // NEW: Indexes for discover profile filtering (Phase 2 Performance)
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Gender);
        
       modelBuilder.Entity<Student>()
            .HasIndex(s => s.Age);
        
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Major);
        
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.IsProfileHidden);
        
        // Composite index for common discover query pattern
        modelBuilder.Entity<Student>()
            .HasIndex(s => new { s.IsBanned, s.IsProfileHidden, s.Gender, s.Age });
        
        modelBuilder.Entity<Match>()
            .HasIndex(m => m.CreatedAt);
        
        modelBuilder.Entity<Match>()
            .HasIndex(m => new { m.Student1Id, m.Student2Id });
        
        modelBuilder.Entity<Message>()
            .HasIndex(m => m.SentAt);
        
        modelBuilder.Entity<Message>()
            .HasIndex(m => m.MatchId);
        
        modelBuilder.Entity<Session>()
            .HasIndex(s => s.RefreshToken);
        
        modelBuilder.Entity<Session>()
            .HasIndex(s => s.ExpiresAt);
        
        modelBuilder.Entity<Report>()
            .HasIndex(r => r.IsReviewed);
        
        modelBuilder.Entity<Swipe>()
            .HasIndex(s => s.IsLike);
        
        // NEW: Index for undo swipe performance
        modelBuilder.Entity<Swipe>()
            .HasIndex(s => new { s.SwiperId, s.CreatedAt });
        
        modelBuilder.Entity<ActivityLog>()
            .HasIndex(a => a.CreatedAt);
            
        // PHASE 3: Seed data extracted to separate file for maintainability
        DatabaseSeeder.SeedData(modelBuilder);
    }
}
