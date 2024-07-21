using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class alimentarTabla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalle de la villa", new DateTime(2024, 7, 21, 21, 4, 54, 289, DateTimeKind.Local).AddTicks(3191), new DateTime(2024, 7, 21, 21, 4, 54, 289, DateTimeKind.Local).AddTicks(3149), "", 180, "Villa Real", 5, 100.0 },
                    { 2, "", "Detalle de la villeke", new DateTime(2024, 7, 21, 21, 4, 54, 289, DateTimeKind.Local).AddTicks(3195), new DateTime(2024, 7, 21, 21, 4, 54, 289, DateTimeKind.Local).AddTicks(3194), "", 550, "Villa Surreal", 12, 600.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
