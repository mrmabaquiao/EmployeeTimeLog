using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmployeeTimeLog.Data.Migrations
{
    public partial class TimeLogToDatabaseAddTimeInAndOut : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeIn",
                table: "TimeLog",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOut",
                table: "TimeLog",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeIn",
                table: "TimeLog");

            migrationBuilder.DropColumn(
                name: "TimeOut",
                table: "TimeLog");
        }
    }
}
