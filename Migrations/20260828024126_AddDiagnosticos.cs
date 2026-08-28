using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitorErrores.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnosticos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diagnosticos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ErrorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    Servicio = table.Column<string>(type: "TEXT", nullable: false),
                    EsConocido = table.Column<bool>(type: "INTEGER", nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    ErroresRecientes = table.Column<int>(type: "INTEGER", nullable: false),
                    EsRecurrente = table.Column<bool>(type: "INTEGER", nullable: false),
                    NivelGravedad = table.Column<string>(type: "TEXT", nullable: false),
                    Recomendacion = table.Column<string>(type: "TEXT", nullable: false),
                    FechaDiagnostico = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnosticos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnosticos");
        }
    }
}
