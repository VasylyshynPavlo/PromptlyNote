using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptlyNote.Data.Migrations
{
    /// <inheritdoc />
    public partial class syncToGoogleCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SyncToGoogleCalendar",
                table: "ToDoTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SyncToGoogleCalendar",
                table: "ToDoTasks");
        }
    }
}
