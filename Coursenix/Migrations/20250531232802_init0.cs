using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coursenix.Migrations
{
    /// <inheritdoc />
    public partial class init0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "AppUserId", "Biography", "Email", "Name", "PhoneNumber", "ProfilePicture" },
                values: new object[,]
                {
                    { 1, "user-001", "Expert in Computer Science with over 10 years of teaching experience.", "alice.johnson@coursenix.com", "Alice Johnson", "+1234567890", null },
                    { 2, "user-002", "Software engineer turned educator. Passionate about teaching algorithms.", "bob.smith@coursenix.com", "Bob Smith", "+0987654321", null },
                    { 3, "user-003", null, "clara.davis@coursenix.com", "Clara Davis", null, null },
                    { 4, "user-004", "Data science enthusiast and experienced ML instructor.", "daniel.thompson@coursenix.com", "Daniel Thompson", "+14152223333", null },
                    { 5, "user-005", "Loves teaching web development and frontend design.", "eva.wilson@coursenix.com", "Eva Wilson", "+16175551234", null },
                    { 6, "user-006", "Veteran backend developer with a focus on C# and .NET.", "frank.martin@coursenix.com", "Frank Martin", "+441234567890", null },
                    { 7, "user-007", "Specializes in mobile app development with Flutter.", "grace.lee@coursenix.com", "Grace Lee", null, null },
                    { 8, "user-008", "Enjoys breaking down complex math concepts.", "henry.walker@coursenix.com", "Henry Walker", "+33123456789", null },
                    { 9, "user-009", null, "ivy.brown@coursenix.com", "Ivy Brown", "+5511987654321", null },
                    { 10, "user-010", "Teaches cybersecurity and ethical hacking.", "jack.miller@coursenix.com", "Jack Miller", "+819012345678", null },
                    { 11, "user-011", "Machine learning and AI instructor.", "karen.white@coursenix.com", "Karen White", "+61234567890", null },
                    { 12, "user-012", null, "liam.scott@coursenix.com", "Liam Scott", null, null },
                    { 13, "user-013", "Loves inspiring students in game development.", "mia.clark@coursenix.com", "Mia Clark", "+49301234567", null },
                    { 14, "user-014", "Teaches systems programming and OS internals.", "nathan.lopez@coursenix.com", "Nathan Lopez", "+390612345678", null },
                    { 15, "user-015", "Database design and SQL expert.", "olivia.young@coursenix.com", "Olivia Young", null, null },
                    { 16, "user-016", "Cloud computing specialist with Azure and AWS.", "paul.hernandez@coursenix.com", "Paul Hernandez", "+8613812345678", null },
                    { 17, "user-017", "DevOps engineer turned instructor.", "quinn.ramirez@coursenix.com", "Quinn Ramirez", "+972501234567", null },
                    { 18, "user-018", "Frontend expert with deep React knowledge.", "rachel.kim@coursenix.com", "Rachel Kim", "+821012345678", null },
                    { 19, "user-019", null, "samuel.turner@coursenix.com", "Samuel Turner", null, null },
                    { 20, "user-020", "Blockchain developer with real-world experience.", "tina.nguyen@coursenix.com", "Tina Nguyen", "+6591234567", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
