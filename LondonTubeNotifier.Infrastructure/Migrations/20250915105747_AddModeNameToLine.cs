using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LondonTubeNotifier.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModeNameToLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModeName",
                table: "Lines",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LineStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StatusSeverity = table.Column<int>(type: "int", nullable: false),
                    StatusDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineStatuses_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "bakerloo",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "central",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "circle",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "district",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "hammersmith-city",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "jubilee",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "metropolitan",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "northern",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "piccadilly",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "victoria",
                column: "ModeName",
                value: "");

            migrationBuilder.UpdateData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "waterloo-city",
                column: "ModeName",
                value: "");

            migrationBuilder.CreateIndex(
                name: "IX_LineStatuses_LineId",
                table: "LineStatuses",
                column: "LineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineStatuses");

            migrationBuilder.DropColumn(
                name: "ModeName",
                table: "Lines");
        }
    }
}
