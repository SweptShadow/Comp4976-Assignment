using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ObituaryApp.Migrations
{
    /// <inheritdoc />
    public partial class AddObituaryNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Obituaries_FullName",
                table: "Obituaries",
                column: "FullName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Obituaries_FullName",
                table: "Obituaries");
        }
    }
}
