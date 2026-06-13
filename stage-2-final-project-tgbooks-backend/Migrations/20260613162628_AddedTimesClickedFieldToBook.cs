using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stage_2_final_project_tgbooks_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddedTimesClickedFieldToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Books",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "TimesClicked",
                table: "Books",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TimesClicked",
                table: "Books");
        }
    }
}
