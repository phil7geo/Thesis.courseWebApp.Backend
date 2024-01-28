using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thesis.courseWebApp.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteCoursesColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Subjects",
                table: "Courses",
                newName: "Subject");

            migrationBuilder.AddColumn<string>(
                name: "Favourite_courses",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Certification",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Course_format",
                table: "Courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Courses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "On_sale",
                table: "Courses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Courses",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "Courses",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "Courses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favourite_courses",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Certification",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Course_format",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "On_sale",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Courses",
                newName: "Subjects");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
