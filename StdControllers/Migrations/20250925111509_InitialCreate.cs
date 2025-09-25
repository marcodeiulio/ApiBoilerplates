using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StdControllers.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    HR = table.Column<string>(type: "TEXT", nullable: true),
                    Headquarters = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobApplicationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplicationStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    ApplicationDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobApplications_JobApplicationStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "JobApplicationStatuses",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "HR", "Headquarters", "Name" },
                values: new object[,]
                {
                    { 1, "Some Teacher", "Stockholm", "Dev School" },
                    { 2, "Cool Name", "Göteborg", "Nice Small Company" }
                });

            migrationBuilder.InsertData(
                table: "JobApplicationStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "To Apply" },
                    { 2, "Applied" },
                    { 3, "Scheduled Action" },
                    { 4, "Interview" },
                    { 5, "No Answer" },
                    { 6, "Rejected" },
                    { 7, "Discarded" },
                    { 8, "Offer" }
                });

            migrationBuilder.InsertData(
                table: "JobApplications",
                columns: new[] { "Id", "ApplicationDate", "CompanyId", "Location", "Role", "StatusId" },
                values: new object[,]
                {
                    { 1, new DateOnly(2025, 8, 12), 1, "Stockholm - On Site", "C# Trainee", 4 },
                    { 2, new DateOnly(2025, 9, 12), 2, "Göteborg - Full Remote", "C# Consultant", 7 },
                    { 3, new DateOnly(2025, 9, 29), null, "Full Remote", "C# Dev", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_CompanyId",
                table: "JobApplications",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_StatusId",
                table: "JobApplications",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "JobApplicationStatuses");
        }
    }
}
