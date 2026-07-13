using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptlyNote.Data.Migrations
{
    /// <inheritdoc />
    public partial class reminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemindBeforeMinutes",
                table: "ToDoTasks",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemindBeforeMinutes",
                table: "ToDoTasks");
        }
    }
}
