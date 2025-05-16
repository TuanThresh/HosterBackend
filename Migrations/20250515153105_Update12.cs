using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HosterBackend.Migrations
{
    /// <inheritdoc />
    public partial class Update12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegisteredDomains_OrderId",
                table: "RegisteredDomains");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_OrderId",
                table: "RegisteredDomains",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegisteredDomains_OrderId",
                table: "RegisteredDomains");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_OrderId",
                table: "RegisteredDomains",
                column: "OrderId");
        }
    }
}
