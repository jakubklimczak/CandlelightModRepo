using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Candlelight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserProfileFavouritesVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FavouritesVisible",
                table: "UserProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavouritesVisible",
                table: "UserProfiles");
        }
    }
}
