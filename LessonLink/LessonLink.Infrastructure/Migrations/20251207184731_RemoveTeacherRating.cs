using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LessonLink.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTeacherRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Teachers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Teachers",
                type: "float",
                nullable: true);
        }
    }
}
