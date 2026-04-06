using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityConfigAndNavigationEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ActivityConfig columns on Rings (owned entity, stored inline)
            migrationBuilder.AddColumn<bool>(
                name: "ActivityConfig_IsEnabled",
                table: "Rings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ActivityConfig_CrawlingEnabled",
                table: "Rings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ActivityConfig_RecentPostWeight",
                table: "Rings",
                type: "TEXT",
                nullable: false,
                defaultValue: 2.0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ActivityConfig_RecentUpdateWeight",
                table: "Rings",
                type: "TEXT",
                nullable: false,
                defaultValue: 1.5m);

            migrationBuilder.AddColumn<int>(
                name: "ActivityConfig_ActivityWindowDays",
                table: "Rings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "ActivityConfig_CrawlFrequency",
                table: "Rings",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Daily");

            // NavigationEvents table
            migrationBuilder.CreateTable(
                name: "NavigationEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DestinationSiteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NavigationEvents_RingId_OccurredAt",
                table: "NavigationEvents",
                columns: new[] { "RingId", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "NavigationEvents");

            migrationBuilder.DropColumn(name: "ActivityConfig_IsEnabled", table: "Rings");
            migrationBuilder.DropColumn(name: "ActivityConfig_CrawlingEnabled", table: "Rings");
            migrationBuilder.DropColumn(name: "ActivityConfig_RecentPostWeight", table: "Rings");
            migrationBuilder.DropColumn(name: "ActivityConfig_RecentUpdateWeight", table: "Rings");
            migrationBuilder.DropColumn(name: "ActivityConfig_ActivityWindowDays", table: "Rings");
            migrationBuilder.DropColumn(name: "ActivityConfig_CrawlFrequency", table: "Rings");
        }
    }
}
