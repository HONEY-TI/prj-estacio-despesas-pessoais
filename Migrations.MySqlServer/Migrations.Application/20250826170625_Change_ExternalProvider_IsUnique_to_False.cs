using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.MySqlServer.Migrations.Application
{
    /// <inheritdoc />
    public partial class Change_ExternalProvider_IsUnique_to_False : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ControleAcesso_ExternalProvider",
                table: "ControleAcesso");

            migrationBuilder.CreateIndex(
                name: "IX_ControleAcesso_ExternalProvider",
                table: "ControleAcesso",
                column: "ExternalProvider");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ControleAcesso_ExternalProvider",
                table: "ControleAcesso");

            migrationBuilder.CreateIndex(
                name: "IX_ControleAcesso_ExternalProvider",
                table: "ControleAcesso",
                column: "ExternalProvider",
                unique: true);
        }
    }
}
