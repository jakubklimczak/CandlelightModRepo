using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Candlelight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SteamAppIdOnGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SteamAppId",
                table: "Games",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SteamAppId",
                table: "Games");
        }
    }
}
