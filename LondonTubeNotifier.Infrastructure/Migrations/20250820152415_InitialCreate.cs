using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LondonTubeNotifier.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Lines",
                columns: new[] { "Id", "Code", "Color", "Name" },
                values: new object[,]
                {
                    { "bakerloo", "BL", "#B36305", "Bakerloo" },
                    { "central", "CL", "#E32017", "Central" },
                    { "circle", "CC", "#FFD300", "Circle" },
                    { "district", "DL", "#00782A", "District" },
                    { "hammersmith-city", "HCL", "#F3A9BB", "Hammersmith & City" },
                    { "jubilee", "JL", "#6A7278", "Jubilee" },
                    { "metropolitan", "ML", "#9B0056", "Metropolitan" },
                    { "northern", "NL", "#000000", "Northern" },
                    { "piccadilly", "PL", "#0019A8", "Piccadilly" },
                    { "victoria", "VL", "#0098D4", "Victoria" },
                    { "waterloo-city", "WCL", "#95CDBA", "Waterloo & City" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lines");
        }
    }
}
