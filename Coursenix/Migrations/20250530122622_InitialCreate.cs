using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coursenix.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Subjects_SubjectId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_SubjectId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_SubjectId",
                table: "Bookings",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Subjects_SubjectId",
                table: "Bookings",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }
    }
}
