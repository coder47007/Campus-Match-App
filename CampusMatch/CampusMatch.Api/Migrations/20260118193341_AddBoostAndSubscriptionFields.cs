using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CampusMatch.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBoostAndSubscriptionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BoostExpiresAt",
                table: "Students",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBoosted",
                table: "Students",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Location = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatorId = table.Column<int>(type: "integer", nullable: false),
                    MaxAttendees = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Students_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    Plan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SuperLikesRemaining = table.Column<int>(type: "integer", nullable: false),
                    RewindsRemaining = table.Column<int>(type: "integer", nullable: false),
                    BoostsRemaining = table.Column<int>(type: "integer", nullable: false),
                    LastSuperLikeReset = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastRewindReset = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastBoostReset = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventAttendees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAttendees_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventAttendees_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 133,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 134,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 135,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 136,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 137,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 138,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 139,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 143,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 144,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 145,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 146,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 147,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 148,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 149,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$ws1mSntBA4Gcy/41hdmzjutu1cS47o7yAj2pARaRlprEkthqeb/Ma", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "BoostExpiresAt", "CreatedAt", "IsBoosted", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { null, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), false, new DateTime(2026, 1, 18, 19, 33, 37, 395, DateTimeKind.Utc).AddTicks(5734), "$2a$11$/4P/MRqWONs3PU9LWbro.uOhpvLWsz4AFpjboCC608LapdlJAduKK", new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 19, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_EventId",
                table: "EventAttendees",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendees_StudentId",
                table: "EventAttendees",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatorId",
                table: "Events",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_StudentId",
                table: "Subscriptions",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventAttendees");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropColumn(
                name: "BoostExpiresAt",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsBoosted",
                table: "Students");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 100,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 133,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 134,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 135,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 136,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 137,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 138,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 139,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 143,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 144,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 145,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 146,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 147,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 148,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 149,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$uLCwoMmBg1NMnM8cdPROpOfftJgmxuPSyLgcOKW501WPucbOH.DZO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 999,
                columns: new[] { "CreatedAt", "LastActiveAt", "PasswordHash", "RewindsResetAt", "SuperLikesResetAt" },
                values: new object[] { new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), new DateTime(2026, 1, 15, 3, 41, 58, 530, DateTimeKind.Utc).AddTicks(6031), "$2a$11$sgof0.trm0ILw.6lgJhhhuDHVUCcHOqgwU/WnWAbpomTcxlS1v0tO", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
