using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunGroupWebApp.Migrations
{
    /// <inheritdoc />
    public partial class recoverModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_AspNetUsers_OrganizerId",
                table: "Clubs");

            migrationBuilder.RenameColumn(
                name: "OrganizerId",
                table: "Clubs",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Clubs_OrganizerId",
                table: "Clubs",
                newName: "IX_Clubs_AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_AspNetUsers_AppUserId",
                table: "Clubs",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clubs_AspNetUsers_AppUserId",
                table: "Clubs");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Clubs",
                newName: "OrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_Clubs_AppUserId",
                table: "Clubs",
                newName: "IX_Clubs_OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubs_AspNetUsers_OrganizerId",
                table: "Clubs",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
