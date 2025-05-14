using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HosterBackend.Migrations
{
    /// <inheritdoc />
    public partial class Update5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Discounts_CustomerTypeId",
                table: "Discounts");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CustomerTypeId",
                table: "Discounts",
                column: "CustomerTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Discounts_CustomerTypeId",
                table: "Discounts");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CustomerTypeId",
                table: "Discounts",
                column: "CustomerTypeId",
                unique: true,
                filter: "[CustomerTypeId] IS NOT NULL");
        }
    }
}
