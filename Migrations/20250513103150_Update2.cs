using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HosterBackend.Migrations
{
    /// <inheritdoc />
    public partial class Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_CustomerTypes_CustomerTypeId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_CustomerTypeId",
                table: "Discounts");

            migrationBuilder.AddColumn<int>(
                name: "DiscountId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerTypeId",
                table: "Discounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiredAt",
                table: "Discounts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DiscountId",
                table: "Orders",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CustomerTypeId",
                table: "Discounts",
                column: "CustomerTypeId",
                unique: true,
                filter: "[CustomerTypeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_CustomerTypes_CustomerTypeId",
                table: "Discounts",
                column: "CustomerTypeId",
                principalTable: "CustomerTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Discounts_DiscountId",
                table: "Orders",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_CustomerTypes_CustomerTypeId",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Discounts_DiscountId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DiscountId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_CustomerTypeId",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "ExpiredAt",
                table: "Discounts");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerTypeId",
                table: "Discounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CustomerTypeId",
                table: "Discounts",
                column: "CustomerTypeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_CustomerTypes_CustomerTypeId",
                table: "Discounts",
                column: "CustomerTypeId",
                principalTable: "CustomerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
