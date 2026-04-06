using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbital.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVerificationMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VerificationMode",
                table: "Rings",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationMode",
                table: "Rings");
        }
    }
}
