using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptlyNote.Data.Migrations
{
    /// <inheritdoc />
    public partial class addOrderToSubTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "SubTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "SubTasks");
        }
    }
}
