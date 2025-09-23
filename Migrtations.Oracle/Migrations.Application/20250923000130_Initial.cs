using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Migrtations.Oracle.Migrations.Application
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerfilUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilUsuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoCategoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoCategoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    SobreNome = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Telefone = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    StatusUsuario = table.Column<int>(type: "NUMBER(5)", nullable: false),
                    PerfilUsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Profile = table.Column<string>(type: "CLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_PerfilUsuario_PerfilUsuarioId",
                        column: x => x.PerfilUsuarioId,
                        principalTable: "PerfilUsuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    UsuarioId = table.Column<string>(type: "varchar(36)", nullable: false),
                    TipoCategoriaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categoria_TipoCategoria_TipoCategoriaId",
                        column: x => x.TipoCategoriaId,
                        principalTable: "TipoCategoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Categoria_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ControleAcesso",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Login = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Senha = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(36)", nullable: false),
                    RefreshToken = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ExternalProvider = table.Column<string>(type: "NVARCHAR2(450)", nullable: true),
                    ExternalId = table.Column<string>(type: "NVARCHAR2(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControleAcesso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ControleAcesso_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImagemPerfilUsuario",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Url = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    UsuarioId = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagemPerfilUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagemPerfilUsuario_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Despesa",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Data = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Descricao = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    DataVencimento = table.Column<DateTime>(type: "TIMESTAMP", nullable: true),
                    UsuarioId = table.Column<string>(type: "varchar(36)", nullable: false),
                    CategoriaId = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despesa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Despesa_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Despesa_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Receita",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Data = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Descricao = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    UsuarioId = table.Column<string>(type: "varchar(36)", nullable: false),
                    CategoriaId = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receita", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Receita_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Receita_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Lancamento",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Data = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    UsuarioId = table.Column<string>(type: "varchar(36)", nullable: false),
                    DespesaId = table.Column<string>(type: "varchar(36)", nullable: true),
                    ReceitaId = table.Column<string>(type: "varchar(36)", nullable: true),
                    CategoriaId = table.Column<string>(type: "varchar(36)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TIMESTAMP", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lancamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lancamento_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lancamento_Despesa_DespesaId",
                        column: x => x.DespesaId,
                        principalTable: "Despesa",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lancamento_Receita_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "Receita",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lancamento_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "PerfilUsuario",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Usuario" }
                });

            migrationBuilder.InsertData(
                table: "TipoCategoria",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Despesa" },
                    { 2, "Receita" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_TipoCategoriaId",
                table: "Categoria",
                column: "TipoCategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_UsuarioId",
                table: "Categoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ControleAcesso_ExternalId",
                table: "ControleAcesso",
                column: "ExternalId",
                unique: true,
                filter: "\"ExternalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ControleAcesso_ExternalProvider",
                table: "ControleAcesso",
                column: "ExternalProvider");

            migrationBuilder.CreateIndex(
                name: "IX_ControleAcesso_Login",
                table: "ControleAcesso",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ControleAcesso_UsuarioId",
                table: "ControleAcesso",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesa_CategoriaId",
                table: "Despesa",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesa_UsuarioId",
                table: "Despesa",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagemPerfilUsuario_Name",
                table: "ImagemPerfilUsuario",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagemPerfilUsuario_Url",
                table: "ImagemPerfilUsuario",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagemPerfilUsuario_UsuarioId",
                table: "ImagemPerfilUsuario",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_CategoriaId",
                table: "Lancamento",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_DespesaId",
                table: "Lancamento",
                column: "DespesaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_ReceitaId",
                table: "Lancamento",
                column: "ReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_Lancamento_UsuarioId",
                table: "Lancamento",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Receita_CategoriaId",
                table: "Receita",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Receita_UsuarioId",
                table: "Receita",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_PerfilUsuarioId",
                table: "Usuario",
                column: "PerfilUsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControleAcesso");

            migrationBuilder.DropTable(
                name: "ImagemPerfilUsuario");

            migrationBuilder.DropTable(
                name: "Lancamento");

            migrationBuilder.DropTable(
                name: "Despesa");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "TipoCategoria");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "PerfilUsuario");
        }
    }
}
