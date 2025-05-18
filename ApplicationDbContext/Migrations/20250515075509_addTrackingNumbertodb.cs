using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fragrance.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addTrackingNumbertodb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "OrderHeader");
        }
    }
}
