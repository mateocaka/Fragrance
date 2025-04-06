using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fragrance.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addsizequantityratignondb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Parfumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Parfumes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Size100",
                table: "Parfumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Size30",
                table: "Parfumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Size50",
                table: "Parfumes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 1,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.0999999999999996, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 2,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.7000000000000002, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 3,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.5, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 4,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.9000000000000004, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 5,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.7999999999999998, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 6,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.2000000000000002, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 7,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.2999999999999998, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 8,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.0, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 9,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.2999999999999998, 100, 30, 50 });

            migrationBuilder.UpdateData(
                table: "Parfumes",
                keyColumn: "ParfumeId",
                keyValue: 10,
                columns: new[] { "Quantity", "Rating", "Size100", "Size30", "Size50" },
                values: new object[] { 99, 4.5, 100, 30, 50 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Parfumes");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Parfumes");

            migrationBuilder.DropColumn(
                name: "Size100",
                table: "Parfumes");

            migrationBuilder.DropColumn(
                name: "Size30",
                table: "Parfumes");

            migrationBuilder.DropColumn(
                name: "Size50",
                table: "Parfumes");
        }
    }
}
