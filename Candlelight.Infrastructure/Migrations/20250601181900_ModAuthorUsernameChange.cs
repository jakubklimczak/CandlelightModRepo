using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Candlelight.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModAuthorUsernameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorUsername",
                table: "Mods");

            migrationBuilder.CreateIndex(
                name: "IX_Mods_CreatedBy",
                table: "Mods",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Mods_Users_CreatedBy",
                table: "Mods",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mods_Users_CreatedBy",
                table: "Mods");

            migrationBuilder.DropIndex(
                name: "IX_Mods_CreatedBy",
                table: "Mods");

            migrationBuilder.AddColumn<string>(
                name: "AuthorUsername",
                table: "Mods",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
