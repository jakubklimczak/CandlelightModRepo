using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Candlelight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomGameSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomGameDetailsId",
                table: "Games",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SteamGameDetailsId",
                table: "Games",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomGameDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CoverImage = table.Column<string>(type: "text", nullable: true),
                    Developer = table.Column<string>(type: "text", nullable: true),
                    Publisher = table.Column<string>(type: "text", nullable: true),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomGameDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomGameDetails_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameFavourites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameFavourites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameFavourites_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameFavourites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModReviews_Id",
                table: "ModReviews",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomGameDetails_GameId",
                table: "CustomGameDetails",
                column: "GameId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomGameDetails_Id",
                table: "CustomGameDetails",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameFavourites_GameId_UserId",
                table: "GameFavourites",
                columns: new[] { "GameId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameFavourites_Id",
                table: "GameFavourites",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameFavourites_UserId",
                table: "GameFavourites",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomGameDetails");

            migrationBuilder.DropTable(
                name: "GameFavourites");

            migrationBuilder.DropIndex(
                name: "IX_ModReviews_Id",
                table: "ModReviews");

            migrationBuilder.DropColumn(
                name: "CustomGameDetailsId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SteamGameDetailsId",
                table: "Games");
        }
    }
}
