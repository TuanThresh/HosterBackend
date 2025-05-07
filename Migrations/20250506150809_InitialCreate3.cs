using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HosterBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "RegisteredDomains",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomains_OrderId",
                table: "RegisteredDomains",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredDomains_Orders_OrderId",
                table: "RegisteredDomains",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredDomains_Orders_OrderId",
                table: "RegisteredDomains");

            migrationBuilder.DropIndex(
                name: "IX_RegisteredDomains_OrderId",
                table: "RegisteredDomains");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "RegisteredDomains");
        }
    }
}
