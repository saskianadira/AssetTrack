using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetTrack.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kondisi",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Assets",
                newName: "Foto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Foto",
                table: "Assets",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Kondisi",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
