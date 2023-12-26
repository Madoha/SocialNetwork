using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Database.Migrations
{
    public partial class UpdatedPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "604c0360-30e3-431f-8521-5dbbd633cce6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6e667401-e509-449f-b80a-9680d1eb8a92");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fcd154e4-1584-4846-a4de-7972bfe114ab");

            migrationBuilder.AddColumn<DateTime>(
                name: "WasEdited",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "446b646a-efac-424e-94db-0479c9c70d4a", "3", "Tester", "TESTER" },
                    { "ad532c10-96b9-4cfd-86cb-68c311bd73cc", "2", "User", "USER" },
                    { "e6709876-5a5f-4c1a-9dd1-61626b4dd998", "1", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "446b646a-efac-424e-94db-0479c9c70d4a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad532c10-96b9-4cfd-86cb-68c311bd73cc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6709876-5a5f-4c1a-9dd1-61626b4dd998");

            migrationBuilder.DropColumn(
                name: "WasEdited",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "604c0360-30e3-431f-8521-5dbbd633cce6", "2", "User", "USER" },
                    { "6e667401-e509-449f-b80a-9680d1eb8a92", "1", "Admin", "ADMIN" },
                    { "fcd154e4-1584-4846-a4de-7972bfe114ab", "3", "Tester", "TESTER" }
                });
        }
    }
}
