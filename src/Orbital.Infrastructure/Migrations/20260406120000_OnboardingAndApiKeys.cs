using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orbital.Infrastructure.Migrations
{
    public partial class OnboardingAndApiKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQLite doesn't support ALTER COLUMN, so rebuild the Sites table to make OwnerUserId nullable
            migrationBuilder.Sql("""
                ALTER TABLE "Sites" RENAME TO "Sites_old";

                CREATE TABLE "Sites" (
                    "Id"                 TEXT NOT NULL CONSTRAINT "PK_Sites" PRIMARY KEY,
                    "OwnerUserId"        TEXT NULL,
                    "Name"               TEXT NOT NULL,
                    "Url"                TEXT NOT NULL,
                    "Description"        TEXT NOT NULL,
                    "VerificationStatus" TEXT NOT NULL,
                    "VerificationToken"  TEXT NOT NULL,
                    "CreatedAt"          TEXT NOT NULL
                );

                INSERT INTO "Sites" ("Id","OwnerUserId","Name","Url","Description","VerificationStatus","VerificationToken","CreatedAt")
                SELECT              "Id","OwnerUserId","Name","Url","Description","VerificationStatus","VerificationToken","CreatedAt"
                FROM "Sites_old";

                DROP TABLE "Sites_old";
                """);

            // Add approval/onboarding settings to Rings
            migrationBuilder.AddColumn<string>(
                name: "ApprovalMode",
                table: "Rings",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "Manual");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublicJoinEnabled",
                table: "Rings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApiOnboardingEnabled",
                table: "Rings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            // Add applicant contact info to RingMemberships
            migrationBuilder.AddColumn<string>(
                name: "ApplicantName",
                table: "RingMemberships",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "RingMemberships",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            // Create RingApiKeys table
            migrationBuilder.CreateTable(
                name: "RingApiKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Label = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    KeyPrefix = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    KeyHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RingApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RingApiKeys_Rings_RingId",
                        column: x => x.RingId,
                        principalTable: "Rings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RingApiKeys_KeyPrefix",
                table: "RingApiKeys",
                column: "KeyPrefix");

            migrationBuilder.CreateIndex(
                name: "IX_RingApiKeys_RingId",
                table: "RingApiKeys",
                column: "RingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RingApiKeys");

            migrationBuilder.DropColumn(name: "ApprovalMode", table: "Rings");
            migrationBuilder.DropColumn(name: "IsPublicJoinEnabled", table: "Rings");
            migrationBuilder.DropColumn(name: "IsApiOnboardingEnabled", table: "Rings");
            migrationBuilder.DropColumn(name: "ApplicantName", table: "RingMemberships");
            migrationBuilder.DropColumn(name: "ContactEmail", table: "RingMemberships");

            migrationBuilder.Sql("""
                ALTER TABLE "Sites" RENAME TO "Sites_old";

                CREATE TABLE "Sites" (
                    "Id"                 TEXT NOT NULL CONSTRAINT "PK_Sites" PRIMARY KEY,
                    "OwnerUserId"        TEXT NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000',
                    "Name"               TEXT NOT NULL,
                    "Url"                TEXT NOT NULL,
                    "Description"        TEXT NOT NULL,
                    "VerificationStatus" TEXT NOT NULL,
                    "VerificationToken"  TEXT NOT NULL,
                    "CreatedAt"          TEXT NOT NULL
                );

                INSERT INTO "Sites" ("Id","OwnerUserId","Name","Url","Description","VerificationStatus","VerificationToken","CreatedAt")
                SELECT "Id", COALESCE("OwnerUserId",'00000000-0000-0000-0000-000000000000'), "Name","Url","Description","VerificationStatus","VerificationToken","CreatedAt"
                FROM "Sites_old";

                DROP TABLE "Sites_old";
                """);
        }
    }
}
