using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class V002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "comment");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStamp",
                table: "comment_reaction",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "comment_reaction");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "comment",
                type: "varchar(32)",
                nullable: false,
                defaultValue: "");
        }
    }
}
