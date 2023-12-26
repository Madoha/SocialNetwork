using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Database.Migrations
{
    public partial class PostParametersUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e5edd58e-51e2-49de-a5d2-16c5f3e3be1d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6d41829-a656-400d-bde7-32fe87064fe8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f7612605-60c7-490d-9113-aaf50a1b517d");

            migrationBuilder.AddColumn<DateTime>(
                name: "WasCreated",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "WasCreated",
                table: "Posts");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "e5edd58e-51e2-49de-a5d2-16c5f3e3be1d", "2", "User", "USER" },
                    { "e6d41829-a656-400d-bde7-32fe87064fe8", "1", "Admin", "ADMIN" },
                    { "f7612605-60c7-490d-9113-aaf50a1b517d", "3", "Tester", "TESTER" }
                });
        }
    }
}
