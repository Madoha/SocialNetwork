using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Database.Migrations
{
    public partial class UserFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6af20afe-b323-46db-ace3-6aef28027f6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "85dcfb23-7a33-4fb0-9aed-3de0cd08ee7a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a9b2681-b45d-48d2-b8d8-b75c8ef8f4bf");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2a67bac5-b23d-442d-8416-bbb1b1e69a4e", "3", "Bloger", "BLOGER" },
                    { "2fe7ec3a-d9a2-4d88-9104-50e0ad11b2b2", "1", "Admin", "ADMIN" },
                    { "cc785b70-beba-434b-b96f-02aec8cad77e", "2", "User", "USER" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2a67bac5-b23d-442d-8416-bbb1b1e69a4e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2fe7ec3a-d9a2-4d88-9104-50e0ad11b2b2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc785b70-beba-434b-b96f-02aec8cad77e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6af20afe-b323-46db-ace3-6aef28027f6e", "2", "User", "USER" },
                    { "85dcfb23-7a33-4fb0-9aed-3de0cd08ee7a", "1", "Admin", "ADMIN" },
                    { "8a9b2681-b45d-48d2-b8d8-b75c8ef8f4bf", "3", "Bloger", "BLOGER" }
                });
        }
    }
}
