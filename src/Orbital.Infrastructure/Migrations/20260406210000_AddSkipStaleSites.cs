using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkipStaleSites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ActivityConfig_SkipStaleSites",
                table: "Rings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ActivityConfig_StaleSiteThresholdDays",
                table: "Rings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 90);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ActivityConfig_SkipStaleSites", table: "Rings");
            migrationBuilder.DropColumn(name: "ActivityConfig_StaleSiteThresholdDays", table: "Rings");
        }
    }
}
