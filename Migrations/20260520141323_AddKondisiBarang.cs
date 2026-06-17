using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddKondisiBarang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KondisiBarang",
                table: "Peminjamans",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KondisiBarang",
                table: "Peminjamans");
        }
    }
}
