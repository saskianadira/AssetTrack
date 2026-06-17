using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddKeteranganPengembalian : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeteranganPengembalian",
                table: "Peminjamans",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeteranganPengembalian",
                table: "Peminjamans");
        }
    }
}
