using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Candlelight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReviewsAndFavourites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<Guid>>(
                name: "Dependencies",
                table: "ModVersions",
                type: "uuid[]",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DownloadCount",
                table: "ModVersions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                table: "ModVersions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "ModVersions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "SupportedVersions",
                table: "ModVersions",
                type: "text[]",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ModFavourites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModFavourites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModFavourites_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModFavourites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSoftDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModReviews_Mods_ModId",
                        column: x => x.ModId,
                        principalTable: "Mods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModReviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModFavourites_Id",
                table: "ModFavourites",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModFavourites_ModId_UserId",
                table: "ModFavourites",
                columns: new[] { "ModId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModFavourites_UserId",
                table: "ModFavourites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ModReviews_ModId_UserId",
                table: "ModReviews",
                columns: new[] { "ModId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModReviews_UserId",
                table: "ModReviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModFavourites");

            migrationBuilder.DropTable(
                name: "ModReviews");

            migrationBuilder.DropColumn(
                name: "Dependencies",
                table: "ModVersions");

            migrationBuilder.DropColumn(
                name: "DownloadCount",
                table: "ModVersions");

            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                table: "ModVersions");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "ModVersions");

            migrationBuilder.DropColumn(
                name: "SupportedVersions",
                table: "ModVersions");
        }
    }
}
