using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stage_2_final_project_tgbooks_backend.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteBookMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role",
                table: "Users",
                newName: "Role");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "role");
        }
    }
}
