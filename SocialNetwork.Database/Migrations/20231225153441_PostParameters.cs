using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialNetwork.Database.Migrations
{
    public partial class PostParameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "62c19831-6d3e-4f91-9ea4-8864bd10b081");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f08190e-87c4-413f-b776-499fdfc1e496");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f3ab1b32-7c73-4744-af34-8732e308c569");

            migrationBuilder.RenameColumn(
                name: "ImgUrl",
                table: "Posts",
                newName: "MediaUrl");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "Posts",
                newName: "ImgUrl");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "62c19831-6d3e-4f91-9ea4-8864bd10b081", "3", "Tester", "TESTER" },
                    { "7f08190e-87c4-413f-b776-499fdfc1e496", "1", "Admin", "ADMIN" },
                    { "f3ab1b32-7c73-4744-af34-8732e308c569", "2", "User", "USER" }
                });
        }
    }
}
