using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RunGroupWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIndextoEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Clubs_AppUserId",
                table: "Clubs",
                newName: "IX_CreatedClubAppUserId");

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Addresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClubId",
                table: "Clubs",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUserId",
                table: "AspNetUsers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantIdClubId",
                table: "AppUserClubs",
                columns: new[] { "AppUserId", "ClubId" });

            migrationBuilder.CreateIndex(
                name: "IX_AddressId",
                table: "Addresses",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClubId",
                table: "Clubs");

            migrationBuilder.DropIndex(
                name: "IX_AppUserId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantIdClubId",
                table: "AppUserClubs");

            migrationBuilder.DropIndex(
                name: "IX_AddressId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_CreatedClubAppUserId",
                table: "Clubs",
                newName: "IX_Clubs_AppUserId");
        }
    }
}
