using CampusMatch.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusMatch.Api.Data;

/// <summary>
/// Database seed data for development and testing.
/// Separated from DbContext for better maintainability.
/// </summary>
public static class DatabaseSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        var now = DateTime.UtcNow;
        
        // NOTE: This should come from configuration in production
        var baseUrl = "http://10.0.0.56:5229/uploads/";
        
        // Seed interests
        SeedInterests(modelBuilder);
        
        // Seed students (test data)
        SeedStudents(modelBuilder, passwordHash, adminPasswordHash, now, baseUrl);
        
        // Seed prompts
        SeedPrompts(modelBuilder);
    }
    
    private static void SeedInterests(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Interest>().HasData(
            new Interest { Id = 1, Name = "Music", Emoji = "🎵", Category = "Arts" },
            new Interest { Id = 2, Name = "Sports", Emoji = "⚽", Category = "Sports" },
            new Interest { Id = 3, Name = "Gaming", Emoji = "🎮", Category = "Entertainment" },
            new Interest { Id = 4, Name = "Cooking", Emoji = "👨‍🍳", Category = "Food" },
            new Interest { Id = 5, Name = "Travel", Emoji = "✈️", Category = "Lifestyle" },
            new Interest { Id = 6, Name = "Photography", Emoji = "📷", Category = "Arts" },
            new Interest { Id = 7, Name = "Reading", Emoji = "📚", Category = "Education" },
            new Interest { Id = 8, Name = "Hiking", Emoji = "🥾", Category = "Sports" },
            new Interest { Id = 9, Name = "Movies", Emoji = "🎬", Category = "Entertainment" },
            new Interest { Id = 10, Name = "Coffee", Emoji = "☕", Category = "Food" },
            new Interest { Id = 11, Name = "Fitness", Emoji = "💪", Category = "Sports" },
            new Interest { Id = 12, Name = "Art", Emoji = "🎨", Category = "Arts" },
            new Interest { Id = 13, Name = "Dancing", Emoji = "💃", Category = "Arts" },
            new Interest { Id = 14, Name = "Netflix", Emoji = "📺", Category = "Entertainment" },
            new Interest { Id = 15, Name = "Yoga", Emoji = "🧘", Category = "Sports" }
        );
    }
    
    private static void SeedStudents(ModelBuilder modelBuilder, string passwordHash, string adminPasswordHash, 
        DateTime now, string baseUrl)
    {
        var students = new List<Student>();

        // Core test students
        students.Add(new Student { Id = 1, Email = "emma@mybvc.ca", PasswordHash = passwordHash, Name = "Emma Wilson", Age = 21, Major = "Computer Science", Year = "Junior", Bio = "Coffee enthusiast ☕ Loves hiking and coding late nights.", PhotoUrl = baseUrl + "female_1.png", University = "Bow Valley College", Gender = "Female", PreferredGender = "Male", PhoneNumber = "555-123-4001", InstagramHandle = "@emma_codes", CreatedAt = now, LastActiveAt = now });
        students.Add(new Student { Id = 2, Email = "james@mybvc.ca", PasswordHash = passwordHash, Name = "James Chen", Age = 22, Major = "Business", Year = "Senior", Bio = "Entrepreneur at heart 🚀 Basketball player. Let's grab coffee!", PhotoUrl = baseUrl + "male_1.png", University = "Bow Valley College", Gender = "Male", PreferredGender = "Female", PhoneNumber = "555-123-4002", InstagramHandle = "@jameschen_biz", CreatedAt = now, LastActiveAt = now });
        students.Add(new Student { Id = 3, Email = "sofia@mybvc.ca", PasswordHash = passwordHash, Name = "Sofia Rodriguez", Age = 20, Major = "Psychology", Year = "Sophomore", Bio = "Art lover 🎨 Bookworm. Looking for deep conversations and museum dates.", PhotoUrl = baseUrl + "female_2.png", University = "Bow Valley College", Gender = "Female", PreferredGender = "Male", PhoneNumber = "555-123-4003", CreatedAt = now, LastActiveAt = now });
        students.Add(new Student { Id = 4, Email = "alex@mybvc.ca", PasswordHash = passwordHash, Name = "Alex Thompson", Age = 23, Major = "Engineering", Year = "Graduate", Bio = "Building the future one circuit at a time ⚡ Guitar player, dog lover.", PhotoUrl = baseUrl + "male_2.png", University = "Bow Valley College", Gender = "Male", PreferredGender = "Female", PhoneNumber = "555-123-4004", InstagramHandle = "@alex_builds", CreatedAt = now, LastActiveAt = now });
        
        students.AddRange(new[] {
            new Student { Id = 5, Email = "olivia@mybvc.ca", PasswordHash = passwordHash, Name = "Olivia Park", Age = 21, Major = "Biology", Year = "Junior", Bio = "Pre-med student 🏥 Nature lover.", PhotoUrl = baseUrl + "female_1.png", University = "Bow Valley College", Gender = "Female", PreferredGender = "Male", PhoneNumber = "555-123-4005", CreatedAt = now, LastActiveAt = now },
            new Student { Id = 6, Email = "marcus@mybvc.ca", PasswordHash = passwordHash, Name = "Marcus Johnson", Age = 22, Major = "Music", Year = "Senior", Bio = "Producer and DJ 🎶", PhotoUrl = baseUrl + "male_1.png", University = "Bow Valley College", Gender = "Male", PreferredGender = "Female", PhoneNumber = "555-123-4006", CreatedAt = now, LastActiveAt = now },
        });

        // Generate additional test students
        var maleNames = new[] { "Liam", "Noah", "Oliver", "Elijah", "James", "William", "Benjamin", "Lucas", "Henry", "Theodore", "Jack", "Levi", "Alexander", "Jackson", "Mateo", "Daniel", "Michael", "Mason", "Sebastian", "Ethan", "Logan", "Owen", "Samuel", "Jacob", "Asher" };
        var femaleNames = new[] { "Olivia", "Emma", "Charlotte", "Amelia", "Sophia", "Isabella", "Ava", "Mia", "Evelyn", "Harper", "Luna", "Camila", "Gianna", "Elizabeth", "Eleanor", "Ella", "Abigail", "Sofia", "Avery", "Scarlett", "Emily", "Aria", "Penelope", "Chloe", "Layla" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Thompson", "White", "Harris" };
        var majors = new[] { "Computer Science", "Business", "Psychology", "Nursing", "Engineering", "Biology", "Art", "Music", "Education", "Marketing" };

        int startId = 100;

        // 25 Males
        for (int i = 0; i < 25; i++)
        {
            students.Add(new Student {
                Id = startId + i,
                Email = $"{maleNames[i].ToLower()}.{lastNames[i].ToLower()}@mybvc.ca",
                PasswordHash = passwordHash,
                Name = $"{maleNames[i]} {lastNames[i]}",
                Age = 19 + (i % 5),
                Major = majors[i % majors.Length],
                Year = "Junior",
                Bio = $"Hey! I'm {maleNames[i]}. I study {majors[i % majors.Length]} and love meeting new people.",
                PhotoUrl = baseUrl + (i % 2 == 0 ? "male_1.png" : "male_2.png"),
                University = "Bow Valley College",
                Gender = "Male",
                PreferredGender = "Female",
                PhoneNumber = $"555-01{i:00}",
                CreatedAt = now,
                LastActiveAt = now
            });
        }

        // 25 Females
         for (int i = 0; i < 25; i++)
        {
            students.Add(new Student {
                Id = startId + 25 + i,
                Email = $"{femaleNames[i].ToLower()}.{lastNames[24-i].ToLower()}@mybvc.ca",
                PasswordHash = passwordHash,
                Name = $"{femaleNames[i]} {lastNames[24-i]}",
                Age = 19 + (i % 5),
                Major = majors[i % majors.Length],
                Year = "Junior",
                Bio = $"Hi there! I'm {femaleNames[i]}. Exploring campus life!",
                PhotoUrl = baseUrl + (i % 2 == 0 ? "female_1.png" : "female_2.png"),
                University = "Bow Valley College",
                Gender = "Female",
                PreferredGender = "Male",
                PhoneNumber = $"555-02{i:00}",
                CreatedAt = now,
                LastActiveAt = now
            });
        }

        // Admin
        students.Add(new Student { Id = 999, Email = "admin@mybvc.ca", PasswordHash = adminPasswordHash, Name = "Admin User", Age = 30, Major = "Administration", Year = "Staff", Bio = "Administrator", PhotoUrl = null, University = "Bow Valley College", Gender = "Other", PreferredGender = "Any", PhoneNumber = "555-000-0000", IsAdmin = true, CreatedAt = now, LastActiveAt = now });

        modelBuilder.Entity<Student>().HasData(students);
    }
    
    private static void SeedPrompts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prompt>().HasData(
            new Prompt { Id = 1, Question = "My ideal first date is...", Category = "Dating" },
            new Prompt { Id = 2, Question = "I'm looking for someone who...", Category = "Dating" },
            new Prompt { Id = 3, Question = "Two truths and a lie...", Category = "Fun" },
            new Prompt { Id = 4, Question = "My best travel story is...", Category = "Lifestyle" },
            new Prompt { Id = 5, Question = "I geek out on...", Category = "About Me" },
            new Prompt { Id = 6, Question = "The way to my heart is...", Category = "Dating" },
            new Prompt { Id = 7, Question = "My most controversial opinion is...", Category = "Fun" },
            new Prompt { Id = 8, Question = "I'm convinced that...", Category = "Fun" },
            new Prompt { Id = 9, Question = "My simple pleasures are...", Category = "Lifestyle" },
            new Prompt { Id = 10, Question = "A life goal of mine is...", Category = "About Me" },
            new Prompt { Id = 11, Question = "I'll pick the restaurant if you...", Category = "Dating" },
            new Prompt { Id = 12, Question = "My favorite campus spot is...", Category = "Campus" },
            new Prompt { Id = 13, Question = "I'm weirdly attracted to...", Category = "Fun" },
            new Prompt { Id = 14, Question = "The key to my heart is...", Category = "Dating" },
            new Prompt { Id = 15, Question = "My go-to karaoke song is...", Category = "Fun" }
        );
    }
}
