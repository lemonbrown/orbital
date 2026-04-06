using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicDirectoryEnabled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublicDirectoryEnabled",
                table: "Rings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublicDirectoryEnabled",
                table: "Rings");
        }
    }
}
