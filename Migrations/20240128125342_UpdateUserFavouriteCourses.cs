using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thesis.courseWebApp.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserFavouriteCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "Favourite_courses",
                table: "Users",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Favourite_courses",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");
        }
    }
}
