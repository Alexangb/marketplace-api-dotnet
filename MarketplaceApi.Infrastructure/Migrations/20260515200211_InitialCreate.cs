using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MarketplaceApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categorias_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    apellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estado = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    FotoUrl = table.Column<string>(type: "text", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Direccion = table.Column<string>(type: "text", nullable: true),
                    Biografia = table.Column<string>(type: "text", nullable: true),
                    UltimoAcceso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usuarios_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "horarios_disponibles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: true),
                    dia_semana = table.Column<int>(type: "integer", nullable: false),
                    hora_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    hora_fin = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("horarios_disponibles_pkey", x => x.id);
                    table.ForeignKey(
                        name: "horarios_disponibles_usuario_id_fkey",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "servicios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    precio = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    duracion_minutos = table.Column<int>(type: "integer", nullable: false),
                    categoria_id = table.Column<int>(type: "integer", nullable: true),
                    usuario_id = table.Column<int>(type: "integer", nullable: true),
                    estado = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Visitas = table.Column<int>(type: "integer", nullable: true),
                    PrecioDescuento = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxReservasPorDia = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("servicios_pkey", x => x.id);
                    table.ForeignKey(
                        name: "servicios_categoria_id_fkey",
                        column: x => x.categoria_id,
                        principalTable: "categorias",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "servicios_usuario_id_fkey",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "resenas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: true),
                    servicio_id = table.Column<int>(type: "integer", nullable: true),
                    calificacion = table.Column<int>(type: "integer", nullable: true),
                    comentario = table.Column<string>(type: "text", nullable: true),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("resenas_pkey", x => x.id);
                    table.ForeignKey(
                        name: "resenas_servicio_id_fkey",
                        column: x => x.servicio_id,
                        principalTable: "servicios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "resenas_usuario_id_fkey",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "reservas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: true),
                    servicio_id = table.Column<int>(type: "integer", nullable: true),
                    fecha_reserva = table.Column<DateOnly>(type: "date", nullable: false),
                    hora_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    hora_fin = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValueSql: "'Pendiente'::character varying"),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    MotivoCancelacion = table.Column<string>(type: "text", nullable: true),
                    FechaCancelacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PrecioFinal = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reservas_pkey", x => x.id);
                    table.ForeignKey(
                        name: "reservas_servicio_id_fkey",
                        column: x => x.servicio_id,
                        principalTable: "servicios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "reservas_usuario_id_fkey",
                        column: x => x.usuario_id,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_horarios_disponibles_usuario_id",
                table: "horarios_disponibles",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_resenas_servicio_id",
                table: "resenas",
                column: "servicio_id");

            migrationBuilder.CreateIndex(
                name: "IX_resenas_usuario_id",
                table: "resenas",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "idx_reserva_usuario",
                table: "reservas",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservas_servicio_id",
                table: "reservas",
                column: "servicio_id");

            migrationBuilder.CreateIndex(
                name: "idx_servicio_categoria",
                table: "servicios",
                column: "categoria_id");

            migrationBuilder.CreateIndex(
                name: "IX_servicios_usuario_id",
                table: "servicios",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "idx_usuario_email",
                table: "usuarios",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "usuarios_email_key",
                table: "usuarios",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "horarios_disponibles");

            migrationBuilder.DropTable(
                name: "resenas");

            migrationBuilder.DropTable(
                name: "reservas");

            migrationBuilder.DropTable(
                name: "servicios");

            migrationBuilder.DropTable(
                name: "categorias");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
