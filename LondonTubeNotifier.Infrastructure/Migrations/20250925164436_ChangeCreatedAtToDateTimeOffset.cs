using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LondonTubeNotifier.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCreatedAtToDateTimeOffset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "bakerloo");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "central");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "circle");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "district");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "hammersmith-city");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "jubilee");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "metropolitan");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "northern");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "piccadilly");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "victoria");

            migrationBuilder.DeleteData(
                table: "Lines",
                keyColumn: "Id",
                keyValue: "waterloo-city");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "LineStatuses",
                type: "datetimeoffset",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "LineStatuses",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.InsertData(
                table: "Lines",
                columns: new[] { "Id", "Code", "Color", "ModeName", "Name" },
                values: new object[,]
                {
                    { "bakerloo", "BL", "#B36305", "", "Bakerloo" },
                    { "central", "CL", "#E32017", "", "Central" },
                    { "circle", "CC", "#FFD300", "", "Circle" },
                    { "district", "DL", "#00782A", "", "District" },
                    { "hammersmith-city", "HCL", "#F3A9BB", "", "Hammersmith & City" },
                    { "jubilee", "JL", "#6A7278", "", "Jubilee" },
                    { "metropolitan", "ML", "#9B0056", "", "Metropolitan" },
                    { "northern", "NL", "#000000", "", "Northern" },
                    { "piccadilly", "PL", "#0019A8", "", "Piccadilly" },
                    { "victoria", "VL", "#0098D4", "", "Victoria" },
                    { "waterloo-city", "WCL", "#95CDBA", "", "Waterloo & City" }
                });
        }
    }
}
