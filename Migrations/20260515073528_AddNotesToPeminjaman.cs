using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesToPeminjaman : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Peminjamans",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Peminjamans");
        }
    }
}
