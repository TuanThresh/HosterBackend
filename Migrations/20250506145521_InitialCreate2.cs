using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HosterBackend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredDomain_DomainAccounts_DomainAccountId",
                table: "RegisteredDomain");

            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredDomain_DomainProducts_DomainProductId",
                table: "RegisteredDomain");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegisteredDomain",
                table: "RegisteredDomain");

            migrationBuilder.RenameTable(
                name: "RegisteredDomain",
                newName: "RegisteredDomains");

            migrationBuilder.RenameIndex(
                name: "IX_RegisteredDomain_DomainProductId",
                table: "RegisteredDomains",
                newName: "IX_RegisteredDomains_DomainProductId");

            migrationBuilder.RenameIndex(
                name: "IX_RegisteredDomain_DomainAccountId",
                table: "RegisteredDomains",
                newName: "IX_RegisteredDomains_DomainAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegisteredDomains",
                table: "RegisteredDomains",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredDomains_DomainAccounts_DomainAccountId",
                table: "RegisteredDomains",
                column: "DomainAccountId",
                principalTable: "DomainAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredDomains_DomainProducts_DomainProductId",
                table: "RegisteredDomains",
                column: "DomainProductId",
                principalTable: "DomainProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredDomains_DomainAccounts_DomainAccountId",
                table: "RegisteredDomains");

            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredDomains_DomainProducts_DomainProductId",
                table: "RegisteredDomains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegisteredDomains",
                table: "RegisteredDomains");

            migrationBuilder.RenameTable(
                name: "RegisteredDomains",
                newName: "RegisteredDomain");

            migrationBuilder.RenameIndex(
                name: "IX_RegisteredDomains_DomainProductId",
                table: "RegisteredDomain",
                newName: "IX_RegisteredDomain_DomainProductId");

            migrationBuilder.RenameIndex(
                name: "IX_RegisteredDomains_DomainAccountId",
                table: "RegisteredDomain",
                newName: "IX_RegisteredDomain_DomainAccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegisteredDomain",
                table: "RegisteredDomain",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredDomain_DomainAccounts_DomainAccountId",
                table: "RegisteredDomain",
                column: "DomainAccountId",
                principalTable: "DomainAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredDomain_DomainProducts_DomainProductId",
                table: "RegisteredDomain",
                column: "DomainProductId",
                principalTable: "DomainProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
