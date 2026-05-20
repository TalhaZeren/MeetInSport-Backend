using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetInSport.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExpirationToPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationAt",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ExpirationDays",
                table: "LessonPackages",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationAt",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ExpirationDays",
                table: "LessonPackages");
        }
    }
}
