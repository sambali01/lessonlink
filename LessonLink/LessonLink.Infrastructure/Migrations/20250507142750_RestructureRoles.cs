using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LessonLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RestructureRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3315d85b-004e-4c34-be97-fefd2e88111f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "99ea3533-4fea-45c8-b58f-d57c51ec9617");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d6f807d3-09f1-422c-8a13-7de29100de4d");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1a1a1111-1111-1111-1111-111111111111", null, "Student", "STUDENT" },
                    { "2b2b2222-2222-2222-2222-222222222222", null, "Teacher", "TEACHER" },
                    { "3c3c3333-3333-3333-3333-333333333333", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a1a1111-1111-1111-1111-111111111111");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2b2b2222-2222-2222-2222-222222222222");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c3c3333-3333-3333-3333-333333333333");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3315d85b-004e-4c34-be97-fefd2e88111f", null, "Admin", "ADMIN" },
                    { "99ea3533-4fea-45c8-b58f-d57c51ec9617", null, "Teacher", "TEACHER" },
                    { "d6f807d3-09f1-422c-8a13-7de29100de4d", null, "Student", "STUDENT" }
                });
        }
    }
}
