using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fragrance.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addsizetod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "ShopingCarts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Parfumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 1,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 2,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 3,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 4,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 5,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 6,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 7,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 8,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 9,
                column: "Size",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 10,
                column: "Size",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "ShopingCarts");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Parfumes");
        }
    }
}
