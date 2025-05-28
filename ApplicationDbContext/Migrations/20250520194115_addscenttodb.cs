using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fragrance.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addscenttodb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScentNotes",
                table: "Parfumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScentProfile",
                table: "Parfumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 1,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 2,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 3,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 4,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 5,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 6,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 7,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 8,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 9,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 10,
                columns: new[] { "ScentNotes", "ScentProfile" },
                values: new object[] { "Ginger,Vanilla,Amber", "Spicy,Woody" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScentNotes",
                table: "Parfumes");

            migrationBuilder.DropColumn(
                name: "ScentProfile",
                table: "Parfumes");
        }
    }
}
