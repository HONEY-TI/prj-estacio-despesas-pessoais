using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.MySqlServer.Migrations.Application
{
    /// <inheritdoc />
    public partial class Profile_in_Usuario_as_LongText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Profile",
                table: "Usuario",
                type: "LONGTEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profile",
                table: "Usuario");
        }
    }
}
