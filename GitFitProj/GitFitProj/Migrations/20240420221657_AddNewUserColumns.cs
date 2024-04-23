using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GitFitProj.Migrations
{
    /// <inheritdoc />
    public partial class AddNewUserColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyStepGoal",
                table: "UserModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreferredActivityType",
                table: "UserModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredIntensity",
                table: "UserModel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "WeightGoal",
                table: "UserModel",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "AverageHeartRate",
                table: "Activity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Intensity",
                table: "Activity",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyStepGoal",
                table: "UserModel");

            migrationBuilder.DropColumn(
                name: "PreferredActivityType",
                table: "UserModel");

            migrationBuilder.DropColumn(
                name: "PreferredIntensity",
                table: "UserModel");

            migrationBuilder.DropColumn(
                name: "WeightGoal",
                table: "UserModel");

            migrationBuilder.DropColumn(
                name: "AverageHeartRate",
                table: "Activity");

            migrationBuilder.DropColumn(
                name: "Intensity",
                table: "Activity");
        }
    }
}
