using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calendar_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Events",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Events",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Events",
                newName: "start");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Events",
                newName: "end");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "title",
                table: "Events",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Events",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "start",
                table: "Events",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "end",
                table: "Events",
                newName: "EndDate");
        }
    }
}
