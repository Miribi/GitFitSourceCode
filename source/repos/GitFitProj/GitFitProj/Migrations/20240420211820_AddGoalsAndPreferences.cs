using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitFitProj.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalsAndPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivityDate",
                table: "Activity",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "CaloriesBurned",
                table: "Activity",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "Activity",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityDate",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "CaloriesBurned",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Activity");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "UserModel",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
