using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.MySqlServer.Migrations.Application
{
    /// <inheritdoc />
    public partial class GoogleCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "ControleAcesso",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalProvider",
                table: "ControleAcesso",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ControleAcesso_ExternalId",
                table: "ControleAcesso",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ControleAcesso_ExternalProvider",
                table: "ControleAcesso",
                column: "ExternalProvider",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ControleAcesso_ExternalId",
                table: "ControleAcesso");

            migrationBuilder.DropIndex(
                name: "IX_ControleAcesso_ExternalProvider",
                table: "ControleAcesso");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "ControleAcesso");

            migrationBuilder.DropColumn(
                name: "ExternalProvider",
                table: "ControleAcesso");
        }
    }
}
