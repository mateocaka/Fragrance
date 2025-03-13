using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fragrance.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addauthortoparfume : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Parfumes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 1,
                column: "Author",
                value: "Jean Paul Gaultier");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 2,
                column: "Author",
                value: "Parfume de Marly");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 3,
                column: "Author",
                value: "Viktor Rolf");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 4,
                column: "Author",
                value: "Maison Margiela");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 5,
                column: "Author",
                value: "Armani");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 6,
                column: "Author",
                value: "Azzaro");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 7,
                column: "Author",
                value: "Tom Ford");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 8,
                column: "Author",
                value: "Tom Ford");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 9,
                column: "Author",
                value: "Yves Saint Laurent");

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 10,
                column: "Author",
                value: "Chanel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "Parfumes");
        }
    }
}
