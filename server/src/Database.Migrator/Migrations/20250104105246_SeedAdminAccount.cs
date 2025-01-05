using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrator.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CanManageArticles", "Email", "PasswordHash", "PasswordSalt" },
                values: new object[] { new Guid("be5d8b69-2c71-4532-b304-5597e1c519ee"), true, "admin@example.ru", new byte[] { 8, 14, 179, 106, 14, 232, 75, 244, 70, 75, 183, 34, 225, 172, 160, 91, 14, 196, 238, 125, 207, 140, 175, 58, 73, 24, 12, 136, 201, 252, 155, 98, 174, 142, 90, 113, 209, 44, 15, 204, 165, 153, 65, 215, 236, 23, 177, 182, 89, 51, 113, 97, 20, 124, 212, 166, 252, 62, 247, 255, 1, 107, 138, 80 }, new byte[] { 147, 104, 176, 193, 252, 142, 184, 175, 131, 159, 222, 22, 145, 71, 130, 41, 95, 238, 250, 148, 91, 24, 150, 233, 147, 177, 133, 228, 22, 88, 206, 251, 128, 203, 58, 123, 193, 77, 149, 252, 64, 206, 233, 136, 120, 180, 102, 5, 21, 33, 30, 83, 195, 47, 30, 243, 219, 184, 106, 224, 169, 72, 197, 110, 15, 47, 113, 137, 221, 163, 1, 14, 56, 7, 72, 207, 41, 109, 52, 135, 120, 237, 240, 25, 242, 29, 141, 54, 123, 228, 46, 232, 184, 49, 96, 110, 165, 29, 236, 48, 44, 22, 146, 123, 251, 222, 130, 126, 135, 168, 180, 9, 211, 234, 21, 5, 252, 148, 226, 144, 142, 109, 96, 237, 78, 154, 142, 31 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("be5d8b69-2c71-4532-b304-5597e1c519ee"));
        }
    }
}
