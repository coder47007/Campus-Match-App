using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CampusMatch.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialSupabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Emoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Question = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Major = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Year = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    University = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PreferredGender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    InstagramHandle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SuperLikesRemaining = table.Column<int>(type: "integer", nullable: false),
                    SuperLikesResetAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RewindsRemaining = table.Column<int>(type: "integer", nullable: false),
                    RewindsResetAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    MinAgePreference = table.Column<int>(type: "integer", nullable: false),
                    MaxAgePreference = table.Column<int>(type: "integer", nullable: false),
                    MaxDistancePreference = table.Column<int>(type: "integer", nullable: false),
                    ShowOnlineStatus = table.Column<bool>(type: "boolean", nullable: false),
                    NotifyOnMatch = table.Column<bool>(type: "boolean", nullable: false),
                    NotifyOnMessage = table.Column<bool>(type: "boolean", nullable: false),
                    NotifyOnSuperLike = table.Column<bool>(type: "boolean", nullable: false),
                    PushNotificationToken = table.Column<string>(type: "text", nullable: true),
                    IsProfileHidden = table.Column<bool>(type: "boolean", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsBanned = table.Column<bool>(type: "boolean", nullable: false),
                    BannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BanReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdminId = table.Column<int>(type: "integer", nullable: true),
                    TargetUserId = table.Column<int>(type: "integer", nullable: true),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Students_AdminId",
                        column: x => x.AdminId,
                        principalTable: "Students",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ActivityLogs_Students_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlockerId = table.Column<int>(type: "integer", nullable: false),
                    BlockedId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blocks_Students_BlockedId",
                        column: x => x.BlockedId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Blocks_Students_BlockerId",
                        column: x => x.BlockerId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Student1Id = table.Column<int>(type: "integer", nullable: false),
                    Student2Id = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Students_Student1Id",
                        column: x => x.Student1Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Students_Student2Id",
                        column: x => x.Student2Id,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    BlobName = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReporterId = table.Column<int>(type: "integer", nullable: false),
                    ReportedId = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsReviewed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Students_ReportedId",
                        column: x => x.ReportedId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Students_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    DeviceInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentInterests",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    InterestId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentInterests", x => new { x.StudentId, x.InterestId });
                    table.ForeignKey(
                        name: "FK_StudentInterests_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentInterests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentPrompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    PromptId = table.Column<int>(type: "integer", nullable: false),
                    Answer = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPrompts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentPrompts_Prompts_PromptId",
                        column: x => x.PromptId,
                        principalTable: "Prompts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentPrompts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Swipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SwiperId = table.Column<int>(type: "integer", nullable: false),
                    SwipedId = table.Column<int>(type: "integer", nullable: false),
                    IsLike = table.Column<bool>(type: "boolean", nullable: false),
                    IsSuperLike = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Swipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Swipes_Students_SwipedId",
                        column: x => x.SwipedId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Swipes_Students_SwiperId",
                        column: x => x.SwiperId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<int>(type: "integer", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Students_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Interests",
                columns: new[] { "Id", "Category", "Emoji", "Name" },
                values: new object[,]
                {
                    { 1, "Arts", "🎵", "Music" },
                    { 2, "Sports", "⚽", "Sports" },
                    { 3, "Entertainment", "🎮", "Gaming" },
                    { 4, "Food", "👨‍🍳", "Cooking" },
                    { 5, "Lifestyle", "✈️", "Travel" },
                    { 6, "Arts", "📷", "Photography" },
                    { 7, "Education", "📚", "Reading" },
                    { 8, "Sports", "🥾", "Hiking" },
                    { 9, "Entertainment", "🎬", "Movies" },
                    { 10, "Food", "☕", "Coffee" },
                    { 11, "Sports", "💪", "Fitness" },
                    { 12, "Arts", "🎨", "Art" },
                    { 13, "Arts", "💃", "Dancing" },
                    { 14, "Entertainment", "📺", "Netflix" },
                    { 15, "Sports", "🧘", "Yoga" }
                });

            migrationBuilder.InsertData(
                table: "Prompts",
                columns: new[] { "Id", "Category", "IsActive", "Question" },
                values: new object[,]
                {
                    { 1, "Dating", true, "My ideal first date is..." },
                    { 2, "Dating", true, "I'm looking for someone who..." },
                    { 3, "Fun", true, "Two truths and a lie..." },
                    { 4, "Lifestyle", true, "My best travel story is..." },
                    { 5, "About Me", true, "I geek out on..." },
                    { 6, "Dating", true, "The way to my heart is..." },
                    { 7, "Fun", true, "My most controversial opinion is..." },
                    { 8, "Fun", true, "I'm convinced that..." },
                    { 9, "Lifestyle", true, "My simple pleasures are..." },
                    { 10, "About Me", true, "A life goal of mine is..." },
                    { 11, "Dating", true, "I'll pick the restaurant if you..." },
                    { 12, "Campus", true, "My favorite campus spot is..." },
                    { 13, "Fun", true, "I'm weirdly attracted to..." },
                    { 14, "Dating", true, "The key to my heart is..." },
                    { 15, "Fun", true, "My go-to karaoke song is..." }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Age", "BanReason", "BannedAt", "Bio", "CreatedAt", "Email", "Gender", "InstagramHandle", "IsAdmin", "IsBanned", "IsProfileHidden", "LastActiveAt", "Latitude", "Longitude", "Major", "MaxAgePreference", "MaxDistancePreference", "MinAgePreference", "Name", "NotifyOnMatch", "NotifyOnMessage", "NotifyOnSuperLike", "PasswordHash", "PhoneNumber", "PhotoUrl", "PreferredGender", "PushNotificationToken", "RefreshToken", "RefreshTokenExpiry", "RewindsRemaining", "RewindsResetAt", "ShowOnlineStatus", "SuperLikesRemaining", "SuperLikesResetAt", "University", "Year" },
                values: new object[,]
                {
                    { 1, 21, null, null, "Coffee enthusiast ☕ Loves hiking and coding late nights.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "emma@mybvc.ca", "Female", "@emma_codes", false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Computer Science", 30, 25, 18, "Emma Wilson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-123-4001", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 2, 22, null, null, "Entrepreneur at heart 🚀 Basketball player. Let's grab coffee!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "james@mybvc.ca", "Male", "@jameschen_biz", false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Business", 30, 25, 18, "James Chen", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-123-4002", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Senior" },
                    { 3, 20, null, null, "Art lover 🎨 Bookworm. Looking for deep conversations and museum dates.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "sofia@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Psychology", 30, 25, 18, "Sofia Rodriguez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-123-4003", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Sophomore" },
                    { 4, 23, null, null, "Building the future one circuit at a time ⚡ Guitar player, dog lover.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "alex@mybvc.ca", "Male", "@alex_builds", false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Engineering", 30, 25, 18, "Alex Thompson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-123-4004", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Graduate" },
                    { 5, 21, null, null, "Pre-med student 🏥 Nature lover.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "olivia@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Biology", 30, 25, 18, "Olivia Park", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-123-4005", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 6, 22, null, null, "Producer and DJ 🎶", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "marcus@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Music", 30, 25, 18, "Marcus Johnson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-123-4006", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Senior" },
                    { 100, 19, null, null, "Hey! I'm Liam. I study Computer Science and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "liam.smith@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Computer Science", 30, 25, 18, "Liam Smith", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0100", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 101, 20, null, null, "Hey! I'm Noah. I study Business and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "noah.johnson@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Business", 30, 25, 18, "Noah Johnson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0101", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 102, 21, null, null, "Hey! I'm Oliver. I study Psychology and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "oliver.williams@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Psychology", 30, 25, 18, "Oliver Williams", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0102", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 103, 22, null, null, "Hey! I'm Elijah. I study Nursing and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "elijah.brown@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Nursing", 30, 25, 18, "Elijah Brown", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0103", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 104, 23, null, null, "Hey! I'm James. I study Engineering and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "james.jones@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Engineering", 30, 25, 18, "James Jones", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0104", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 105, 19, null, null, "Hey! I'm William. I study Biology and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "william.garcia@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Biology", 30, 25, 18, "William Garcia", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0105", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 106, 20, null, null, "Hey! I'm Benjamin. I study Art and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "benjamin.miller@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Art", 30, 25, 18, "Benjamin Miller", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0106", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 107, 21, null, null, "Hey! I'm Lucas. I study Music and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "lucas.davis@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Music", 30, 25, 18, "Lucas Davis", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0107", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 108, 22, null, null, "Hey! I'm Henry. I study Education and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "henry.rodriguez@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Education", 30, 25, 18, "Henry Rodriguez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0108", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 109, 23, null, null, "Hey! I'm Theodore. I study Marketing and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "theodore.martinez@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Marketing", 30, 25, 18, "Theodore Martinez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0109", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 110, 19, null, null, "Hey! I'm Jack. I study Computer Science and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "jack.hernandez@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Computer Science", 30, 25, 18, "Jack Hernandez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0110", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 111, 20, null, null, "Hey! I'm Levi. I study Business and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "levi.lopez@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Business", 30, 25, 18, "Levi Lopez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0111", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 112, 21, null, null, "Hey! I'm Alexander. I study Psychology and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "alexander.gonzalez@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Psychology", 30, 25, 18, "Alexander Gonzalez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0112", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 113, 22, null, null, "Hey! I'm Jackson. I study Nursing and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "jackson.wilson@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Nursing", 30, 25, 18, "Jackson Wilson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0113", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 114, 23, null, null, "Hey! I'm Mateo. I study Engineering and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "mateo.anderson@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Engineering", 30, 25, 18, "Mateo Anderson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0114", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 115, 19, null, null, "Hey! I'm Daniel. I study Biology and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "daniel.thomas@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Biology", 30, 25, 18, "Daniel Thomas", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0115", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 116, 20, null, null, "Hey! I'm Michael. I study Art and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "michael.taylor@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Art", 30, 25, 18, "Michael Taylor", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0116", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 117, 21, null, null, "Hey! I'm Mason. I study Music and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "mason.moore@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Music", 30, 25, 18, "Mason Moore", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0117", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 118, 22, null, null, "Hey! I'm Sebastian. I study Education and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "sebastian.jackson@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Education", 30, 25, 18, "Sebastian Jackson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0118", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 119, 23, null, null, "Hey! I'm Ethan. I study Marketing and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "ethan.martin@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Marketing", 30, 25, 18, "Ethan Martin", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0119", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 120, 19, null, null, "Hey! I'm Logan. I study Computer Science and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "logan.lee@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Computer Science", 30, 25, 18, "Logan Lee", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0120", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 121, 20, null, null, "Hey! I'm Owen. I study Business and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "owen.perez@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Business", 30, 25, 18, "Owen Perez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0121", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 122, 21, null, null, "Hey! I'm Samuel. I study Psychology and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "samuel.thompson@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Psychology", 30, 25, 18, "Samuel Thompson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0122", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 123, 22, null, null, "Hey! I'm Jacob. I study Nursing and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "jacob.white@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Nursing", 30, 25, 18, "Jacob White", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0123", "http://10.0.0.56:5229/uploads/male_2.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 124, 23, null, null, "Hey! I'm Asher. I study Engineering and love meeting new people.", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "asher.harris@mybvc.ca", "Male", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Engineering", 30, 25, 18, "Asher Harris", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0124", "http://10.0.0.56:5229/uploads/male_1.png", "Female", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 125, 19, null, null, "Hi there! I'm Olivia. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "olivia.harris@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Computer Science", 30, 25, 18, "Olivia Harris", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0200", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 126, 20, null, null, "Hi there! I'm Emma. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "emma.white@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Business", 30, 25, 18, "Emma White", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0201", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 127, 21, null, null, "Hi there! I'm Charlotte. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "charlotte.thompson@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Psychology", 30, 25, 18, "Charlotte Thompson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0202", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 128, 22, null, null, "Hi there! I'm Amelia. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "amelia.perez@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Nursing", 30, 25, 18, "Amelia Perez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0203", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 129, 23, null, null, "Hi there! I'm Sophia. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "sophia.lee@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Engineering", 30, 25, 18, "Sophia Lee", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0204", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 130, 19, null, null, "Hi there! I'm Isabella. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "isabella.martin@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Biology", 30, 25, 18, "Isabella Martin", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0205", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 131, 20, null, null, "Hi there! I'm Ava. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "ava.jackson@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Art", 30, 25, 18, "Ava Jackson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0206", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 132, 21, null, null, "Hi there! I'm Mia. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "mia.moore@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Music", 30, 25, 18, "Mia Moore", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0207", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 133, 22, null, null, "Hi there! I'm Evelyn. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "evelyn.taylor@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Education", 30, 25, 18, "Evelyn Taylor", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0208", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 134, 23, null, null, "Hi there! I'm Harper. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "harper.thomas@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Marketing", 30, 25, 18, "Harper Thomas", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0209", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 135, 19, null, null, "Hi there! I'm Luna. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "luna.anderson@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Computer Science", 30, 25, 18, "Luna Anderson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0210", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 136, 20, null, null, "Hi there! I'm Camila. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "camila.wilson@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Business", 30, 25, 18, "Camila Wilson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0211", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 137, 21, null, null, "Hi there! I'm Gianna. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "gianna.gonzalez@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Psychology", 30, 25, 18, "Gianna Gonzalez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0212", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 138, 22, null, null, "Hi there! I'm Elizabeth. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "elizabeth.lopez@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Nursing", 30, 25, 18, "Elizabeth Lopez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0213", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 139, 23, null, null, "Hi there! I'm Eleanor. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "eleanor.hernandez@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Engineering", 30, 25, 18, "Eleanor Hernandez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0214", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 140, 19, null, null, "Hi there! I'm Ella. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "ella.martinez@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Biology", 30, 25, 18, "Ella Martinez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0215", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 141, 20, null, null, "Hi there! I'm Abigail. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "abigail.rodriguez@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Art", 30, 25, 18, "Abigail Rodriguez", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0216", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 142, 21, null, null, "Hi there! I'm Sofia. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "sofia.davis@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Music", 30, 25, 18, "Sofia Davis", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0217", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 143, 22, null, null, "Hi there! I'm Avery. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "avery.miller@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Education", 30, 25, 18, "Avery Miller", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0218", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 144, 23, null, null, "Hi there! I'm Scarlett. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "scarlett.garcia@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Marketing", 30, 25, 18, "Scarlett Garcia", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0219", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 145, 19, null, null, "Hi there! I'm Emily. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "emily.jones@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Computer Science", 30, 25, 18, "Emily Jones", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0220", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 146, 20, null, null, "Hi there! I'm Aria. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "aria.brown@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Business", 30, 25, 18, "Aria Brown", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0221", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 147, 21, null, null, "Hi there! I'm Penelope. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "penelope.williams@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Psychology", 30, 25, 18, "Penelope Williams", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0222", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 148, 22, null, null, "Hi there! I'm Chloe. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "chloe.johnson@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Nursing", 30, 25, 18, "Chloe Johnson", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0223", "http://10.0.0.56:5229/uploads/female_2.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 149, 23, null, null, "Hi there! I'm Layla. Exploring campus life!", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "layla.smith@mybvc.ca", "Female", null, false, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Engineering", 30, 25, 18, "Layla Smith", true, true, true, "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", "555-0224", "http://10.0.0.56:5229/uploads/female_1.png", "Male", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Junior" },
                    { 999, 30, null, null, "Administrator", new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "admin@mybvc.ca", "Other", null, true, false, false, new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), null, null, "Administration", 30, 25, 18, "Admin User", true, true, true, "$2a$11$sgof0.trm0ILw.6lgJhhhuDHVUCcHOqgwU/WnWAbpomTcxlS1v0tO", "555-000-0000", null, "Any", null, null, null, 1, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), "Bow Valley College", "Staff" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_AdminId",
                table: "ActivityLogs",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_CreatedAt",
                table: "ActivityLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLogs_TargetUserId",
                table: "ActivityLogs",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockedId",
                table: "Blocks",
                column: "BlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockerId_BlockedId",
                table: "Blocks",
                columns: new[] { "BlockerId", "BlockedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CreatedAt",
                table: "Matches",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Student1Id_Student2Id",
                table: "Matches",
                columns: new[] { "Student1Id", "Student2Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_Student2Id",
                table: "Matches",
                column: "Student2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MatchId",
                table: "Messages",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SentAt",
                table: "Messages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_StudentId",
                table: "Photos",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_IsReviewed",
                table: "Reports",
                column: "IsReviewed");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportedId",
                table: "Reports",
                column: "ReportedId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReporterId",
                table: "Reports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ExpiresAt",
                table: "Sessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RefreshToken",
                table: "Sessions",
                column: "RefreshToken");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_StudentId",
                table: "Sessions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInterests_InterestId",
                table: "StudentInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPrompts_PromptId",
                table: "StudentPrompts",
                column: "PromptId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPrompts_StudentId",
                table: "StudentPrompts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_IsBanned",
                table: "Students",
                column: "IsBanned");

            migrationBuilder.CreateIndex(
                name: "IX_Students_LastActiveAt",
                table: "Students",
                column: "LastActiveAt");

            migrationBuilder.CreateIndex(
                name: "IX_Students_University",
                table: "Students",
                column: "University");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_IsLike",
                table: "Swipes",
                column: "IsLike");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwipedId",
                table: "Swipes",
                column: "SwipedId");

            migrationBuilder.CreateIndex(
                name: "IX_Swipes_SwiperId_SwipedId",
                table: "Swipes",
                columns: new[] { "SwiperId", "SwipedId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLogs");

            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "StudentInterests");

            migrationBuilder.DropTable(
                name: "StudentPrompts");

            migrationBuilder.DropTable(
                name: "Swipes");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropTable(
                name: "Prompts");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
