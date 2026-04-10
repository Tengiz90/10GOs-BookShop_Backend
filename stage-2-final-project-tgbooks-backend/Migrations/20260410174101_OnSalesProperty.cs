using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stage_2_final_project_tgbooks_backend.Migrations
{
    /// <inheritdoc />
    public partial class OnSalesProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OffPercentage",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "OnSale",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OffPercentage",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "OnSale",
                table: "Books");
        }
    }
}
